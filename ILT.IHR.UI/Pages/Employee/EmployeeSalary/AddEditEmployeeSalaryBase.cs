using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ILT.IHR.UI.Service;
using ILT.IHR.DTO;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace ILT.IHR.UI.Pages.Employee.EmployeeSalary
{
    public class AddEditEmployeeSalaryBase: ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service    
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service
        [Inject]
        public ISalaryService SalaryService { get; set; } //Service
        protected string Title = "Add";
        public bool ShowDialog { get; set; }
        public bool ShowChildDialog { get; set; }
        public string subClient { get; set; }
        private int SalaryId { get; set; }
        private int SalaryRateId { get; set; }
        [Parameter]
        public List<ListValue> PaymentTypeList { get; set; } 
        public List<ListValue> WeekEndingDayList { get; set; }
        public IEnumerable<ILT.IHR.DTO.Company> CompanyList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Company> ClientList { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.Company> VendorList { get; set; }  // Table APi Data
        [Inject]
        public IUserService UserService { get; set; } //Service
        public IEnumerable<ILT.IHR.DTO.User> UsersList { get; set; }  // Table APi Data
        public IEnumerable<ILT.IHR.DTO.User> timesheetApproverList { get; set; }  // Table APi Data
        public List<ListValue> TitleList { get; set; }
        public List<EndClient> endClientList { get; set; }
        public List<Country> CountryList { get; set; }
        public List<State> StateList { get; set; }
        public string country { get; set; }
        public int countryId { get; set; }
        public List<SubClient> subClients { get; set; }
        public string selectedClient { get; set; }
        public string selectedEndClient { get; set; }
        public class SubClient
        {
            public string Text { get; set; }

            public bool isValidSubClient { get; set; } = false;
        }
        [Parameter]
        public EventCallback<bool> UpdateSalarys { get; set; }

        public Salary salary = new Salary();
        [Inject]
        public ILookupService LookupService { get; set; } //Service
        [Parameter]
        public int EmployeeId { get; set; }
        [Parameter]
        public string EmployeeName { get; set; }
        public decimal SalaryBasic { get; set; }
        public decimal SalaryHRA { get; set; }
        public decimal SalaryLTA { get; set; }
        public decimal SalaryEducationAllowance { get; set; }
        public decimal Salarybonus { get; set; }
        public decimal SalarySpecialAllowance { get; set; }
        public decimal SalaryVariablePay { get; set; }
        public decimal SalaryMedicalInsurance { get; set; }
        public decimal SalaryConveyance { get; set; }
        public decimal SalaryMealAllowance { get; set; }
        public decimal SalaryMedicalAllowance { get; set; }
        public decimal SalaryTelephoneAllowance { get; set; }
        public decimal SalaryGratuity { get; set; }
        public decimal TotalEmployeeGrossWagePM { get; set; }
        public decimal TotalEmployeeGrossWagePA { get; set; }
        public string TotalCTCPM { get; set; }
        public List<Salary> Salaries { get; set; }  // Table APi Data

        public ILT.IHR.DTO.User user;
        public List<RolePermission> RolePermissions;
        public RolePermission SalaryRolePermission;
        protected bool isShow { get; set; }
        public bool isSaveButtonDisabled { get; set; } = false;
        public string ErrorMessage;


        protected override async Task OnInitializedAsync()
        {

            subClients = new List<SubClient>();
            subClient = "";
            RolePermissions = await sessionStorage.GetItemAsync<List<RolePermission>>(SessionConstants.ROLEPERMISSION);
            SalaryRolePermission = RolePermissions.Find(usr => usr.ModuleShort == Constants.SALARY);
            user = await sessionStorage.GetItemAsync<ILT.IHR.DTO.User>("User");
           // await JSRuntime.InvokeVoidAsync("DecimalPonints");
        }
        [Inject] IJSRuntime JSRuntime { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
           
        }

        public void Show(int Id)
        {
            SalaryId = Id;
            ResetDialog();
            loadEmployeeData(EmployeeId);
            if (SalaryId != 0)
            {
                isShow = SalaryRolePermission.Update;
                GetDetails(SalaryId);
            }
            else
            {
                isShow = SalaryRolePermission.Add;
                Title = "Add";
                subClients = new List<SubClient>();
                selectedClient = "";
                selectedEndClient = "";
                ShowChildDialog = false;
                salary.StartDate = DateTime.Now;
                salary.HRA = "0";
                salary.LTA = "0";
                salary.Bonus = "0";
                salary.EducationAllowance = "0";
                salary.VariablePay = "0";
                salary.SpecialAllowance = "0";
                salary.ProvidentFund = "0";
                salary.CostToCompany = "0";
                salary.Conveyance = "0";
                salary.MealAllowance = "0";
                salary.MedicalAllowance = "0";
                salary.MedicalInsurance = "0";
                salary.TelephoneAllowance = "0";
                salary.Gratuity = "0";
                SalaryBasic = 0;
                Salarybonus = 0;
                SalaryEducationAllowance = 0;
                SalarySpecialAllowance = 0;
                SalaryLTA = 0;
                SalaryVariablePay = 0;
                SalaryMedicalInsurance = 0;
                SalaryHRA = 0;
                SalaryConveyance = 0;
                SalaryMealAllowance = 0;
                SalaryMedicalAllowance = 0;
                SalaryTelephoneAllowance = 0;
                SalaryGratuity = 0;
                ProvidentFund = 0;
                ShowDialog = true;
                StateHasChanged();
            }
        }

        protected async Task loadEmployeeData(int employeeId)
        {
            if (employeeId != 0)
            {
                Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                resp = await EmployeeService.GetEmployeeByIdAsync(employeeId) as Response<ILT.IHR.DTO.Employee>;
                if (resp.MessageType == MessageType.Success)
                {
                    if (resp.Data.EmployeeID != 0)
                    {
                        Salaries = resp.Data.Salaries;
                        //salary.Country = resp.Data.Country;
                    }
                }
                StateHasChanged();
            }
        }


      
       
        private async Task GetDetails(int Id)
        {
            selectedClient = "";
            selectedEndClient = "";
            subClients = new List<SubClient>();
            Response<Salary> resp = new Response<Salary>();
            resp = await SalaryService.GetSalaryById(Id) as Response<Salary>;
            if (resp.MessageType == MessageType.Success)
            salary = resp.Data;
            Title = "Edit";
            ShowDialog = true;
            ShowChildDialog = false;
            setValueCTCPMPA();
            StateHasChanged();
        }
        protected async Task SalaryValidation()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            ErrorMessage = "";
            if(salary.EndDate != null)
            {
                if (salary.StartDate <= salary.EndDate)
                {
                    SalarySave();
                } else {
                    ErrorMessage = "End date must be greater than start date";
                  //  toastService.ShowError("End date must be greater than start date", "");
                    salary.EndDate = null;
                    this.isSaveButtonDisabled = false;
                }
            }  else
            {
                SalarySave();
            }
        }
        protected async Task SalarySave()
        {
            if (Salaries.Count == 0)
            {
                await SaveSalary();
            }
            else
            {
                var currentRec = Salaries.Where(x => x.SalaryID == SalaryId).FirstOrDefault();
                if (currentRec == null || currentRec.EndDate != null)
                {
                    if(salary.EndDate == null)
                    {
                        salary.EndDate = Convert.ToDateTime("2999-12-31");
                    }
                    bool salaryAlreadyexist = Salaries.FindIndex(x => ((x.StartDate <= salary.StartDate && x.EndDate == null) ||
                                          (x.StartDate <= salary.EndDate && salary.StartDate <= x.EndDate) ||
                                          (x.StartDate >= salary.StartDate && x.EndDate <= salary.EndDate) ||
                                          (x.StartDate >= salary.StartDate && x.EndDate == null) ||
                                          (x.StartDate <= salary.StartDate && x.EndDate >= salary.StartDate))) > -1;
                    if (salaryAlreadyexist)
                    {
                        ErrorMessage = "Salary already exists for this period";
                        if(salary.EndDate == Convert.ToDateTime("2999-12-31"))
                        {
                            salary.EndDate = null;
                        }
                    }
                    else
                    {
                        if (salary.EndDate == Convert.ToDateTime("2999-12-31"))
                        {
                            salary.EndDate = null;
                        }
                        await SaveSalary();
                    }
                }
                else
                {

                    await SaveSalary();
                }
            }
            isSaveButtonDisabled = false;
        }



        protected async Task SaveSalary()
        {
            salary.EmployeeName = EmployeeName;
            if (SalaryId == 0)
            {
                salary.CreatedBy = user.FirstName + " " + user.LastName;
                salary.EmployeeID = EmployeeId;
                var result = await SalaryService.SaveSalary(salary);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Salary saved successfully", "");
                    UpdateSalarys.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                    Cancel();
                }
            }
            else if (SalaryId != 0)
            {
                salary.ModifiedBy = user.FirstName + " " + user.LastName;
                var result = await SalaryService.UpdateSalary(SalaryId, salary);
                if (result.MessageType == MessageType.Success)
                {
                    toastService.ShowSuccess("Salary saved successfully", "");
                    UpdateSalarys.InvokeAsync(true);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
            }
            //isSaveButtonDisabled = false;
        }
        public void Cancel()
        {
            ShowDialog = false;
            ShowChildDialog = false;
            StateHasChanged();
        }
        public void Close()
        {   
            ShowDialog = false;
            ShowChildDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            ErrorMessage = null;
            salary = new Salary { };
        }

        protected async Task setValueCTCPMPA()
       {
            StateHasChanged();
            GetTotalCostToCompanyPM();
            GettotalCostToCompanyPA();
            StateHasChanged();
        }

        public void OnSalaryChangeBasicPayOnInput(ChangeEventArgs e)
        {
            var value = salary.BasicPay;

        }
        protected decimal OnSalaryBasicPay()
        {
            var BasicPay = salary.BasicPay != "" ? Convert.ToDecimal(salary.BasicPay) : 0;
            if (BasicPay != 0)
            {
                return  Convert.ToDecimal(string.Format("{0:0.00}", BasicPay));
               
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeBasicPay()
        {
            var BasicPay = salary.BasicPay != "" ? Convert.ToDecimal(salary.BasicPay) : 0;
            if(BasicPay != 0)
            {
                 SalaryBasic = Convert.ToDecimal(string.Format("{0:0.00}", BasicPay * 12));
                return SalaryBasic;
            } else
            {
                SalaryBasic = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryHRA()
        {
            var Hra = salary.HRA != "" ? Convert.ToDecimal(salary.HRA) : 0;
            if (Hra != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", Hra));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal OnSalaryChangeHRA()
        {
            var Hra = salary.HRA != "" ? Convert.ToDecimal(salary.HRA) : 0;
            if (Hra != 0)
            {
                SalaryHRA = Convert.ToDecimal(string.Format("{0:0.00}", Hra * 12));
                return SalaryHRA;
            }
            else
            {
                SalaryHRA = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryLTA()
        {
            var Lta = salary.LTA != "" ? Convert.ToDecimal(salary.LTA) : 0;
            if (Lta != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", Lta));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeLTA()
        {
            var Lta = salary.LTA != "" ? Convert.ToDecimal(salary.LTA) : 0;
            if (Lta != 0)
            {
                SalaryLTA = Convert.ToDecimal(string.Format("{0:0.00}", Lta * 12));
                return Decimal.Parse(SalaryLTA.ToString("0.00")); ;
            }
            else
            {
                SalaryLTA = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryEducationAllowance()
        {
            var educationAllowance = salary.EducationAllowance != "" ? Convert.ToDecimal(salary.EducationAllowance) : 0; ;
            if (educationAllowance != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", educationAllowance));
                
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeEducationAllowance()
        {
            var educationAllowance = salary.EducationAllowance != "" ? Convert.ToDecimal(salary.EducationAllowance) : 0;
            if (educationAllowance != 0)
            {
                SalaryEducationAllowance = Convert.ToDecimal(string.Format("{0:0.00}", educationAllowance * 12));
                return SalaryEducationAllowance;
            }
            else
            {
                SalaryEducationAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0)); 
            }
        }
        protected decimal OnSalarySpecialAllowance()
        {
            var specialAllowance = salary.SpecialAllowance != "" ? Convert.ToDecimal(salary.SpecialAllowance) : 0;
            if (specialAllowance != 0)
            {
                return  Convert.ToDecimal(string.Format("{0:0.00}", specialAllowance));
                
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeSpecialAllowance()
        {
            var specialAllowance = salary.SpecialAllowance != "" ? Convert.ToDecimal(salary.SpecialAllowance) : 0;
            if (specialAllowance != 0)
            {
                SalarySpecialAllowance = Convert.ToDecimal(string.Format("{0:0.00}", specialAllowance * 12));
                return SalarySpecialAllowance;
            }
            else
            {
                SalarySpecialAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0)); 
            }
        }
        protected decimal OnSalaryBonus()
        {
            var bonus = salary.Bonus !="" ? Convert.ToDecimal(salary.Bonus) : 0;
            if (bonus != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", bonus));
                
            } else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0)); ;
            }
        }
        protected decimal OnSalaryChangeBonus()
        {
            var bonus = salary.Bonus != "" ? Convert.ToDecimal(salary.Bonus) : 0;
            if (bonus != 0)
            {
                Salarybonus = Convert.ToDecimal(string.Format("{0:0.00}", bonus * 12));
                return Salarybonus;
            }
            else
            {
                Salarybonus = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0)); ;
            }
        }

        protected decimal OnSalaryChangeMedicalInsurance()
        {
            var medicalInsurance = salary.MedicalInsurance != "" ? Convert.ToDecimal(salary.MedicalInsurance) : 0;
            if (medicalInsurance != 0)
            {
                SalaryMedicalInsurance = Convert.ToDecimal(string.Format("{0:0.00}", medicalInsurance));
                return SalaryMedicalInsurance;
            }
            else
            {
                SalaryMedicalInsurance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal OnSalaryChangeVariablePay()
        {
            var variablePay = salary.VariablePay != "" ? Convert.ToDecimal(salary.VariablePay) : 0;
            if (variablePay != 0)
            {
                SalaryVariablePay = Convert.ToDecimal(string.Format("{0:0.00}", variablePay));
                return SalaryVariablePay;
            }
            else
            {
                SalaryVariablePay = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryMealAllowance()
        {
            var mealAllowance = salary.MealAllowance != "" ? Convert.ToDecimal(salary.MealAllowance) : 0;
            if (mealAllowance != 0)
            {
                return  Convert.ToDecimal(string.Format("{0:0.00}", mealAllowance));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal OnSalaryChangeMealAllowance()
        {
            var mealAllowance = salary.MealAllowance != "" ? Convert.ToDecimal(salary.MealAllowance) : 0;
            if (mealAllowance != 0)
            {
                SalaryMealAllowance = Convert.ToDecimal(string.Format("{0:0.00}", mealAllowance * 12));
                return SalaryMealAllowance;
            }
            else
            {
                SalaryMealAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryTelephoneAllowance()
        {
            var telephoneAllowance = salary.TelephoneAllowance  != "" ? Convert.ToDecimal(salary.TelephoneAllowance) : 0;
            if (telephoneAllowance != 0)
            {
               return Convert.ToDecimal(string.Format("{0:0.00}", telephoneAllowance));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeTelephoneAllowance()
        {
            var telephoneAllowance = salary.TelephoneAllowance != "" ? Convert.ToDecimal(salary.TelephoneAllowance) : 0;
            if (telephoneAllowance != 0)
            {
                SalaryTelephoneAllowance = Convert.ToDecimal(string.Format("{0:0.00}", telephoneAllowance * 12));
                return SalaryTelephoneAllowance;
            }
            else
            {
                SalaryTelephoneAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryConveyance()
        {
            var conveyance = salary.Conveyance != "" ? Convert.ToDecimal(salary.Conveyance) : 0;
            if (conveyance != 0)
            {
                SalaryConveyance = Convert.ToDecimal(string.Format("{0:0.00}", conveyance));
                return SalaryConveyance;
            }
            else
            {
                SalaryConveyance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeConveyance()
        {
            var conveyance = salary.Conveyance != "" ? Convert.ToDecimal(salary.Conveyance) : 0;
            if (conveyance != 0)
            {
                SalaryConveyance = Convert.ToDecimal(string.Format("{0:0.00}", conveyance * 12));
                return SalaryConveyance;
            }
            else
            {
                SalaryConveyance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal OnSalaryMedicalAllowance()
        {
            var MedicalAllowance = salary.MedicalAllowance != "" ? Convert.ToDecimal(salary.MedicalAllowance) : 0;
            if (MedicalAllowance != 0)
            {
                SalaryMedicalAllowance = Convert.ToDecimal(string.Format("{0:0.00}", MedicalAllowance));
                return SalaryMedicalAllowance;
            }
            else
            {
                SalaryMedicalAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal OnSalaryChangeMedicalAllowance()
        {
            var medicalAllowance = salary.MedicalAllowance != "" ? Convert.ToDecimal(salary.MedicalAllowance) : 0;
            if (medicalAllowance != 0)
            {
                SalaryMedicalAllowance = Convert.ToDecimal(string.Format("{0:0.00}", medicalAllowance * 12));
                return SalaryMedicalAllowance;
            }
            else
            {
                SalaryMedicalAllowance = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
         
        protected  decimal GetEmployeeTotalGrossWagePM()
        {
            var BasicPay = salary.BasicPay != "" ? Convert.ToDecimal(salary.BasicPay) : 0;
            var HRA = salary.HRA != "" ? Convert.ToDecimal(salary.HRA) : 0;
            var LTA = salary.LTA != "" ? Convert.ToDecimal(salary.LTA) : 0;
            var EducationAllowance = salary.EducationAllowance != "" ? Convert.ToDecimal(salary.EducationAllowance) : 0;
            var SpecialAllowance = salary.SpecialAllowance != "" ? Convert.ToDecimal(salary.SpecialAllowance) : 0;
            var Bonus = salary.Bonus != "" ? Convert.ToDecimal(salary.Bonus) : 0;
            var MealAllowance = salary.MealAllowance != "" ? Convert.ToDecimal(salary.MealAllowance) : 0;
            var TelephoneAllowance = salary.TelephoneAllowance != "" ? Convert.ToDecimal(salary.TelephoneAllowance) : 0;
            var Conveyance = salary.Conveyance != "" ? Convert.ToDecimal(salary.Conveyance) : 0;
            var MedicalAllowance = salary.MedicalAllowance != "" ? Convert.ToDecimal(salary.MedicalAllowance) : 0;
            var TotalEmployeeGrossWagePM = BasicPay  + HRA  + LTA + EducationAllowance + SpecialAllowance + Bonus + MealAllowance + TelephoneAllowance + Conveyance + MedicalAllowance;
            return Convert.ToDecimal(string.Format("{0:0.00}", TotalEmployeeGrossWagePM));
        }

        protected decimal GetEmployeeTotalGrossWagePA()
        {

            var TotalEmployeeGrossWagePA = SalaryBasic + SalaryHRA + SalaryLTA + SalaryEducationAllowance + SalarySpecialAllowance + Salarybonus + SalaryVariablePay + SalaryMealAllowance + SalaryTelephoneAllowance + SalaryConveyance + SalaryMedicalAllowance + SalaryMedicalInsurance;
            return Convert.ToDecimal(string.Format("{0:0.00}", TotalEmployeeGrossWagePA));
        }
        public decimal ProvidentFund { get; set; }
        protected decimal OnSalaryProvidentFund()
        {
            var providentFund = salary.ProvidentFund != "" ? Convert.ToDecimal(salary.ProvidentFund) : 0;
            if (providentFund != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", providentFund));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeProvidentFund()
        {
            var providentFund = salary.ProvidentFund != "" ? Convert.ToDecimal(salary.ProvidentFund) : 0;
            if (providentFund != 0)
            {
                ProvidentFund = Convert.ToDecimal(string.Format("{0:0.00}", providentFund * 12));
                return ProvidentFund;
            }
            else
            {
                ProvidentFund = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}",0));
            }
        }
        protected decimal OnSalaryGratuity()
        {
            var gratuity = salary.Gratuity != "" ? Convert.ToDecimal(salary.Gratuity) : 0;
            if (gratuity != 0)
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", gratuity));
            }
            else
            {
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }
        protected decimal OnSalaryChangeGratuity()
        {
            var gratuity = salary.Gratuity != "" ? Convert.ToDecimal(salary.Gratuity) : 0;
            if (gratuity != 0)
            {
                SalaryGratuity = Convert.ToDecimal(string.Format("{0:0.00}", gratuity * 12));
                return SalaryGratuity;
            }
            else
            {
                SalaryGratuity = 0;
                return Convert.ToDecimal(string.Format("{0:0.00}", 0));
            }
        }

        protected decimal GetTotalCostToCompanyPM()
        {
            var BasicPay = salary.BasicPay != "" ? Convert.ToDecimal(salary.BasicPay) : 0;
            var HRA = salary.HRA != "" ? Convert.ToDecimal(salary.HRA) : 0;
            var LTA = salary.LTA != "" ? Convert.ToDecimal(salary.LTA) : 0;
            var EducationAllowance = salary.EducationAllowance != "" ? Convert.ToDecimal(salary.EducationAllowance) : 0;
            var SpecialAllowance = salary.SpecialAllowance != "" ? Convert.ToDecimal(salary.SpecialAllowance) : 0;
            var Bonus = salary.Bonus != "" ? Convert.ToDecimal(salary.Bonus) : 0;
            var MealAllowance = salary.MealAllowance != "" ? Convert.ToDecimal(salary.MealAllowance) : 0;
            var TelephoneAllowance = salary.TelephoneAllowance != "" ? Convert.ToDecimal(salary.TelephoneAllowance) : 0;
            var Conveyance = salary.Conveyance != "" ? Convert.ToDecimal(salary.Conveyance) : 0;
            var MedicalAllowance = salary.MedicalAllowance != "" ? Convert.ToDecimal(salary.MedicalAllowance) : 0;
            var ProvidentFund = salary.ProvidentFund != "" ? Convert.ToDecimal(salary.ProvidentFund) : 0;
            var Gratuity = salary.Gratuity != "" ? Convert.ToDecimal(salary.Gratuity) : 0;
            var totalCostToCompanyPM = BasicPay + HRA + LTA + EducationAllowance + SpecialAllowance + Bonus + MealAllowance + TelephoneAllowance + Conveyance + MedicalAllowance + ProvidentFund + Gratuity;
            TotalCTCPM = totalCostToCompanyPM.ToString();
            return Convert.ToDecimal(string.Format("{0:0.00}", totalCostToCompanyPM));
        }

        protected decimal GettotalCostToCompanyPA()
        {
            var totalCostToCompanyPA = SalaryBasic + SalaryHRA + SalaryLTA + SalaryEducationAllowance + SalarySpecialAllowance + Salarybonus + SalaryVariablePay + SalaryMealAllowance + SalaryTelephoneAllowance + SalaryConveyance + SalaryMedicalAllowance + ProvidentFund + SalaryGratuity + SalaryMedicalInsurance;
            salary.CostToCompany = totalCostToCompanyPA.ToString();
            return Convert.ToDecimal(string.Format("{0:0.00}", totalCostToCompanyPA));
        }

      
    }
}
