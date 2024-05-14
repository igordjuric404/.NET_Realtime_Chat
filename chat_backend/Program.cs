using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using _NET_Realtime_Chat.Data;
using _NET_Realtime_Chat.Services;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebApp",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var key = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(key))
{
    throw new InvalidOperationException("JWT Key is not configured");
}

var keyBytes = Encoding.ASCII.GetBytes(key);

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("OnAuthenticationFailed: " + context.Exception.Message);

                // Attempt to manually validate token again within the event
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var token = context.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
                    if (handler.CanReadToken(token))
                    {
                        var jwtToken = handler.ReadJwtToken(token);
                        Console.WriteLine("Manual validation within OnAuthenticationFailed:");
                        Console.WriteLine("Header: " + jwtToken.Header.SerializeToJson());
                        Console.WriteLine("Payload: " + jwtToken.Payload.SerializeToJson());
                        Console.WriteLine("Signature: " + jwtToken.RawSignature);

                        // Validate token manually
                        var validationParameters = new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };

                        SecurityToken validatedToken;
                        var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                        if (principal is not null)
                        {
                            Console.WriteLine("Manual validation within event succeeded");
                        }
                        else
                        {
                            Console.WriteLine("Manual validation within event failed");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during manual validation within event: " + ex.Message);
                }

                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var token = context.SecurityToken as JwtSecurityToken;
                if (token != null)
                {
                    Console.WriteLine("OnTokenValidated: " + token.RawData);
                }
                else
                {
                    Console.WriteLine("Token is null or not JwtSecurityToken");
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Headers.ContainsKey("Authorization"))
    {
        var authHeader = context.Request.Headers["Authorization"].ToString();
        Console.WriteLine("Authorization Header: " + authHeader);

        if (authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            Console.WriteLine("Token from header: " + token);

            var handler = new JwtSecurityTokenHandler();
            if (handler.CanReadToken(token))
            {
                var jwtToken = handler.ReadJwtToken(token);
                Console.WriteLine("Token is well-formed: " + jwtToken.RawData);

                // Inspecting parts of the JWT
                Console.WriteLine("Header: " + jwtToken.Header.SerializeToJson());
                Console.WriteLine("Payload: " + jwtToken.Payload.SerializeToJson());
                Console.WriteLine("Signature: " + jwtToken.RawSignature);

                try
                {
                    // Manual validation
                    var principal = ValidateToken(token, key);
                    if (principal is not null)
                    {
                        Console.WriteLine("Manual validation succeeded");
                    }
                    else
                    {
                        Console.WriteLine("Manual validation failed");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception during manual validation: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Token cannot be read");
            }
        }
        else
        {
            Console.WriteLine("Authorization header does not start with 'Bearer '");
        }
    }
    else
    {
        Console.WriteLine("No Authorization header present");
    }

    await next();
    Console.WriteLine("Passed custom middleware");
});

app.UseCors("AllowWebApp");

app.Use(async (context, next) =>
{
    Console.WriteLine("Before UseAuthentication");
    await next();
    Console.WriteLine("After UseAuthentication");
});

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chathub");

app.Run();

// Helper method for manual token validation
static ClaimsPrincipal? ValidateToken(string token, string secret)
{
    try
    {
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

        Console.WriteLine("Manual Token Validation Successful");
        return principal;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Manual Token Validation Failed: {ex.Message}");
        return null;
    }
}
