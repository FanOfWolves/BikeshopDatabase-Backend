using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net.Http;

namespace DatabaseApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new MapperConfiguration(conf => {
                conf.ForAllMaps((obj, cfg) => cfg.ForAllMembers(options => options.Condition((source, dest, sourceMember) => sourceMember != null)));
                conf.AddProfile(new AutoMapperProfiles());
            });
            services.AddSingleton<IMapper>(mapServ => config.CreateMapper());
            services.AddSingleton<Helpers.MonitoringService>(monServ => new Helpers.MonitoringService("https://metricsapi20201007030533.azurewebsites.net"));
            services.AddDbContext<BikeShop_Context>(options => options.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddAutoMapper(typeof(Startup));
            services.AddControllers();
            services.AddSwaggerGen(c => {
                // Set the comments path for the Swagger JSON and UI.
                // ENABLES xml comments to also be generated
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
    
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
            );

            app.UseSwagger();
            //Route: hostingUrl/swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Website API");
            });

            //Allow for forwarding with ngnix
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
