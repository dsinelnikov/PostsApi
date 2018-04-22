using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PostsApi.Models;
using PostsApi.Services;

namespace PostsApi
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
            //var connectionString = @"Server=ODP1208023\SQLEXPRESS;Database=Posts;User Id=sa;Password=123456;";
            //var connectionString = "Server=tcp:posts.database.windows.net,1433;Initial Catalog=Posts;Persist Security Info=False;User ID=denis;Password=P@ssw0rd;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //services.AddDbContext<PostsContext>(opt => opt.UseSqlServer(connectionString));
            services.AddDbContext<PostsContext>(opt => opt.UseSqlServer(Configuration.GetConnectionString("PostsDbConnection")));
            services.AddSingleton<ISiteSettings, SiteSettings>();
            services.AddMvc();
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
