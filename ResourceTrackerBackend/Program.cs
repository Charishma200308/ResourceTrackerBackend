using Microsoft.IdentityModel.Tokens;
using ResourceTrackerBackend.DAO;
using ResourceTrackerBackend.Orchestration;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace ResourceTrackerBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "your-super-secret-key";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
            .CreateLogger();



            builder.Services.AddSingleton<IEmployeeOrch, Repositry>();
            builder.Services.AddSingleton<EmployeeService>();
            builder.Services.AddScoped<IAuthService, AuthService>();


            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularApp", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog();

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();

                    try
                    {
                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = key,
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ClockSkew = TimeSpan.Zero
                        }, out SecurityToken validatedToken);

                        var jwtToken = (JwtSecurityToken)validatedToken;
                        var claims = jwtToken.Claims;

                        // ?? Create ClaimsIdentity and ClaimsPrincipal
                        var identity = new ClaimsIdentity(claims, "jwt");
                        var principal = new ClaimsPrincipal(identity);
                        context.User = principal;
                    }
                    catch
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid Token");
                        return;
                    }
                }

                await next();
            });

            app.UseCors(cors => cors.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseAuthentication();


            app.UseCors("AllowAngularApp");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
