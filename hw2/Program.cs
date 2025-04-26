using hw2.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // Add this using for Swagger Security
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<PasswordHashingService>();
builder.Services.AddSingleton<AuthTokenService>();

// --- Add JWT Authentication --- 
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Is this true?
        ValidateAudience = true, // Is this true?
        ValidateLifetime = true, // Is this true?
        ValidateIssuerSigningKey = true, // Is this true?
        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Reads from config
        ValidAudience = builder.Configuration["Jwt:Audience"], // Reads from config
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Reads from config
    };
});
// --- End JWT Authentication ---

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => // Modify AddSwaggerGen
{
    // Define the BearerAuth security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http, // Use Http for Bearer
        Scheme = "bearer", // Lowercase bearer
        BearerFormat = "JWT"
    });

    // Make sure Swagger UI requires a Bearer token to be specified
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {} // No specific scopes required
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyOrigin
    ().AllowAnyHeader().AllowAnyMethod());
app.UseHttpsRedirection();

// --- Add Authentication Middleware --- 
// IMPORTANT: Must be before UseAuthorization
app.UseAuthentication();
// --- End Authentication Middleware ---

app.UseAuthorization();

app.MapControllers();

app.Run();
