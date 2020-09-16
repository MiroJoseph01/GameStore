using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Payments;
using GameStore.BLL.Services;
using GameStore.DAL;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Repositories;
using GameStore.Web.Filters;
using GameStore.Web.Util;
using GameStore.Web.Util.AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameStore.Web
{
    public class Startup
    {
        private const string DevelopmentConnectionString = "DevelopmentConnection";
        private const string DefaultConnectionString = "DefaultConnection";
        private readonly IConfigurationRoot _configurationString;
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            _configurationString = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("db_settings.json").Build();
            _currentEnvironment = hostingEnvironment;
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAutoMapper(services);

            ConfigureRepositories(services);

            ConfigureApplicationServices(services);

            services.AddScoped<LoggingFilter>();

            if (_currentEnvironment.EnvironmentName == "Development")
            {
                services.AddDbContext<GameStoreContext>(i =>
                    i.UseSqlServer(
                        _configurationString
                            .GetConnectionString(DevelopmentConnectionString)));
            }
            else
            {
                services.AddDbContext<GameStoreContext>(i =>
                    i.UseSqlServer(
                        _configurationString
                            .GetConnectionString(DefaultConnectionString)));
            }

            services.AddMvc(option =>
            {
                //option.EnableEndpointRouting = false;
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

            using (
                IServiceScope serviceScope = app
                    .ApplicationServices
                    .GetService<IServiceScopeFactory>()
                    .CreateScope())
            {
                GameStoreContext context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<GameStoreContext>();
                context.Database.EnsureCreated();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();

            services.AddScoped<ICommentService, CommentService>();

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<IGenreService, GenreService>();

            services.AddScoped<IPlatformService, PlatformService>();

            services.AddScoped<IPublisherService, PublisherService>();

            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IPaymentContext, PaymentContext>();
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services
                .AddScoped(
                typeof(IRepository<>),
                typeof(Repository<>));

            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped<ICommentRepository, CommentRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            IMapper mapper = AutoMapperConfiguration.Configure();

            services.AddSingleton(mapper);
        }
    }
}
