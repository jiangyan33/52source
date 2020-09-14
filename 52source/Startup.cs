using _52source.Middlewares;
using _52sourceService;
using CommonEntity.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace _52source
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var option = new DBOptions();
            Configuration.GetSection("ConnectionOptions").Bind(option);
            IndexService.InitDbUtils(option);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // 通过反射，将指定的服务全都注入到服务容器中
            // loadFrom:已知程序集的文件名或路径，加载程序集。loadFile是加载程序集中的内容，在这里无法使用
            var assTypes = System.Reflection.Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "52SourceService.dll").GetTypes();
            foreach (var item in assTypes)
            {
                if (item.Namespace == "_52sourceService.BusinessService")
                {
                    services.AddTransient(item);
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (env.IsDevelopment())
            {
                app.UseCorsMiddleware();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    RequestPath = "/52doc",
            //    FileProvider = new PhysicalFileProvider("D:\\jiangyan33\\myproject\\52doc")
            //});
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}