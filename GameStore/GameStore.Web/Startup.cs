using System;
using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using GameStore.BLL.Interfaces;
using GameStore.BLL.Interfaces.Services;
using GameStore.BLL.Payments;
using GameStore.BLL.Services;
using GameStore.DAL;
using GameStore.DAL.Interfaces;
using GameStore.DAL.Interfaces.Repositories;
using GameStore.DAL.Pipeline;
using GameStore.DAL.Repositories.Facade;
using GameStore.Web.Util;
using GameStore.Web.Util.AutoMapper;
using GameStore.Web.Util.Filters;
using GameStore.Web.Util.Logger;
using GameStore.Web.Util.ModelBinders;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDb = GameStore.DAL.Repositories.Mongo;
using SqlDb = GameStore.DAL.Repositories.Sql;

namespace GameStore.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string DevelopmentConnectionString = "DevelopmentConnection";
        private const string DefaultConnectionString = "DefaultConnection";
        private const string DevelopmentMongoConnectionString = "DevelopmentMongoConnection";
        private const string DefaultMongoConnectionString = "DefaultMongoConnection";
        private const string DevelopmentDatabaseNamePath = "MongoConnection:DevelopmentName";
        private const string DefaultDatabaseNamePath = "MongoConnection:DefaultName";
        private const string CreateNewIdsPath = "MongoConnection:NewIds";
        private readonly IConfigurationRoot _configurationString;
        private readonly IWebHostEnvironment _currentEnvironment;

        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            _configurationString = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("db_settings.json").Build();

            _currentEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAutoMapper(services);

            ConfigureRepositories(services);

            ConfigureApplicationServices(services);

            ConfigureLogger(services);

            services.AddScoped<ICommentHelper, CommentHelper>();

            services
                .AddControllersWithViews(opts =>
                {
                    opts
                        .ModelBinderProviders
                        .Insert(0, new QueryModelBinderProvider());
                })
                .AddNewtonsoftJson(
                    options => options
                        .SerializerSettings
                        .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            if (_currentEnvironment.EnvironmentName == "Development")
            {
                services.AddDbContext<GameStoreContext>(i =>
                {
                    i.UseSqlServer(
                        _configurationString
                            .GetConnectionString(DevelopmentConnectionString));
                    i.EnableSensitiveDataLogging(true);
                });

                services.Configure<MongoSettings>(o =>
                {
                    o.ConnectionString = _configurationString.GetConnectionString(DevelopmentMongoConnectionString);
                    o.Name = _configurationString.GetSection(DevelopmentDatabaseNamePath).Value;
                    o.RewriteIds = Convert.ToBoolean(_configurationString.GetSection(CreateNewIdsPath).Value);
                });
            }
            else
            {
                services.AddDbContext<GameStoreContext>(i =>
                    i.UseSqlServer(
                        _configurationString
                            .GetConnectionString(DefaultConnectionString)));

                services.Configure<MongoSettings>(o =>
                {
                    o.ConnectionString = _configurationString.GetConnectionString(DefaultMongoConnectionString);
                    o.Name = _configurationString.GetSection(DefaultDatabaseNamePath).Value;
                    o.RewriteIds = Convert.ToBoolean(_configurationString.GetSection(CreateNewIdsPath).Value);
                });
            }

            services.AddMvc(option =>
            {
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

            if (Convert.ToBoolean(_configurationString.GetSection(CreateNewIdsPath).Value))
            {
                using IServiceScope serviceScope = app
                    .ApplicationServices
                    .GetService<IServiceScopeFactory>()
                    .CreateScope();

                var options = serviceScope
                    .ServiceProvider
                    .GetRequiredService<IOptions<MongoSettings>>();
                new MongoSeed(options).Initialize();
            }

            using (IServiceScope serviceScope = app
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

            app.UseStatusCodePagesWithReExecute("/error/{0}");

            app.UseRouting();

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

            services.AddScoped<IShipperService, ShipperService>();

            services.AddScoped<IPipeline, SelectionPipeline>();

            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPaymentContext, PaymentContext>();

            services.AddScoped(
                typeof(IEntityStateLogger<>),
                typeof(EntityStateLogger<>));
        }

        private void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped(
                typeof(IRepository<>),
                typeof(SqlDb.Repository<>));

            services.AddScoped<IGameRepositoryFacade, GameRepositoryFacade>();
            services.AddScoped<IGenreRepositoryFacade, GenreRepositoryFacade>();
            services.AddScoped<IPublisherRepositoryFacade, PublisherRepositoryFacade>();
            services.AddScoped<IOrderRepositoryFacade, OrderRepositoryFacade>();

            services.AddScoped<ICommentRepository, SqlDb.CommentRepository>();
            services.AddScoped<IOrderDetailRepository, SqlDb.OrderDetailRepository>();
            services.AddScoped<IPlatformRepository, SqlDb.PlatformRepository>();
            services.AddScoped<IViewRepository, SqlDb.ViewRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IMongoGameRepository, MongoDb.GameRepository>();
            services.AddScoped<IMongoGenreRepository, MongoDb.GenreRepository>();
            services.AddScoped<IMongoPublisherRepository, MongoDb.PublisherRepository>();
            services.AddScoped<IMongoOrderRepository, MongoDb.OrderRepository>();
            services.AddScoped<IMongoShipperRepository, MongoDb.ShipperRepository>();

            services.AddScoped<IGameRepository, SqlDb.GameRepository>();
            services.AddScoped<IGenreRepository, SqlDb.GenreRepository>();
            services.AddScoped<IPublisherRepository, SqlDb.PublisherRepository>();
            services.AddScoped<IOrderRepository, SqlDb.OrderRepository>();
        }

        private void ConfigureLogger(IServiceCollection services)
        {
            services
                .AddScoped(
                typeof(IAppLogger<>),
                typeof(AppLogger<>));

            services.AddScoped<LoggingFilter>();
        }

        private void ConfigureAutoMapper(IServiceCollection services)
        {
            IMapper mapper = AutoMapperConfiguration.Configure();

            services.AddSingleton(mapper);
        }
    }
}
