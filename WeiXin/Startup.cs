using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using log4net.Repository;
using Microsoft.EntityFrameworkCore;

namespace WeiXin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            loggerRepository = log4net.LogManager.CreateRepository("NETCoreRepository");
            log4net.Config.XmlConfigurator.Configure(loggerRepository, new System.IO.FileInfo("log4net.config"));
        }

        public IConfiguration Configuration { get; }
        public static ILoggerRepository loggerRepository { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataBase.AccountContext>(option => { option.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); });
            services.AddMemoryCache();
            services.AddMvc();
            services.AddMvc(options =>
            {
                options.Filters.Add<Common.HttpGlobalExceptionFilter>();
            });
            services.AddMvc(config =>
            {
                // Add XML Content Negotiation
                config.RespectBrowserAcceptHeader = true;
                config.InputFormatters.Add(new XmlSerializerInputFormatter());
                config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
