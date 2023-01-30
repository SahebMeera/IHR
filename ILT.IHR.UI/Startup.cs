using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ILT.IHR.UI.Data;
using ILT.IHR.UI.Service;
using System.Net.Http;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.SessionStorage;
using Blazored.Toast;
using Microsoft.JSInterop;
using BlazorDownloadFile;
//using ILT.IHR.WebAPI.ErrorHandler;

namespace ILT.IHR.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddServerSideBlazor().AddCircuitOptions(o =>
            //{
            //    // if (env.IsDevelopment()) //only add details when debugging
            //    // {
            //        o.DetailedErrors = true;
            //    // }
            //});

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddControllers();
            services.AddSingleton<WeatherForecastService>();
            services.AddScoped<DataProvider>();
            services.AddBlazoredSessionStorage();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddSingleton<HttpClient>();
            services.AddBlazoredToast();
            services.AddBlazorContextMenu();
            services.AddBlazorDownloadFile();
            services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = 1048576;
            });
            services.AddHttpClient<IUserService, UserService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ILookupService, LookupService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ICompanyService, CompanyService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEmployeeChangesetService, EmployeeChangesetService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IRoleService, RoleService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IDependentService, DependentService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IAssignmentService, AssignmentService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IContactService, ContactService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IDirectDepositService, DirectDepositService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ICountryService, CountryService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ILeaveBalanceService, LeaveBalanceService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ILeaveService, LeaveService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IHolidayService, HolidayService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ITimeSheetService, TimeSheetService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ITimeEntryService, TimeEntryService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<INotificationService, NotificationService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ICommonService, CommonService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ILeaveAccrualService, LeaveAccrualService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEmailApprovalService, EmailApprovalService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEndClientService, EndClientService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IExpenseService, ExpenseService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEmployeeW4Service, EmployeeW4Service>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IFormI9Service, FormI9Service>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IWorkFromHomeService, WorkFromHomeService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IWizardService, WizardService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IWizardDataService, WizardDataService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IAssetService, AssetService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ITicketService, TicketService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IAssetChangesetService, AssetChangesetService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IAppraisalService, AppraisalService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<ISalaryService, SalaryService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            }); 
            services.AddHttpClient<IAuditLogService, AuditLogService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<II9DocumentService, I9DocumentService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IFormI9ChangesetService, FormI9ChangesetService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IUserNotificationService, UserNotificationService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
            services.AddHttpClient<IEmployeeSkillService, EmployeeSkillService>(client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiUrl"]);
            });
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
