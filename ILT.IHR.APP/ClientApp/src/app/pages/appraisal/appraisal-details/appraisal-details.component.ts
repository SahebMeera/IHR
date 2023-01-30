import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { AppraisalResponseType, AppraisalStatus, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../constant';
import { IAppraisal, IAppraisalDisplay } from '../../../core/interfaces/Appraisal';
import { IAppraisalDetailDisplay } from '../../../core/interfaces/AppraisalDetail';
import { IAppraisalGoal, IAppraisalGoalDisplay } from '../../../core/interfaces/AppraisalGoal';
import { IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { DataProvider } from '../../../core/providers/data.provider';
import { EmployeeService } from '../../employee/employee.service';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { AppraisalService } from '../appraisal.service';

@Component({
  selector: 'app-appraisal-details',
  templateUrl: './appraisal-details.component.html',
  styleUrls: ['./appraisal-details.component.scss']
})
export class AppraisalDetailsComponent implements OnInit {
    appraisalStatus = AppraisalStatus;
    commonUtils = new CommonUtils()
    myDate = new Date();
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeRolePermission: IRolePermissionDisplay;
    user: any;
    isEditMode: boolean = false;
    appraisalDetails: IAppraisalDisplay;

    appraisalForm: FormGroup;

    AppraisalID: number;
    EmployeeID: number;

    isManagerdisable: boolean = false;
    isEmployeedisable: boolean = false;
    isPreviousYearGoaldisable: boolean = false;
    isCurrentYeatGoaldisable: boolean = false;
    isGoalAchievedDisabled: boolean = false;
    isCommentsDisabled: boolean = false;
    isManagerFeedBackForEmployee: boolean = false;

    AppraisalDetailPannel1List: IAppraisalDetailDisplay[] = []
    AppraisalDetailPannel2List: IAppraisalGoalDisplay[] = []
    AppraisalDetailPannel3List: IAppraisalGoalDisplay[] = []
    AppraisalStatusList: any[] = [];

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private holidayService: HolidayService,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private router: Router,
        private activeRoute: ActivatedRoute,
        private rolePermissionService: RolePermissionService,
        private appraisalService: AppraisalService,
        private lookupService: LookUpService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.buildAppraisalForm({}, 'New');
        if (this.dataProvider.storage) {
            this.isEditMode = this.dataProvider.storage["isEditMode"]
            if (this.isEditMode) {
                var appraisalDetails = this.dataProvider.storage["appraisalDetails"]
                if (appraisalDetails !== null && appraisalDetails !== undefined) {
                    this.appraisalDetails = appraisalDetails
                    this.AppraisalID = appraisalDetails.appraisalID;
                    if (this.AppraisalID != 0 && this.AppraisalID !== undefined) {
                        this.GetDetails(this.AppraisalID);
                    }
                    this.EmployeeID = appraisalDetails.employeeID;
                    this.loadEmployeeDetails(appraisalDetails.employeeID)
                }
            }
        }
    }
 
  ngOnInit(): void {
  }

    LoadDropDown() {
            this.lookupService.getListValues().subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.AppraisalStatusList = result['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.APPRAISALSTATUS);
            }
        })
    }

    Employee: IEmployeeDisplay
    loadEmployeeDetails(empID: number) {
        if (empID !== undefined && empID !== null && empID !== 0) {
            this.employeeService.getEmployeeByIdAsync(empID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.Employee = result['data'];
                }
            })
        }
    }

    appraisal: IAppraisalDisplay;
    GoalRating: IAppraisalDetailDisplay
    GetDetails(Id: number) {
        this.appraisalService.GetAppraisalById(Id).subscribe(resp => {
            if (resp !== undefined && resp['data'] !== null && resp['data'] !== undefined) {
                this.appraisal = resp['data'];
                this.isManagerFeedBackForEmployee = true;

                if (this.appraisal.statusValue.toUpperCase() == AppraisalStatus.ASSIGNED) {
                    if (this.user.employeeID == this.appraisal.employeeID) {
                        this.isEmployeedisable = false;
                        this.isManagerdisable = true;
                        this.isGoalAchievedDisabled = true;
                        this.isCurrentYeatGoaldisable = false;
                    }
                    else {
                        this.isEmployeedisable = true;
                        this.isManagerdisable = true;
                        this.isGoalAchievedDisabled = true;
                        this.isCurrentYeatGoaldisable = true;
                    }

                }
                if (this.appraisal.statusValue.toUpperCase() == AppraisalStatus.PENDINGREVIEW) {
                    if (this.appraisal.reviewerID != null && this.user.employeeID == this.appraisal.reviewerID) {
                        this.isEmployeedisable = true;
                        this.isManagerdisable = false;
                        this.isGoalAchievedDisabled = false;
                        this.isCurrentYeatGoaldisable = false;
                        this.isManagerFeedBackForEmployee = true;
                    }
                    else {
                        this.isEmployeedisable = true;
                        this.isManagerdisable = true;
                        this.isGoalAchievedDisabled = true;
                        this.isCurrentYeatGoaldisable = true;
                        this.isManagerFeedBackForEmployee = true;
                        this.isCommentsDisabled = true;
                    }
                }
                if (this.appraisal.statusValue.toUpperCase() == AppraisalStatus.PENDINGFINALREVIEW) {
                    if (this.appraisal.finalReviewerID != null && this.user.employeeID == this.appraisal.finalReviewerID) {
                        this.isEmployeedisable = true;
                        this.isManagerdisable = true;
                        this.isGoalAchievedDisabled = true;
                        this.isCurrentYeatGoaldisable = true;
                        this.isCommentsDisabled = false;
                        this.isManagerFeedBackForEmployee = false;
                    } else {
                        this.isEmployeedisable = true;
                        this.isManagerdisable = true;
                        this.isGoalAchievedDisabled = true;
                        this.isCurrentYeatGoaldisable = true;
                        this.isCommentsDisabled = true;
                        this.isManagerFeedBackForEmployee = false;
                    }
                }
                if (this.appraisal != null && this.appraisal.statusValue != null && this.appraisal.statusValue.toUpperCase() == AppraisalStatus.COMPLETE) {
                    this.isEmployeedisable = true;
                    this.isManagerdisable = true;
                    this.isGoalAchievedDisabled = true;
                    this.isCurrentYeatGoaldisable = true;
                    this.isCommentsDisabled = true;
                }
                this.AppraisalDetailPannel1List = this.appraisal.appraisalDetails.filter(x => x.responseType.toUpperCase() == AppraisalResponseType.RATING);
                if (resp['data'].appraisalGoals != undefined && resp['data'].appraisalGoals != null && resp['data'].appraisalGoals.length > 0) {
                    this.AppraisalDetailPannel3List = resp['data'].appraisalGoals.filter(p => p.reviewYear == this.appraisal.reviewYear - 1);
                    this.AppraisalDetailPannel2List = resp['data'].appraisalGoals.filter(p => p.reviewYear == this.appraisal.reviewYear);
                    if (this.AppraisalDetailPannel2List == null || this.AppraisalDetailPannel2List.length == 0) {
                        this.AppraisalDetailPannel2List.push({ appraisalGoalID: 0, appraisalID: Id });
                        this.AppraisalDetailPannel2List.push({ appraisalGoalID: 0, appraisalID: Id });
                        this.AppraisalDetailPannel2List.push({ appraisalGoalID: 0, appraisalID: Id });

                        this.AppraisalDetailPannel2List.forEach(x => {
                            this.appraisal.appraisalGoals.push(x);
                        });
                    };
                }
                this.GoalRating = this.appraisal.appraisalDetails.find(x => x.responseType.toUpperCase() == AppraisalResponseType.GOALRATING);
                this.buildAppraisalForm(this.appraisal,'Eddit')
            }
            else {
                //toastService.ShowError(ErrorMsg.ERRORMSG);
            }
        })

    }


    buildAppraisalForm(data: any, keyName: string) {
        this.appraisalForm = this.fb.group({
            AppraisalID: [keyName === 'New' ? 0 : data.appraisalID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? null : data.employeeName],
            FinalReviewerID: [keyName === 'New' ? null : data.finalReviewerID],
            Manager: [keyName === 'New' ? '' : data.manager],
            FinalReviewer: [keyName === 'New' ? '' : data.finalReviewer],
            ReviewerID: [keyName === 'New' ? null : data.reviewerID],
            Reviewer: [keyName === 'New' ? new Date() : data.reviewer],
            StatusID: [keyName === 'New' ? null : data.statusID, Validators.required],
            Status: [keyName === 'New' ? null : data.status],
            StatusValue: [keyName === 'New' ? null : data.statusValue],
            MgrFeedback: [keyName === 'New' ? null : data.mgrFeedback],
            AssignedDate: [keyName === 'New' ? null : data.assignedDate !== null ? new Date(data.assignedDate) : null],
            SubmitDate: [keyName === 'New' ? null : data.submitDate !== null ? new Date(data.submitDate) : null],
            ReviewDate: [keyName === 'New' ? null : data.reviewDate !== null ? new Date(data.reviewDate) : null],
            FinalReviewDate: [keyName === 'New' ? null : data.finalReviewDate !== null ? new Date(data.finalReviewDate) : null],
            Comment: [keyName === 'New' ? '' : data.comment],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : new Date(data.createdDate)],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: [''],
            AppraisalDetails: this.AppraisalDetailPannel1List
        });
    }

    removeGoal( index: number, appraisalGoal: IAppraisalGoalDisplay) {
        if (appraisalGoal != null && appraisalGoal.appraisalGoalID != 0) {
            var itemToRemove = this.AppraisalDetailPannel2List.find(r => r.appraisalGoalID == appraisalGoal.appraisalGoalID);
            this.AppraisalDetailPannel2List.splice(this.AppraisalDetailPannel2List.indexOf(itemToRemove), 1)
            this.appraisal.appraisalGoals.splice(this.appraisal.appraisalGoals.indexOf(itemToRemove), 1)
            //this.AppraisalDetailPannel2List.Remove(itemToRemove);
            //this.appraisal.AppraisalGoals.Remove(itemToRemove);
        } else {
            if (index > -1) {
                this.AppraisalDetailPannel2List.splice(index, 1);
            }
        }
    }
    ErrorMessage: string;
    AddGoals() {
        if (this.AppraisalDetailPannel2List.filter(x => x.goal === undefined || x.goal === null || x.goal === '').length > 0) {
          //  this.ErrorMessage = "Please Enter All Current Year Goals";
            this.messageService.add({ severity: 'error', summary: 'Please Enter All Current Year Goals', detail: '' });
        }
        else {
            if (this.AppraisalDetailPannel2List != null && this.AppraisalDetailPannel2List.length < 6) {
                this.AppraisalDetailPannel2List.push({ appraisalGoalID: 0, appraisalID: this.appraisal.appraisalID, reviewYear: 0, goal: '', empResponse: null, empComment: '', mgrResponse: null, mgrComment: null, createdBy: this.appraisal.createdBy, createdDate: this.appraisal.createdDate, modifiedBy: this.appraisal.modifiedBy, modifiedDate: this.appraisal.modifiedDate});
                this.appraisal.appraisalGoals.push({ appraisalGoalID: 0, appraisalID: this.appraisal.appraisalID, reviewYear: 0, goal: '', empResponse: null, empComment: '', mgrResponse: null, mgrComment: null, createdBy: this.appraisal.createdBy, createdDate: this.appraisal.createdDate, modifiedBy: this.appraisal.modifiedBy, modifiedDate: this.appraisal.modifiedDate});
            }

        }
    }
    isSaveButtonDisabled: boolean = false;
    submitted: boolean = false;
    SaveAppraisal() {
        if (this.isSaveButtonDisabled) {
            return;
        } else {
            this.isSaveButtonDisabled = true;
            this.SaveAppraisalDetails();
            this.isSaveButtonDisabled = false;
        }
    }
    goals: IAppraisalGoalDisplay[] = []
    SaveAppraisalDetails() {
        //console.log(this.appraisalForm.value)
        //console.log(this.appraisal)
        //console.log(this.AppraisalDetailPannel2List)
        //console.log(this.AppraisalDetailPannel3List)
        var appraisal = new IAppraisal();
        appraisal.AppraisalID = this.appraisal.appraisalID,
        appraisal.ReviewYear = Number(this.appraisal.reviewYear),
        appraisal.EmployeeID = this.appraisal.employeeID,
        appraisal.EmployeeName = this.appraisal.employeeName,
        appraisal.FinalReviewerID = this.appraisal.finalReviewerID,
        appraisal.Manager = this.appraisal.manager,
        appraisal.FinalReviewer = this.appraisal.finalReviewer,
        appraisal.ReviewerID = this.appraisal.reviewerID,
        appraisal.Reviewer = this.appraisal.reviewer,
        appraisal.MgrFeedback = this.appraisal.mgrFeedback,
        appraisal.AssignedDate = this.appraisal.assignedDate,
        appraisal.SubmitDate = this.appraisal.submitDate,
        appraisal.ReviewDate = this.appraisal.reviewDate,
        appraisal.Comment = this.appraisal.comment,
        appraisal.FinalReviewDate = this.appraisal.finalReviewDate,
        appraisal.StatusID = this.appraisal.statusID,
        appraisal.Status = this.appraisal.status,
        appraisal.StatusValue = this.appraisal.statusValue

        if (this.AppraisalDetailPannel1List !== undefined && this.AppraisalDetailPannel1List.length > 0) {
            appraisal['AppraisalDetails'] = [];
            this.AppraisalDetailPannel1List.forEach(x => {
                let a = {
                    AppraisalDetailID: x.appraisalDetailID,
                    AppraisalID: x.appraisalID,
                    AppraisalQualityID: x.appraisalQualityID,
                    Quality: x.quality,
                    ResponseTypeID: x.responseTypeID,
                    ResponseType: x.responseType,
                    ResponseTypeDescription: x.responseTypeDescription,
                    EmpResponse: x.empResponse,
                    EmpComment: x.empComment,
                    MgrResponse: x.mgrResponse,
                    MgrComment: x.mgrComment,
                    CreatedBy: x.createdBy,
                    CreatedDate: x.createdDate,
                    ModifiedBy: x.modifiedBy,
                    ModifiedDate: x.modifiedDate
                }
                appraisal['AppraisalDetails'].push(a)
            })
        } else {
            appraisal['AppraisalDetails'] = []
        }
         this.goals = [...this.AppraisalDetailPannel2List, ...this.AppraisalDetailPannel3List]
        if (this.appraisal.appraisalGoals !== undefined && this.appraisal.appraisalGoals !== null && this.appraisal.appraisalGoals.length > 0) {
            appraisal['AppraisalGoals'] = [];
            this.goals.forEach(x => {
                let a = {
                    AppraisalGoalID: x.appraisalGoalID,
                    AppraisalID: x.appraisalID,
                    ReviewYear: Number(x.reviewYear),
                    Goal: x.goal,
                    EmpResponse: x.empResponse,
                    EmpComment: x.empComment,
                    MgrResponse: x.mgrResponse,
                    MgrComment: x.mgrComment,
                    CreatedBy: x.createdBy,
                    CreatedDate: x.createdDate,
                    ModifiedBy: x.modifiedBy,
                    ModifiedDate: x.modifiedDate
                }
                appraisal['AppraisalGoals'].push(a)
            })
        } else {
            appraisal['AppraisalGoals'] = []
        }
        this.appraisalService.SaveAppraisal(appraisal).subscribe(result => {
            if (result['data'] !== null && result['messageType'] === 1) {
                this.messageService.add({ severity: 'success', summary: 'Appraisal saved successfully', detail: '' });
                this.isSaveButtonDisabled = true;
                this.GetDetails(result['data'].recordID);

                //this.ExpenseUpdated.emit();
                //this.cancel();
            } else {
                this.isSaveButtonDisabled = false;
                this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
            }
        })
    }
    SubmitAppraisal() {
        var appraisal = new IAppraisal();
        appraisal.AppraisalID = this.appraisal.appraisalID,
            appraisal.ReviewYear = Number(this.appraisal.reviewYear),
            appraisal.EmployeeID = this.appraisal.employeeID,
            appraisal.EmployeeName = this.appraisal.employeeName,
            appraisal.FinalReviewerID = this.appraisal.finalReviewerID,
            appraisal.Manager = this.appraisal.manager,
            appraisal.FinalReviewer = this.appraisal.finalReviewer,
            appraisal.ReviewerID = this.appraisal.reviewerID,
            appraisal.Reviewer = this.appraisal.reviewer,
            appraisal.MgrFeedback = this.appraisal.mgrFeedback,
            appraisal.AssignedDate = this.appraisal.assignedDate,
            appraisal.SubmitDate = this.appraisal.submitDate,
            appraisal.ReviewDate = this.appraisal.reviewDate,
            appraisal.Comment = this.appraisal.comment,
            appraisal.FinalReviewDate = this.appraisal.finalReviewDate,
            appraisal.StatusID = this.appraisal.statusID,
            appraisal.Status = this.appraisal.status,
            appraisal.StatusValue = this.appraisal.statusValue

        if (this.AppraisalDetailPannel1List !== undefined && this.AppraisalDetailPannel1List.length > 0) {
            appraisal['AppraisalDetails'] = [];
            this.AppraisalDetailPannel1List.forEach(x => {
                let a = {
                    AppraisalDetailID: x.appraisalDetailID,
                    AppraisalID: x.appraisalID,
                    AppraisalQualityID: x.appraisalQualityID,
                    Quality: x.quality,
                    ResponseTypeID: x.responseTypeID,
                    ResponseType: x.responseType,
                    ResponseTypeDescription: x.responseTypeDescription,
                    EmpResponse: x.empResponse,
                    EmpComment: x.empComment,
                    MgrResponse: x.mgrResponse,
                    MgrComment: x.mgrComment,
                    CreatedBy: x.createdBy,
                    CreatedDate: x.createdDate,
                    ModifiedBy: x.modifiedBy,
                    ModifiedDate: x.modifiedDate
                }
                appraisal['AppraisalDetails'].push(a)
            })
        } else {
            appraisal['AppraisalDetails'] = []
        }
        this.goals = [...this.AppraisalDetailPannel2List, ...this.AppraisalDetailPannel3List]
        if (this.appraisal.appraisalGoals !== undefined && this.appraisal.appraisalGoals !== null && this.appraisal.appraisalGoals.length > 0) {
            appraisal['AppraisalGoals'] = [];
            this.goals.forEach(x => {
                let a = {
                    AppraisalGoalID: x.appraisalGoalID,
                    AppraisalID: x.appraisalID,
                    ReviewYear: Number(x.reviewYear),
                    Goal: x.goal,
                    EmpResponse: x.empResponse,
                    EmpComment: x.empComment,
                    MgrResponse: x.mgrResponse,
                    MgrComment: x.mgrComment,
                    CreatedBy: x.createdBy,
                    CreatedDate: x.createdDate,
                    ModifiedBy: x.modifiedBy,
                    ModifiedDate: x.modifiedDate
                }
                appraisal['AppraisalGoals'].push(a)
            })
        } else {
            appraisal['AppraisalGoals'] = []
        }
        if (this.AppraisalDetailPannel2List.filter(x => x.goal === undefined || x.goal === null || x.goal === '').length > 0) {
            //  this.ErrorMessage = "Please Enter All Current Year Goals";
            this.messageService.add({ severity: 'error', summary: 'Please Enter All Current Year Goals', detail: '' });
        } else {
            if (appraisal !== null && this.appraisal !== undefined && this.appraisal.statusValue !== null && this.appraisal.statusValue !== undefined && this.appraisal.statusValue !== '') {
                if (appraisal.StatusValue.toUpperCase() == AppraisalStatus.ASSIGNED && this.user.employeeID == appraisal.EmployeeID && this.isEmployeedisable == false) {
                    if (appraisal['AppraisalDetails'] != null && appraisal['AppraisalDetails'].filter(x => x.EmpResponse == 0 || x.EmpResponse == null).length > 0) {
                        //ErrorMessage = "Please Select Employee Rating For all Objectives";
                        this.messageService.add({ severity: 'error', summary: 'Please Select Employee Rating For all Objectives', detail: '' });
                    }
                    else if (this.AppraisalDetailPannel3List != null && this.AppraisalDetailPannel3List.filter(x => x.empResponse == 0 || x.empResponse == null).length > 0) {
                        //ErrorMessage = "Please Select Employee Rating For All Goals";
                        this.messageService.add({ severity: 'error', summary: 'Please Select Employee Rating For All Goals', detail: '' });

                    }
                    else {
                        appraisal.SubmitDate = new Date();
                        this.SubmitAppraisalDetails(appraisal);
                    }
                }

                if (appraisal.StatusValue.toUpperCase() == AppraisalStatus.PENDINGREVIEW && appraisal.ReviewerID != null && this.user.employeeID == appraisal.ReviewerID && this.isManagerdisable == false) {
                    if (appraisal['AppraisalDetails'] != null && appraisal['AppraisalDetails'].filter(x => x.MgrResponse == 0 || x.MgrResponse == null).length > 0) {
                        this.messageService.add({ severity: 'error', summary: 'Please Select Manager Rating For all Objectives', detail: '' });
                    }
                    else if (this.AppraisalDetailPannel3List != null && this.AppraisalDetailPannel3List.filter(x => x.mgrResponse == 0 || x.mgrResponse == null).length > 0) {
                        this.messageService.add({ severity: 'error', summary: 'Please Select Manager Rating For All Goals', detail: '' });

                    } else if (appraisal.MgrFeedback === null || appraisal.MgrFeedback === '') {
                        this.messageService.add({ severity: 'error', summary: 'Please Enter  Manager FeedBack', detail: '' });
                    }
                    else {
                        appraisal.ReviewDate = new Date();
                        this.SubmitAppraisalDetails(appraisal);
                    }
                }

                if (appraisal.StatusValue != null && appraisal.StatusValue.toUpperCase() == AppraisalStatus.PENDINGFINALREVIEW && appraisal.FinalReviewerID != null && this.user.EmployeeID == appraisal.FinalReviewerID && this.isCommentsDisabled == false) {
                    if (appraisal.Comment === null || appraisal.Comment === '') {
                        //ErrorMessage = "Please Enter Comment";
                        this.messageService.add({ severity: 'error', summary: 'Please Enter Comment', detail: '' });

                    }
                    else {
                        appraisal.FinalReviewDate = new Date();
                        appraisal.StatusID = this.AppraisalStatusList.find(x => x.value.toUpperCase() == AppraisalStatus.COMPLETE).listValueID;
                        this.SubmitAppraisalDetails(appraisal);
                    }
                }
            }
        }
    }
    isSubmitButtonDisabled: boolean = false;
    SubmitAppraisalDetails(appraisal: IAppraisal) {
        if (appraisal != null && this.isEmployeedisable == true && this.isManagerdisable == true && this.isGoalAchievedDisabled == true && this.isCurrentYeatGoaldisable == true) {
            if (this.isSubmitButtonDisabled)
                return;
            this.isSubmitButtonDisabled = true;
            this.appraisalService.SaveAppraisal(appraisal).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'Appraisal saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    //this.GetDetails(result['data'].recordID);
                    //this.ExpenseUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
            this.isSubmitButtonDisabled = false;
        }
        else {
            if (appraisal.StatusValue.toUpperCase() == AppraisalStatus.ASSIGNED) {
                appraisal.StatusID = this.AppraisalStatusList.find(x => x.value.toUpperCase() == AppraisalStatus.PENDINGREVIEW).listValueID;
            }
            else if (appraisal.StatusValue.toUpperCase() == AppraisalStatus.PENDINGREVIEW) {
                appraisal.StatusID = this.AppraisalStatusList.find(x => x.value.toUpperCase() == AppraisalStatus.PENDINGFINALREVIEW).listValueID;
            }
            else if (appraisal.StatusValue.toUpperCase() == AppraisalStatus.PENDINGFINALREVIEW) {
                appraisal.StatusID = this.AppraisalStatusList.find(x => x.value.toUpperCase() == AppraisalStatus.COMPLETE).listValueID;
            }
            if (this.isSubmitButtonDisabled)
                return;
            this.isSubmitButtonDisabled = true;
            this.appraisalService.SaveAppraisal(appraisal).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'Appraisal saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    //this.GetDetails(result['data'].recordID);
                    //this.ExpenseUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
            this.isSubmitButtonDisabled = false;
        }
    }
    cancel() {
        this.router.navigate(['appraisal']);
    }

}
