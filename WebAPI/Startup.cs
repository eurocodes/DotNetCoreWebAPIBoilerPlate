using AutoMapper;
using Core.Application.DTOs.Configurations;
using Infrastructure;
using Infrastructure.Queue.JobConsumer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Extensions;

namespace WebAPI {
    public class Startup {
        public static IWebHostEnvironment envr;
        public static SystemVariables appSettings;
        public Startup(IConfiguration configuration, IWebHostEnvironment env) {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            string fileName = string.Concat("appsettings.", env.EnvironmentName, ".json");
            builder.AddJsonFile(fileName, optional: true);
            Configuration = builder.Build();
            envr = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddCors(options => {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });
            Core.Application.DI.Resolve.resolve(services);
            Infrastructure.DI.Resolve.resolve(services);
            services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddHttpContextAccessor();
            services.AddMvc().AddNewtonsoftJson();
            services.AddResponseCompression(Options => {
                Options.EnableForHttps = true;
                Options.Providers.Add<GzipCompressionProvider>();
            });
            /*services.AddStackExchangeRedisCache(options => {
                options.Configuration = "localhost:6379";
            });*/            
            var appSettingsSection = Configuration.GetSection("SystemVariables");
            services.Configure<SystemVariables>(appSettingsSection);
            appSettings = appSettingsSection.Get<SystemVariables>();
            var cnfg = new MapperConfiguration(cfg => {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;                
            });
            var mapper = cnfg.CreateMapper();
            services.AddSingleton(mapper);
            services.AddHostedService<EmailSender>();
            services.AddDbContextSQLServer(envr, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
            app.UseResponseCompression();

        }
    }
}
