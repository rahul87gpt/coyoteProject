using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Coyote.Console.App.EntityFrameworkCore;
using Coyote.Console.App.Repository;
using Coyote.Console.App.Repository.IRepository;
using Coyote.Console.App.Repository.Repository;
using Coyote.Console.App.Services;
using Coyote.Console.App.Services.IServices;
using Coyote.Console.App.Services.Services;
using Coyote.Console.App.WebAPI.Configutation;
using Coyote.Console.App.WebAPI.Helper;
using Coyote.Console.App.WebAPI.Services;
using Coyote.Console.Common;
using Coyote.Console.Common.Services;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Coyote.Console.App.WebAPI
{
    /// <summary>
    /// Startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ConfigureServices
        /// </summary>
        /// <param name="services"></param>
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddControllers();
            services.AddDbContext<CoyoteAppDBContext>(option => option.UseSqlServer(Configuration.GetConnectionString(ConfigurationDataProvider.ConnectionStringName)));
            services.AddDevExpressControls();
            services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
            services.AddTransient<IWebDocumentViewerReportResolver, CustomWebDocumentViewerReportResolver>();
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddTransient(typeof(IAutoMappingServices), typeof(AutoMappingServices));
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient(typeof(ILoggerManager), typeof(LoggerManager));
            services.AddTransient(typeof(IUserRepository), typeof(UserRepository));
            services.AddTransient(typeof(IAPNServices), typeof(APNServices));
            services.AddTransient(typeof(ICashierServices), typeof(CashierService));
            services.AddTransient(typeof(ICommodityService), typeof(CommodityService));
            services.AddTransient(typeof(IDepartmentService), typeof(DepartmentService));
            services.AddTransient(typeof(IGLAccountServices), typeof(GLAccountServices));
            services.AddTransient(typeof(IKeypadServices), typeof(KeypadServices));
            services.AddTransient(typeof(ILoginService), typeof(LoginService));
            services.AddTransient(typeof(IMasterListService), typeof(MasterListService));
            services.AddTransient(typeof(IMasterListItemService), typeof(MasterListItemServices));
            services.AddTransient(typeof(IOutletProductService), typeof(OutletProductService));
            services.AddTransient(typeof(IOutlerSupplierServices), typeof(OutletSupplierServies));
            services.AddTransient(typeof(IPOSMessageServices), typeof(POSMessageServices));
            services.AddTransient(typeof(IPrintChangedLabelServices), typeof(PrintLabelChangedServices));
            services.AddTransient(typeof(IPrintLabelTypeServices), typeof(PrintLabelTypeServices));
            services.AddTransient(typeof(IProductService), typeof(ProductService));
            services.AddTransient(typeof(IRoleService), typeof(RoleService));
            services.AddTransient(typeof(IStoreGroupServices), typeof(StoreGroupServices));
            services.AddTransient(typeof(IStoreServices), typeof(StoreServices));
            services.AddTransient(typeof(ISupplierOrderScheduleServices), typeof(SupplierOrderScheduleServices));
            services.AddTransient(typeof(ISupplierProductService), typeof(SupplierProductService));
            services.AddTransient(typeof(ISupplierService), typeof(SupplierService));
            services.AddTransient(typeof(ITaxServices), typeof(TaxServices));
            services.AddTransient(typeof(ITillServices), typeof(TillServices));
            services.AddTransient(typeof(IUserServices), typeof(UserServices));
            services.AddTransient(typeof(IUserRoleService), typeof(UserRoleService));
            services.AddTransient(typeof(IWarehouseServices), typeof(WarehouseServices));
            services.AddTransient(typeof(IZoneOutletServices), typeof(ZoneOutletServices));
            services.AddTransient(typeof(ISendMailService), typeof(SendMailService));
            services.AddTransient(typeof(IEmailTemplateService), typeof(EmailTemplateService));
            services.AddTransient(typeof(IModuleActionsService), typeof(ModuleActionsService));
            services.AddTransient(typeof(IPromotionService), typeof(PromotionService));
            services.AddTransient(typeof(IImageUploadHelper), typeof(ImageUploadHelper));
            services.AddTransient(typeof(IOrdersService), typeof(OrdersService));
            services.AddTransient(typeof(IStockAdjustService), typeof(StockAdjustService));
            services.AddTransient(typeof(IStockTakeService), typeof(StockTakeService));
            services.AddTransient(typeof(IStockOnHandService), typeof(StockOnHandService));
            services.AddTransient(typeof(ICompetitionService), typeof(CompetitionService));
            services.AddTransient(typeof(IXeroAccountServices), typeof(XeroAccountServices));
            services.AddTransient(typeof(ISFTPService), typeof(SFTPService));
            services.AddTransient(typeof(IReportService), typeof(ReportService));
            services.AddTransient(typeof(IOptimalOrderServices), typeof(OptimalOrderServices));
            services.AddTransient(typeof(IMassPriceServices), typeof(MassPriceServices));
            services.AddTransient(typeof(IUserLoggerServices), typeof(UserLoggerServices));
            services.AddTransient(typeof(IManualSaleService), typeof(ManualSaleService));
            services.AddTransient(typeof(IRecipeServices), typeof(RecipeServices));
            services.AddTransient(typeof(IPathsService), typeof(PathsService));
            services.AddTransient(typeof(IMemberService), typeof(MemberService));
            services.AddTransient(typeof(IEPayService), typeof(EPayService));
            services.AddTransient(typeof(IRebateServices), typeof(RebateServices));
            services.AddTransient(typeof(IExportServices), typeof(ExportServices));
            services.AddTransient(typeof(IImportServices), typeof(ImportServices));
            services.AddTransient(typeof(IHostSettingsService), typeof(HostSettingsService));
            services.AddTransient(typeof(ICostPriceZonesService), typeof(CostPriceZonesService));
            services.AddTransient(typeof(ISystemControlsService), typeof(SystemControlsService));
            services.AddTransient(typeof(IHostProcessing), typeof(HostProcessingService));
            services.AddTransient(typeof(IReportSchedulerServices), typeof(ReportSchedulerServices));
            services.AddTransient(typeof(IHangfireJobs), typeof(HangfireJobs));
            services.AddTransient(typeof(ISchedulerService), typeof(SchedulerServices));
            services.AddTransient(typeof(IEDIMetcashService), typeof(EDIMetcashService));
            services.AddTransient(typeof(IAccessDepartmentServices), typeof(AccessDepartmentService));
            services.AddTransient(typeof(IHostUpdChangeService), typeof(HostUpdChangeService));
            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var lionOrderSettingsSection = Configuration.GetSection("LIONOrderSettings");
            services.Configure<LIONOrderSettings>(lionOrderSettingsSection);

            var ediEmailSettingsSection = Configuration.GetSection("EDIEmailSettings");
            services.Configure<EDIEmailSettings>(ediEmailSettingsSection);

            var sftpSettingsSection = Configuration.GetSection("SFTPSettings");
            services.Configure<SFTPSettings>(sftpSettingsSection);

            var CocaColaOrderSettingsSection = Configuration.GetSection("CocaColaOrderSettings");
            services.Configure<CocaColaOrderSettings>(CocaColaOrderSettingsSection);

            var distributorOrderSettingsSection = Configuration.GetSection("DistributorOrderSettings");
            services.Configure<DistributorOrderSettings>(distributorOrderSettingsSection);

            var reportsSettingsSection = Configuration.GetSection("FastReportSettings");
            services.Configure<FastReportSettings>(reportsSettingsSection);

            var fileDirectorySection = Configuration.GetSection("FileDirectorySettings");
            services.Configure<FileDirectorySettings>(fileDirectorySection);

            services.AddHangfire(config =>
               config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseDefaultTypeSerializer()
               .UseMemoryStorage()
               );

            services.AddHangfireServer();

            //Setting MultiPartBodyLength ,
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            //var key_jwt = Configuration.GetSection("Secret").Get<AppSettings>().Secret;
            var issuer = appSettings.Issuer;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    //ValidateLifetime = false, to show custom message on expired token
                    //options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                    };
                });
           
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Values Api", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                 Scheme = "OAuth2",
                                 Name = "Bearer",
                                 In = ParameterLocation.Header,
                            },
                          Array.Empty<string>()
                    }
                });

                c.IncludeXmlComments(GetXmlCommentsPath());

#pragma warning disable CS0618 // Type or member is obsolete
                c.DescribeAllEnumsAsStrings();
#pragma warning restore CS0618 // Type or member is obsolete

               
            });

            //add cors service
            services.AddCors(options =>
                {
                    options.AddPolicy("Cors", policy =>
                    {
                        policy.WithOrigins(Configuration.GetSection("AllowedUrls").Get<AllowedUrls>().CorsOrigins.Split(',')).
                        AllowAnyHeader().
                        AllowAnyMethod();
                    });
                });

            // configure DI for application services
            services.AddScoped<ILoginHelper, LoginHelper>();

            //Add framework services.
            services.AddMvc(options =>
            {
                options.Filters.Add(new StoreIdFilterAttribute());
                options.Filters.Add(typeof(StoreIdFilterAttribute));
            });
           // services.AddMvc().AddNewtonsoftJson();
            services.ConfigureReportingServices(configurator => {
                configurator.ConfigureReportDesigner(designerConfigurator => {
                    designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
                    designerConfigurator.RegisterObjectDataSourceWizardTypeProvider<ObjectDataSourceWizardCustomTypeProvider>();
                    designerConfigurator.RegisterObjectDataSourceConstructorFilterService<CustomObjectDataSourceConstructorFilterService>();
                });
                configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
                    viewerConfigurator.UseCachedReportSourceBuilder();
                });
            });
            //Customize Model state Errors 
            services.Configure<ApiBehaviorOptions>(o =>
            {
                o.InvalidModelStateResponseFactory = actionContext =>
                  {
                      var problems = new CustomModelStateError(actionContext);
                      return new BadRequestObjectResult(problems);
                  };
            });
            
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(60);
            });
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="app"></param>
       /// <param name="env"></param>
       /// <param name="logger"></param>
       /// <param name="serviceProvider"></param>
       /// <param name="loggerFactory"></param>
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)//, IBackgroundJobClient backgroundJobClient
        {
            var reportingLogger = loggerFactory.CreateLogger("DXReporting");
            DevExpress.XtraReports.Web.ClientControls.LoggerService.Initialize((exception, message) => {
                var logMessage = $"[{DateTime.Now}]: Exception occurred. Message: '{message}'. Exception Details:\r\n{exception}";
                reportingLogger.LogError(logMessage);
            });
            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;
            app.UseDevExpressControls();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseExceptionConfigure(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("Cors");
            app.UseFastReport();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
            app.UseHangfireDashboard();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration.GetSection("SwaggerEndpoint").Get<SwaggerEndpoint>().Endpoint, "API V1");
                c.DocumentTitle = "Coyote";
                c.DocExpansion(DocExpansion.None);
            });

         //   backgroundJobClient.Enqueue(() => Debug.WriteLine("Hello Hanfire job!"));
          serviceProvider.GetService<IHangfireJobs>().CallScheduler();

        //    Enable to test scheduler working
        //  serviceProvider.GetService<IHangfireJobs>().CallTestScheduler();
        }

        private static string GetXmlCommentsPath()
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, xmlFile);
        }
    }
}
