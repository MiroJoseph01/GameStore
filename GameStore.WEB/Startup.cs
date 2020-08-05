using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Services;
using GameStore.DAL;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.RepostitoriesInterfaces;
using GameStore.DAL.Repositories;
using GameStore.WEB.Filters;
using GameStore.WEB.Util;
using GameStore.WEB.Util.AutoMapperProfiles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameStore.WEB
{
    public class Startup
    {
        private readonly IConfigurationRoot _configurationString;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _configuration = configuration;

            _configurationString = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("db_settings.json").Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            MapperConfiguration mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new GameProfile());
                mc.AddProfile(new CommentProfile());
                mc.AddProfile(new GenreProfile());
                mc.AddProfile(new PlatformProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();

            services.AddSingleton(mapper);

            services.AddScoped<LoggingFilter>();

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IGameService, GameService>();

            services.AddScoped<ICommentService, CommentService>();

            services.AddScoped<IFileService, FileService>();

            services.AddDbContext<GameStoreContext>(i =>
                i.UseSqlServer(_configurationString.GetConnectionString("DefaultConnection")));

            services.AddMvc(option =>
            {
                option.EnableEndpointRouting = false;
                option.Filters.Add(typeof(LoggingFilter));
                option.Filters.Add(typeof(ExceptionFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                GameStoreContext context = serviceScope.ServiceProvider.GetRequiredService<GameStoreContext>();
                context.Database.EnsureCreated();
            }

            app.UseStaticFiles();

            app.UseStatusCodePages();

            app.UseRouting();

            app.UseMvcWithDefaultRoute();
        }
    }
}
