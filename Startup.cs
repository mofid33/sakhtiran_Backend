using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using MarketPlace.API.Data.Models;
using MarketPlace.API.Data.Repositories.IRepository;
using MarketPlace.API.Data.Repositories.Repository;
using MarketPlace.API.FirebaseServices;
using MarketPlace.API.Helper;
using MarketPlace.API.Services.IService;
using MarketPlace.API.Services.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace MarketPlace.API
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

            //services.AddHttpsRedirection(options =>
            //{
            //   options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
            //   options.HttpsPort = 443;
            //});

            services.AddDbContext<MarketPlaceDbContext>(options => options
            //.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionlocal")));
            .UseSqlServer(Configuration.GetConnectionString("Connection")));

            services.AddMvc(option => option.EnableEndpointRouting = false).AddNewtonsoftJson(options => { options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; });
            services.AddAutoMapper(typeof(Startup));
            services.AddCors();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Optimal;
                });
            services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                    options.Providers.Add<GzipCompressionProvider>();
                });
            services.AddControllers();

            services.AddRazorPages()
            .AddNewtonsoftJson();

            services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            //.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnectionlocal"), new SqlServerStorageOptions
                        .UseSqlServerStorage(Configuration.GetConnectionString("Connection"), new SqlServerStorageOptions

                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            }));

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.AddMvc();

            // services.AddSwaggerGen(c =>
            // {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            //     c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //     {
            //         Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
            //         Name = "Authorization",
            //         In = ParameterLocation.Header,
            //         Type = SecuritySchemeType.ApiKey,
            //         Scheme = "Bearer"
            //     });

            //     c.AddSecurityRequirement(new OpenApiSecurityRequirement
            //     {
            //         {
            //             new OpenApiSecurityScheme
            //             {
            //                 Reference = new OpenApiReference
            //                 {
            //                     Type = ReferenceType.SecurityScheme,
            //                     Id = "Bearer"
            //                 }
            //             },
            //             Array.Empty<string>()
            //         }
            //     });
            // });



            // Add the processing server as IHostedService
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            //services
            services.AddScoped<IGuaranteeService, GuaranteeService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IFileUploadService, FileUploadService>();
            services.AddScoped<IMessageLanguageService, MessageLanguageService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IVariationService, VariationService>();
            services.AddScoped<ISpecificationGroupService, SpecificationGroupService>();
            services.AddScoped<ISpecificationService, SpecificationService>();
            services.AddScoped<IReturningReasonService, ReturningReasonService>();
            services.AddScoped<IReturningTypeService, ReturningTypeService>();
            services.AddScoped<IMeasurementUnitService, MeasurementUnitService>();
            services.AddScoped<IGoodsService, GoodsService>();
            services.AddScoped<ILanguageCurrencyService, LanguageCurrencyService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IWebSliderService, WebSliderService>();
            services.AddScoped<IWebModuleService, WebModuleService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IFromService, FromService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IShopService, ShopService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            services.AddScoped<IAccountingService, AccountingService>();
            services.AddScoped<IWareHouseService, WareHouseService>();
            services.AddScoped<IUserActivityService, UserActivityService>();
            services.AddScoped<IGoodsSurveyQuestionsService, GoodsSurveyQuestionsService>();
            services.AddScoped<IShopSurveyQuestionsService, ShopSurveyQuestionsService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IOrderCancelingReasonService, OrderCancelingReasonService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IUserOrderService, UserOrderService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IProductStatisticsService, ProductStatisticsService>();
            services.AddScoped<IHelpService, HelpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRecommendationService, RecommendationService>();
            services.AddScoped<MarketPlace.API.Services.IService.IBackgroundService, MarketPlace.API.Services.Service.BackgroundService>();
            services.AddScoped<INotificationSettingService, NotificationSettingService>();
            services.AddScoped<IMessagingService, MessagingService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IOrderReturnService, OrderReturnService>();
            services.AddScoped<IPupupService, PupupService>();


            //repository
            services.AddScoped<IGuaranteeRepository, GuaranteeRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IProvinceRepository, ProvinceRepository>();
            services.AddScoped<IVariationRepository, VariationRepository>();
            services.AddScoped<ISpecificationGroupRepository, SpecificationGroupRepository>();
            services.AddScoped<ISpecificationRepository, SpecificationRepository>();
            services.AddScoped<IReturningReasonRepository, ReturningReasonRepository>();
            services.AddScoped<IReturningTypeRepository, ReturningTypeRepository>();
            services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
            services.AddScoped<IGoodsRepository, GoodsRepository>();
            services.AddScoped<ILanguageCurrencyRepository, LanguageCurrencyRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IWebSliderRepository, WebSliderRepository>();
            services.AddScoped<IWebModuleRepository, WebModuleRepository>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IDiscountRepository, DiscountRepository>();
            services.AddScoped<IGoodsCommentRepository, GoodsCommentRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
            services.AddScoped<IAccountingRepository, AccountingRepository>();
            services.AddScoped<IWareHouseRepository, WareHouseRepository>();
            services.AddScoped<IUserActivityRepository, UserActivityRepository>();
            services.AddScoped<IGoodsSurveyQuestionsRepository, GoodsSurveyQuestionsRepository>();
            services.AddScoped<IShopSurveyQuestionsRepository, ShopSurveyQuestionsRepository>();
            services.AddScoped<IUserOrderRepository, UserOrderRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IOrderCancelingReasonRepository, OrderCancelingReasonRepository>();
            services.AddScoped<IHomeRepository, HomeRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IProductStatisticsRepository, ProductStatisticsRepository>();
            services.AddScoped<IHelpRepository, HelpRepository>();
            services.AddScoped<IRecommendationRepository, RecommendationRepository>();
            services.AddScoped<IVerificationRepository, VerificationRepository>();
            services.AddScoped<INotificationSettingRepository, NotificationSettingRepository>();
            services.AddScoped<IOrderReturnRepository, OrderReturnRepository>();
            services.AddScoped<IForceUpdateRepository, ForceUpdateRepository>();
            services.AddScoped<IPupupRepository, PupupRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
                app.UseHsts();
            }


            //app.UseHttpsRedirection();
      

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/api") && context.Request.Method.ToLower() != "options", appBuilder =>
          {
              appBuilder.UseMyMiddleware();
          });


            //app.UseSwagger(c =>
            //{
            //    c.SerializeAsV2 = true;
            //});
            //app.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            //    c.RoutePrefix = string.Empty;
            //});

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                  ForwardedHeaders.XForwardedProto
            });


            app.UseAuthentication();
            app.UseCors(builder => builder
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .SetIsOriginAllowed((host) => true)
                            .AllowCredentials()
                        );
            app.UseDefaultFiles();
            FileExtensionContentTypeProvider contentTypes = new FileExtensionContentTypeProvider();
            contentTypes.Mappings[".apk"] = "application/vnd.android.package-archive";
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = contentTypes,
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Uploads")),
                RequestPath = new PathString("/Uploads")
            });
            app.UseStaticFiles(new StaticFileOptions()
            {
                ContentTypeProvider = contentTypes,
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"emailTemplate")),
                RequestPath = new PathString("/emailTemplate")
            });
            app.UseResponseCompression();
            app.UseMvc();




            app.UseRouting();

            app.UseHangfireDashboard();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "Payment",
                    pattern: "{controller=Payment}/{action=Index}/{id?}");
            });

            RecurringJob.AddOrUpdate<IBackgroundService>("SetAndUnsetDiscountTime1", service => service.SetAndUnsetDiscount(), "30 2 * * *");
            RecurringJob.AddOrUpdate<IBackgroundService>("CheckAbandonedShoppingCarts", service => service.CheckAbandonedShoppingCarts(), "10 1 * * *");
            RecurringJob.AddOrUpdate<IBackgroundService>("CheckShopStatus", service => service.CheckShopStatus(), "20 3 * * *");
            app.UseSpa(spa =>
               {
                   spa.Options.SourcePath = "ClientApp";
                   spa.UseProxyToSpaDevelopmentServer("http://localhost:3005");
               });
        }
    }
}
//dotnet ef dbcontext scaffold "data source=.;initial catalog=MarketPlaceDb;user id=sa;password=102030;MultipleActiveResultSets=True;App=EntityFramework" Microsoft.EntityFrameworkCore.SqlServer -o Data/Models
//dotnet ef dbcontext scaffold "data source=192.168.1.200;initial catalog=MarketPlaceDb;user id=sa;password=33257600TPDco.ir;MultipleActiveResultSets=True;App=EntityFramework" Microsoft.EntityFrameworkCore.SqlServer -o Data/Models

//         private readonly IHttpContextAccessor _httpContextAccessor;

//         string header = _httpContextAccessor.HttpContext.Request.Headers["X-Custom-Header"];

//  modelBuilder.HasDbFunction(typeof(JsonExtensions).GetMethod(nameof(JsonExtensions.JsonValue)))
//                  .HasTranslation(e => SqlFunctionExpression.Create(
//                      "JSON_VALUE", e, typeof(string), null));


// personal ajyal account

// sb-i47hou3694187@personal.example.com
// Ajyal4044465
