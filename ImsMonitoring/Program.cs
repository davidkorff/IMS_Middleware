using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ImsMonitoring.Data;
using ImsMonitoring.Models;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ImsMonitoring.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add ImsValidationService to the DI container
builder.Services.AddScoped<IImsValidationService, ImsValidationService>();

// Add CORS for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins(
                "http://localhost:5173",  // Vite's default port
                "http://localhost:3001",  // Vite's default port
                "http://localhost:3000"  // Your current frontend port
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());  // Add this if you're using cookies/auth
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();

// Add these lines in this order
app.UseAuthentication();
app.UseAuthorization();

// Add this line to map your controllers
app.MapControllers();

// Auth endpoints
app.MapPost("/api/auth/signup", async (ApplicationDbContext db, SignupRequest request) =>
{
    // Check if email already exists
    if (await db.Users.AnyAsync(u => u.Email == request.Email))
    {
        return Results.BadRequest("Email already registered");
    }

    // Hash password
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

    // Create user
    var user = new ApplicationUser
    {
        Email = request.Email,
        UserName = request.Email,
        PasswordHash = passwordHash
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok("User registered successfully");
});

app.MapPost("/api/auth/login", async (ApplicationDbContext db, LoginRequest request) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
    if (user == null)
    {
        return Results.BadRequest("Invalid credentials");
    }

    if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
    {
        return Results.BadRequest("Invalid credentials");
    }

    // Generate JWT token
    var token = GenerateJwtToken(user);

    return Results.Ok(new { token = token, message = "Login successful" });
});

app.MapGet("/api/auth/profile", async (ApplicationDbContext db, HttpContext context) =>
{
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
    {
        return Results.Unauthorized();
    }

    var user = await db.Users.FindAsync(userId);
    if (user == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(new
    {
        email = user.Email,
        // Add more profile fields as needed
    });
})
.RequireAuthorization(); // This ensures the endpoint requires authentication

// Add this method to generate JWT tokens
string GenerateJwtToken(ApplicationUser user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found"));
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

app.Run();

// Request models
public record SignupRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);
