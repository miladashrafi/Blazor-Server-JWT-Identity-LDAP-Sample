using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Backend.API.Data;
using Backend.API.Entities;
using Backend.API.Interfaces;
using Backend.API.Permissions;
using Backend.API.Services;
using Backend.API.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Backend.API;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        Environment = env;
        if (string.IsNullOrEmpty(env.EnvironmentName)) env.EnvironmentName = "Development";
    }

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.

    private void RegisterPolicies(AuthorizationOptions options)
    {
        var permissionList = new PermissionList();
        var policyPermissionMapping = new Dictionary<string, Permission>
        {
            {
                PolicyTypes.Users.View,
                permissionList.GetPermissionByKey(AdministrativePermission.AdministrativeViewUser)
            },
            {
                PolicyTypes.Users.Manage,
                permissionList.GetPermissionByKey(AdministrativePermission.AdministrativeManageUser)
            }
        };

        foreach (var (policy, permission) in policyPermissionMapping)
            options.AddPolicy(policy, p => { p.RequireClaim(ClaimConstants.PermissionClaimName, permission.Value); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors();
        services.AddControllers()
            .AddJsonOptions(jsonOptions =>
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.AddAuthorization(RegisterPolicies);

        services.AddDbContextPool<ApplicationDbContext>(opt =>
        {
            opt.UseSqlServer(Configuration.GetConnectionString("IdentityDB"));
        });

        services.AddIdentity<ApplicationUser, ApplicationRole>(t =>
        {
            t.Password.RequireDigit = false;
            t.Password.RequireNonAlphanumeric = false;
            t.Password.RequireUppercase = false;
            t.Password.RequireLowercase = false;
            t.Password.RequiredLength = 6;
        }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = Configuration.GetValue<string>("JWTSettings:Issuer"),
                ValidAudience = Configuration.GetValue<string>("JWTSettings:Issuer"),
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration.GetValue<string>("JWTSettings:Secret"))),
                ClockSkew = TimeSpan.Zero
            };
            cfg.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        context.Response.Headers.Add("Token-Expired", "true");

                    return Task.CompletedTask;
                }
            };
        });

        services.AddMemoryCache();

        services.Configure<LdapSetting>(Configuration.GetSection("LdapSetting"));
        services.Configure<JwtSettings>(Configuration.GetSection("JWTSettings"));
        
        services.AddTransient<ILdapService, LDAPService>();

        services.AddScoped<IAccessControlService, AccessControlService>();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "BACKEND API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new[] { "api1", "openid" }
                }
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
            app.UseHsts();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BACKEND API V1"));

        app.UseRouting();

        app.UseCors(opt => opt.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}