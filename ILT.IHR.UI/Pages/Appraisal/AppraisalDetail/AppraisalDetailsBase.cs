using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ILT.IHR.DTO;
using ILT.IHR.UI.Service;
using System.Linq;
using Blazored.Toast.Services;
using Blazored.SessionStorage;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Hosting;
using ILT.IHR.UI.Shared;
using Newtonsoft.Json;

namespace ILT.IHR.UI.Pages.Appraisal.AppraisalDetail
{
    public class AppraisalDetailsBase : ComponentBase
    {
        [Inject]
        public IToastService toastService { get; set; } //Service
        [Inject]
        public ICountryService CountryService { get; set; } //Service
        [Inject]
        public ICompanyService CompanyService { get; set; } //Service
        [Inject]
        public IEndClientService EndClientService { get; set; } //Service
        public DTO.AppraisalDetail AppraisalDetail { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IAppraisalService AppraisalService { get; set; } //Service

        protected DTO.AppraisalDetail selected;

        protected string Title = "Add";

        public ILT.IHR.DTO.AppraisalDetail GoalRating { get; set; }
        public bool ShowDialog { get; set; }
        private int AppraisalID { get; set; }
        public DTO.User user { get; set; }
        [Inject]
        protected DataProvider dataProvider { get; set; }
        [Inject]
        public ISessionStorageService sessionStorage { get; set; } //Service
        [Inject]
        public IEmployeeService EmployeeService { get; set; }

        public ILT.IHR.DTO.Appraisal appraisal = new ILT.IHR.DTO.Appraisal();
        public bool isSaveButtonDisabled { get; set; } = false;
        public bool isSubmitButtonDisabled { get; set; } = false;


        public bool isManagerdisable = false;
        public bool isEmployeedisable = false;  
        public bool isPreviousYearGoaldisable = false;
        public bool isCurrentYeatGoaldisable = false;
        public bool isGoalAchievedDisabled = false;
        public bool isCommentsDisabled = false;
        public bool isManagerFeedBackForEmployee = false;


        [Inject]
        public ILookupService LookupService { get; set; } //Service
        public List<ILT.IHR.DTO.AppraisalDetail> AppraisalDetailPannel1List { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.AppraisalGoal> AppraisalDetailPannel2List { get; set; }  // Table APi Data
        public List<ILT.IHR.DTO.AppraisalGoal> AppraisalDetailPannel3List { get; set; }  // Table APi Data
        public ILT.IHR.DTO.Employee Employee =  new ILT.IHR.DTO.Employee();
        public List<ListValue> AppraisalStatusList { get; set; }
        protected override async Task OnInitializedAsync()
        {
            user = await sessionStorage.GetItemAsync<DTO.User>("User");
            Response<IEnumerable<ListValue>> resps = (await LookupService.GetListValues());
            if (resps.MessageType == MessageType.Success)
            {
                AppraisalStatusList = resps.Data.Where(x => x.Type.ToUpper() == ListTypeConstants.APPRAISALSTATUS).ToList();
            }

            if (dataProvider.storage != null)
            {
                DTO.Appraisal appraisal = (DTO.Appraisal)dataProvider.storage;
                if (appraisal.AppraisalID != 0)
                {
                 
                    AppraisalID = appraisal.AppraisalID;
                    ResetDialog();
                  
                    if (AppraisalID != 0)
                    {
                        GetDetails(AppraisalID);
                    }
                    if (Convert.ToInt32(appraisal.EmployeeID) != 0)
                    {
                        Response<ILT.IHR.DTO.Employee> resp = new Response<ILT.IHR.DTO.Employee>();
                        resp = await EmployeeService.GetEmployeeByIdAsync(Convert.ToInt32(appraisal.EmployeeID)) as Response<ILT.IHR.DTO.Employee>;
                        if (resp.MessageType == MessageType.Success)
                        {
                            Employee = resp.Data;
                            
                        }
                    }
                }
                dataProvider.storage = null;
            }
            else
                NavigationManager.NavigateTo("/appraisal");
        }
      

     
        private async Task GetDetails(int Id)
        {
            Response<ILT.IHR.DTO.Appraisal> resp = new Response<ILT.IHR.DTO.Appraisal>();
            resp = await AppraisalService.GetAppraisalById(Id) as Response<ILT.IHR.DTO.Appraisal>;
            if (resp.MessageType == MessageType.Success)
            {
                appraisal = resp.Data;
                isManagerFeedBackForEmployee = true;
                //if(appraisal.Status.ToUpper() == AppraisalStatus.PENDINGFINALREVIEW)
                //{
                //    isEmployeedisable = true;
                //    isManagerdisable = true;
                //    isPreviousYearGoaldisable = true;
                //    isCurrentYeatGoaldisable = true;
                //}
                if (appraisal.StatusValue.ToUpper() == AppraisalStatus.ASSIGNED)
                {
                    if (user.EmployeeID == appraisal.EmployeeID)
                    {
                        isEmployeedisable = false;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = false;
                    }
                    else
                    {
                        isEmployeedisable = true;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = true;
                    }

                }
                if (appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGREVIEW)
                {
                    if (appraisal.ReviewerID != null && user.EmployeeID == appraisal.ReviewerID)
                    {
                        isEmployeedisable = true;
                        isManagerdisable = false;
                        isGoalAchievedDisabled = false;
                        isCurrentYeatGoaldisable = false;
                        isManagerFeedBackForEmployee = true;
                    }
                    else
                    {
                        isEmployeedisable = true;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = true;
                        isManagerFeedBackForEmployee = true;
                        isCommentsDisabled = true;
                    }
                }
                if (appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGFINALREVIEW)
                {
                    if (appraisal.FinalReviewerID != null && user.EmployeeID ==  appraisal.FinalReviewerID)
                    {
                        isEmployeedisable = true;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = true;
                        isCommentsDisabled = false;
                        isManagerFeedBackForEmployee = false;
                    } else
                    {
                        isEmployeedisable = true;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = true;
                        isCommentsDisabled = true;
                        isManagerFeedBackForEmployee = false;
                    }
                }
                if (appraisal != null && appraisal.StatusValue != null && appraisal.StatusValue.ToUpper() == AppraisalStatus.COMPLETE)
                {
                        isEmployeedisable = true;
                        isManagerdisable = true;
                        isGoalAchievedDisabled = true;
                        isCurrentYeatGoaldisable = true;
                        isCommentsDisabled = true;
                }
                AppraisalDetailPannel1List = appraisal.AppraisalDetails.Where(x => x.ResponseType.ToUpper() == AppraisalResponseType.RATING).ToList();
                if (resp.Data.AppraisalGoals != null && resp.Data.AppraisalGoals.Count > 0)
                {
                    AppraisalDetailPannel3List = resp.Data.AppraisalGoals.Where(p => p.ReviewYear == appraisal.ReviewYear -1).ToList();
                    AppraisalDetailPannel2List = resp.Data.AppraisalGoals.Where(p => p.ReviewYear == appraisal.ReviewYear).ToList();
                    if(AppraisalDetailPannel2List == null || AppraisalDetailPannel2List.Count == 0)
                    {
                        DTO.AppraisalGoal appraisalGoal = new DTO.AppraisalGoal();
                        appraisalGoal.AppraisalID = Id;
                        appraisalGoal.AppraisalGoalID = 0;
                        AppraisalDetailPannel2List.Add(appraisalGoal);
                        DTO.AppraisalGoal appraisalGoal2 = new DTO.AppraisalGoal();
                        appraisalGoal2.AppraisalID = Id;
                        appraisalGoal2.AppraisalGoalID = 0;
                        AppraisalDetailPannel2List.Add(appraisalGoal2);
                        DTO.AppraisalGoal appraisalGoal3 = new DTO.AppraisalGoal();
                        appraisalGoal3.AppraisalID = Id;
                        appraisalGoal3.AppraisalGoalID = 0;
                        AppraisalDetailPannel2List.Add(appraisalGoal3);
                        AppraisalDetailPannel2List.ForEach(x =>
                        {
                            appraisal.AppraisalGoals.Add(x);
                        });
                    };
                }
                GoalRating = appraisal.AppraisalDetails.Find(x => x.ResponseType.ToUpper() == AppraisalResponseType.GOALRATING);
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            Title = "Edit";
            isfirstElementFocus = true;
            ShowDialog = true;
            StateHasChanged();
        }

        protected async Task LoadEmpResponse(string value, int AppraisalDetailID, DTO.AppraisalDetail contxt)
        {
            if(!string.IsNullOrEmpty(value))
            {
                AppraisalDetailPannel1List.ForEach(te =>
                {
                        if (te.AppraisalDetailID == AppraisalDetailID)
                        {
                            te.EmpResponse = null;
                            te.EmpResponse = Convert.ToInt32(value);
                        }
                });
            }
        }

        protected async Task LoadPreYearEmpResponse(string value, int AppraisalGoalID, DTO.AppraisalGoal contxt)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AppraisalDetailPannel3List.ForEach(te =>
                {
                    if (te.AppraisalGoalID == AppraisalGoalID)
                    {
                        te.EmpResponse = null;
                        te.EmpResponse = Convert.ToInt32(value);
                    }
                });
            }
        }

       

        protected async Task LoadPreYearMgrResponse(string value, int AppraisalGoalID, DTO.AppraisalGoal contxt)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AppraisalDetailPannel3List.ForEach(te =>
                {
                    if (te.AppraisalGoalID == AppraisalGoalID)
                    {
                        te.MgrResponse = null;
                        te.MgrResponse = Convert.ToInt32(value);
                    }
                });
            }
        }

        protected async Task LoadMgrResponse(string value, int AppraisalDetailID, DTO.AppraisalDetail contxt)
        {
            if (!string.IsNullOrEmpty(value))
            {
                AppraisalDetailPannel1List.ForEach(te =>
                {
                    if (te.AppraisalDetailID == AppraisalDetailID)
                    {
                        te.MgrResponse = null;
                        te.MgrResponse = Convert.ToInt32(value);
                    }
                });
            }
        }

        protected async Task LoadEmpComment(string EmpComment, int AppraisalDetailID)
        {
            if (!string.IsNullOrEmpty(EmpComment))
            {
                AppraisalDetailPannel1List.ForEach(te =>
            {
                if (string.IsNullOrEmpty(te.EmpComment))
                {
                    if (te.AppraisalDetailID == selected.AppraisalDetailID)
                    {
                        te.EmpComment = EmpComment;
                    }
                }
            });
            }
        }

        public string ErrorMessage;
        protected async Task ValidAppraisal()
        {
        }
            protected async Task AddGoals()
        {
            if ((AppraisalDetailPannel2List.ToList().FindAll(x => string.IsNullOrEmpty(x.Goal)).Count > 0))
            {
                ErrorMessage = "Please Enter All Current Year Goals";
            }
            else
            {
                if(AppraisalDetailPannel2List != null && AppraisalDetailPannel2List.Count < 6)
                {
                    DTO.AppraisalGoal appraisalGoal3 = new DTO.AppraisalGoal();
                    appraisalGoal3.AppraisalID = appraisal.AppraisalID;
                    appraisalGoal3.AppraisalGoalID = 0;
                    AppraisalDetailPannel2List.Add(appraisalGoal3);
                    appraisal.AppraisalGoals.Add(appraisalGoal3);
                }
            }
        }

        protected async Task removeGoal(int index, DTO.AppraisalGoal appraisalGoal)
        {
            if(appraisalGoal !=null && appraisalGoal.AppraisalGoalID != 0)
            {
                var itemToRemove = AppraisalDetailPannel2List.Single(r => r.AppraisalGoalID == appraisalGoal.AppraisalGoalID);
                AppraisalDetailPannel2List.Remove(itemToRemove);
                appraisal.AppraisalGoals.Remove(itemToRemove);
            } else
            {
                if (index > -1)
                {
                    AppraisalDetailPannel2List.RemoveAt(index);
                }
            }
        }


        protected async Task SaveAppraisal()
        {
            SaveAppraisalDetails();
        }
        protected async Task SaveAppraisalDetails()
        {
            if (isSaveButtonDisabled)
                return;
            isSaveButtonDisabled = true;
            string json = JsonConvert.SerializeObject(appraisal);
            var result = await AppraisalService.SaveAppraisal(appraisal);
            if (result.MessageType == MessageType.Success)
            {
                ErrorMessage = "";
                toastService.ShowSuccess("Appraisal Saved successfully", "");
                GetDetails(result.Data.RecordID);
                //Close();
            }
            else
            {
                toastService.ShowError(ErrorMsg.ERRORMSG);
            }
            isSaveButtonDisabled = false;
        }
        protected async Task SubmitAppraisal()
        {
            if (AppraisalDetailPannel2List != null && (AppraisalDetailPannel2List.ToList().FindAll(x => string.IsNullOrEmpty(x.Goal)).Count > 0))
            {
                ErrorMessage = "Please Enter All Current Year Goals";
            }
            else
            {
                if (appraisal != null && !string.IsNullOrEmpty(appraisal.StatusValue))
                {
                    if (appraisal.StatusValue.ToUpper() == AppraisalStatus.ASSIGNED && user.EmployeeID == appraisal.EmployeeID && isEmployeedisable == false)
                    {
                        if (appraisal.AppraisalDetails != null && appraisal.AppraisalDetails.ToList().FindAll(x => x.EmpResponse == 0 || x.EmpResponse == null).Count > 0)
                        {
                            ErrorMessage = "Please Select Employee Rating For all Objectives";

                        }
                        else if (AppraisalDetailPannel3List != null && AppraisalDetailPannel3List.ToList().FindAll(x => x.EmpResponse == 0 || x.EmpResponse == null).Count > 0)
                        {
                            ErrorMessage = "Please Select Employee Rating For All Goals";
                        }
                        else
                        {
                            appraisal.SubmitDate = DateTime.Now;
                            SubmitAppraisalDetails();
                        }
                    }

                    if (appraisal.StatusValue != null && appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGREVIEW && appraisal.ReviewerID != null && user.EmployeeID == appraisal.ReviewerID && isManagerdisable == false)
                    {
                        if (appraisal.AppraisalDetails != null && appraisal.AppraisalDetails.ToList().FindAll(x => x.MgrResponse == 0 || x.MgrResponse == null).Count > 0)
                        {
                            ErrorMessage = "Please Select Manager Rating For all Objectives";

                        }
                        else if (AppraisalDetailPannel3List != null && AppraisalDetailPannel3List.ToList().FindAll(x => x.MgrResponse == 0 || x.MgrResponse == null).Count > 0)
                        {
                            ErrorMessage = "Please Select Manager Rating For All Goals";
                        }
                        else if (string.IsNullOrEmpty(appraisal.MgrFeedback))
                        {
                            ErrorMessage = "Please Enter  Manager FeedBack";
                        }
                        else
                        {
                            appraisal.ReviewDate = DateTime.Now;
                            SubmitAppraisalDetails();
                        }
                    }

                    if (appraisal.StatusValue != null && appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGFINALREVIEW && appraisal.FinalReviewerID != null && user.EmployeeID == appraisal.FinalReviewerID && isCommentsDisabled == false)
                    {
                        if (string.IsNullOrEmpty(appraisal.Comment))
                        {
                            ErrorMessage = "Please Enter Comment";
                        }
                        else
                        {
                            appraisal.FinalReviewDate = DateTime.Now;
                            appraisal.StatusID = AppraisalStatusList.Find(x => x.Value.ToUpper() == AppraisalStatus.COMPLETE).ListValueID;
                            SubmitAppraisalDetails();
                        }
                    }
                }
            }
        }

        protected async Task SubmitAppraisalDetails()
        {
            if (appraisal != null && isEmployeedisable == true && isManagerdisable == true && isGoalAchievedDisabled == true && isCurrentYeatGoaldisable == true)
            {
                if (isSubmitButtonDisabled)
                    return;
                isSubmitButtonDisabled = true;
                var results = await AppraisalService.SaveAppraisal(appraisal);
                if (results.MessageType == MessageType.Success)
                {
                    ErrorMessage = "";
                    toastService.ShowSuccess("Appraisal Saved successfully", "");
                    // GetDetails(result.Data.RecordID);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
                isSubmitButtonDisabled = false;
            }
            else
            {
                if (appraisal.StatusValue.ToUpper() == AppraisalStatus.ASSIGNED)
                {
                    appraisal.StatusID = AppraisalStatusList.Find(x => x.Value.ToUpper() == AppraisalStatus.PENDINGREVIEW).ListValueID;
                }
                else if (appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGREVIEW)
                {
                    appraisal.StatusID = AppraisalStatusList.Find(x => x.Value.ToUpper() == AppraisalStatus.PENDINGFINALREVIEW).ListValueID;
                }
                else if (appraisal.StatusValue.ToUpper() == AppraisalStatus.PENDINGFINALREVIEW)
                {
                    appraisal.StatusID = AppraisalStatusList.Find(x => x.Value.ToUpper() == AppraisalStatus.COMPLETE).ListValueID;
                }
                if (isSubmitButtonDisabled)
                    return;
                isSubmitButtonDisabled = true;
                var result = await AppraisalService.SaveAppraisal(appraisal);
                if (result.MessageType == MessageType.Success)
                {
                    ErrorMessage = "";
                    toastService.ShowSuccess("Appraisal Saved successfully", "");
                    // GetDetails(result.Data.RecordID);
                    Cancel();
                }
                else
                {
                    toastService.ShowError(ErrorMsg.ERRORMSG);
                }
                isSubmitButtonDisabled = false;
            }
        }

            public void Cancel()
        {
            NavigationManager.NavigateTo("/appraisal");
             // ShowDialog = false;
            // ListValueUpdated.InvokeAsync(true);
            StateHasChanged();
        }
        public void Close()
        {
            ShowDialog = false;
            StateHasChanged();
        }

        private void ResetDialog()
        {
            appraisal = new ILT.IHR.DTO.Appraisal { };
        }

        public async void RowClick(DTO.AppraisalDetail data)
        {
            selected = data;
            // GetAssignmentRates();
        }


        [Inject] IJSRuntime JSRuntime { get; set; }
        public bool isfirstElementFocus { get; set; }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (isfirstElementFocus)
            {
                JSRuntime.InvokeVoidAsync("JSHelpers.setFocusByCSSClass");
                isfirstElementFocus = false;
            }
        }

        protected string FormatDate(DateTime? dateTime)
        {
            string formattedDate = "";
            if (dateTime != null)
            {
                var date = dateTime.Value.ToString("MM/dd/yyyy");
                formattedDate = date;
            }

            return formattedDate;
        }



    }
}
