using System;
using System.Net;
using System.Text;
using AutoMapper;
using CoreTools.Extensions;
using CoreTools.Mvc;
using CoreTools.Tokens.Jwt;
using FluentValidation.AspNetCore;
using HelpDeskCore.Data;
using HelpDeskCore.Data.DependencyInjection;
using HelpDeskCore.Data.Entities;
using HelpDeskCore.Extensions;
using HelpDeskCore.Models;
using HelpDeskCore.Services.Emails;
using HelpDeskCore.Services.Notifications;
using HelpDeskCore.Shared.Messaging;
using MailkitTools.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskCore
{
  public class Startup
  {
    readonly string SecretKey;
    readonly SymmetricSecurityKey _signingKey;

    internal static IConfiguration InternalConfiguration;

    public Startup(IConfiguration configuration, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        var builder = new ConfigurationBuilder();
        builder.AddUserSecrets<Startup>();
        configuration = builder.Build();
      }
      InternalConfiguration = Configuration = configuration;
      SecretKey = configuration["SecurityKey"];
      _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      const string CONN_STR = "DefaultConnection";

      // Add framework services.
      services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(Configuration.GetConnectionString(CONN_STR),
              b => b.MigrationsAssembly("HelpDeskCore.Data")));

      services
        .AddMailkitTools<EmailConfigurationProvider>()
        .AddHelpDeskCoreRepositories(() => new ApplicationDbContext(Configuration.GetConnectionString(CONN_STR)))
        .AddHelpDeskCoreServices()
        .Configure<MessageDispatchOptions>(Configuration.GetSection(nameof(MessageDispatchOptions)))
        .Configure<IdentityInitializerSettings>(Configuration.GetSection(nameof(IdentityInitializerSettings)))
        .Configure<FacebookAuthSettings>(Configuration.GetSection(nameof(FacebookAuthSettings)));

      services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

      // jwt wire up
      services.AddSingleton<IJwtFactory, JwtFactory>();

      // Get options from app settings
      var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
      var jwtAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];

      // Configure JwtIssuerOptions
      services.Configure<JwtIssuerOptions>(options =>
      {
        options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
        options.Audience = jwtAudience;
        options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
      });

      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

        ValidateAudience = jwtAppSettingOptions.GetValue<bool>("ValidateAudience"),
        ValidAudience = jwtAudience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = _signingKey,

        RequireExpirationTime = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
      };

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      }).AddJwtBearer(options =>
      {
        options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
        options.TokenValidationParameters = tokenValidationParameters;
        options.SaveToken = true;
      });

      services.AddSignalR();

      // api user claim policy
      services.AddAuthorization(options =>
      {
        options.AddPolicy("ApiUser", policy => policy.RequireClaim(JwtClaimIdentifiers.Rol, JwtClaims.ApiAccess));
      });

      // add identity
      var builder = services.AddIdentityCore<AppUser>(o =>
      {
        // configure identity options
        var p = o.Password;
        p.RequireDigit = false;
        p.RequireLowercase = false;
        p.RequireUppercase = false;
        p.RequireNonAlphanumeric = false;
        p.RequiredLength = 6;
      });

      builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
      builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

      services
        .AddAutoMapper()
        .AddCors(o => o.AddPolicy("CorsPolicy", cpb =>
        {
          cpb
          .AllowAnyMethod()
          .AllowAnyHeader()
          .WithOrigins(jwtAudience);
        }))
        .AddMvc(options =>
        {
          options.Filters.Add(typeof(ValidateModelAttribute));
        })
        .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>());
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseExceptionHandler(builder =>
      {
        builder.Run(async context =>
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
          context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

          var error = context.Features.Get<IExceptionHandlerFeature>();
          if (error != null)
          {
            context.Response.AddApplicationError(error.Error.Message);
            await context.Response.WriteAsync(error.Error.Message).ConfigureAwait(false);
          }
        });
      })
      .UseDefaultFiles()
      .UseStaticFiles()
      .UseCors("CorsPolicy")
      .UseQueryStringAuth()
      .UseAuthentication()
      .UseSignalR(routes => routes.MapHub<NotificationHub>("/api/notify"))
      .UseMvc();
    }
  }
}
