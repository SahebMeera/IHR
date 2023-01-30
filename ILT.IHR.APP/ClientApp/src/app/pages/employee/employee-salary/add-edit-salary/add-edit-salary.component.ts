import { DecimalPipe } from '@angular/common';
import { decimalDigest } from '@angular/compiler/src/i18n/digest';
import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import * as moment from 'moment';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../../common/common-utils';
import { Constants, ErrorMsg, SessionConstants } from '../../../../constant';
import { IRolePermissionDisplay } from '../../../../core/interfaces/RolePermission';
import { IModalPopupAlternateOptions } from '../../../../shared/ihr-modal-popup/ihr-modal-popup-alternate-options';
import { IHRModalPopupComponent } from '../../../../shared/ihr-modal-popup/ihr-modal-popup.component';
import { EmployeeService } from '../../employee.service';
import { EmployeeSalaryService } from '../employee-salary.service';

@Component({
    selector: 'app-add-edit-salary',
    templateUrl: './add-edit-salary.component.html',
    styleUrls: ['./add-edit-salary.component.scss']
})
export class AddEditSalaryComponent implements OnInit {
    commonUtils = new CommonUtils()
    @Input() EmployeeName: string;
    @Output() ListValueUpdated = new EventEmitter<any>();
    @ViewChild('addEditSalaryModal') addEditSalaryModal: IHRModalPopupComponent;
    modalOptions: IModalPopupAlternateOptions;
    isSaveButtonDisabled: boolean = false;
    DependentId: number;
    ModalHeading: string = 'Add Salary';
    isShow: boolean;
    submitted: boolean = false;
    SalaryForm: FormGroup;
    user: any;
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    SalaryId: number = -1;

    SalaryBasic: any = 0.00;
    Salarybonus: number = 0;
    SalaryEducationAllowance: number = 0;
    SalarySpecialAllowance: number = 0;
    SalaryLTA: number = 0;
    SalaryVariablePay: number = 0;
    SalaryMedicalInsurance: number = 0;
    SalaryHRA: number = 0;
    SalaryConveyance: number = 0;
    SalaryMealAllowance: number = 0;
    SalaryMedicalAllowance: number = 0;
    SalaryTelephoneAllowance: number = 0;
    SalaryGratuity: number = 0;
    ProvidentFund: number = 0;

    constructor(private fb: FormBuilder,
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private salaryService: EmployeeSalaryService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.SALARY);
        this.user = JSON.parse(localStorage.getItem("User"));
        this.buildSalaryForm({}, 'New');
    }

    ngOnInit(): void {
        this.loadModalOptions();
    }
    
    Show(Id: number, employeeId: number) {
        this.SalaryId = Id;
        this.ResetDialog();
        this.isShow = false;
        this.loadEmployeeData(employeeId);
        if (this.SalaryId !== 0) {
            this.isShow = this.EmployeeInfoRolePermission.update;
            this.loadModalOptions();
            this.GetDetails(this.SalaryId);
        }
        else {
            this.isShow = this.EmployeeInfoRolePermission.add;
            this.loadModalOptions();
            this.ModalHeading = "Add Salary";
            this.buildSalaryForm({}, 'New');
           
            this.SalaryForm.controls.EmployeeID.patchValue(employeeId)
            this.SalaryForm.controls.EmployeeName.patchValue(this.EmployeeName);

            this.addEditSalaryModal.show();
        }
    }
    GetDetails(Id: number) {

        this.salaryService.getEmployeeSalaryById(Id).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                this.buildSalaryForm(result['data'], 'Edit');
                this.ModalHeading = "Edit Salary";
                this.SalaryId = result['data'].salaryID;
                this.isSaveButtonDisabled = false;
                this.setValueCTCPMPA();
                this.addEditSalaryModal.show();
            }
            })
    }
    setValueCTCPMPA() {
        this.GetTotalCostToCompanyPM();
        this.GettotalCostToCompanyPA();
    }

    buildSalaryForm(data: any, keyName: string) {
        this.SalaryForm = this.fb.group({
            SalaryID: [keyName === 'New' ? 0 : data.salaryID],
            EmployeeID: [keyName === 'New' ? null : data.employeeID],
            EmployeeName: [keyName === 'New' ? '' : data.employeeName],
            BasicPay: [keyName === 'New' ? null : data.basicPay, Validators.required],
            HRA: [keyName === 'New' ? 0 : data.hra !== null ? parseFloat(data.hra) : 0, Validators.required],
            Bonus: [keyName === 'New' ? 0 : data.bonus !== null ? parseFloat(data.bonus) : 0, Validators.required],
            LTA: [keyName === 'New' ? 0 : data.lta !== null ? parseFloat(data.lta) : 0, Validators.required],
            EducationAllowance: [keyName === 'New' ? 0 : data.educationAllowance !== null ? parseFloat(data.educationAllowance) : 0, Validators.required],
            VariablePay: [keyName === 'New' ? 0 : data.variablePay !== null ? parseFloat(data.variablePay) : 0, Validators.required],
            SpecialAllowance: [keyName === 'New' ? 0 : data.specialAllowance !== null ? parseFloat(data.specialAllowance) : 0, Validators.required],
            ProvidentFund: [keyName === 'New' ? 0 : data.providentFund !== null ? parseFloat(data.providentFund) : 0, Validators.required],
            TelephoneAllowance: [keyName === 'New' ? 0 : data.telephoneAllowance !== null ? parseFloat(data.telephoneAllowance) : 0, Validators.required],
            MedicalAllowance: [keyName === 'New' ? 0 : data.medicalAllowance !== null ? parseFloat(data.medicalAllowance) : 0, Validators.required],
            MedicalInsurance: [keyName === 'New' ? 0 : data.medicalInsurance !== null ? parseFloat(data.medicalInsurance) : 0, Validators.required],
            MealAllowance: [keyName === 'New' ? 0 : data.mealAllowance !== null ? parseFloat(data.mealAllowance) : 0, Validators.required],
            Conveyance: [keyName === 'New' ? 0 : data.conveyance !== null ? parseFloat(data.conveyance) : 0, Validators.required],
            Gratuity: [keyName === 'New' ? 0 : data.gonveyance !== null ? parseFloat(data.gratuity) : 0, Validators.required],
            CostToCompany: [keyName === 'New' ? 0 : data.costToCompany !== null ? parseFloat(data.costToCompany) : 0],
            TotalCTCPM: this.TotalCTCPM,
            StartDate: [keyName === 'New' ? new Date() : new Date(data.startDate), Validators.required],
            EndDate: [keyName === 'New' ? null : data.endDate !== null ? new Date(data.endDate) : null],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: ['']
        });
    }
    get addEditSalaryFormControls() { return this.SalaryForm.controls; }
    SalaryFormConvertDecimal() {
        if (this.SalaryForm.value.BasicPay !== null && this.SalaryForm.value.BasicPay !== undefined) {
            this.SalaryForm.value.BasicPay = this.SalaryForm.value.BasicPay.toString();
        }
        if (this.SalaryForm.value.HRA !== null && this.SalaryForm.value.HRA !== undefined) {
            this.SalaryForm.value.HRA = this.SalaryForm.value.HRA.toString();
        }
        if (this.SalaryForm.value.Bonus !== null && this.SalaryForm.value.Bonus !== undefined) {
            this.SalaryForm.value.Bonus = this.SalaryForm.value.Bonus.toString();
        }
        if(this.SalaryForm.value.LTA !== null && this.SalaryForm.value.LTA !== undefined) {
            this.SalaryForm.value.LTA = this.SalaryForm.value.LTA.toString();
        }
        if (this.SalaryForm.value.EducationAllowance !== null && this.SalaryForm.value.EducationAllowance !== undefined) {
            this.SalaryForm.value.EducationAllowance = this.SalaryForm.value.EducationAllowance.toString();
        }
        if (this.SalaryForm.value.VariablePay !== null && this.SalaryForm.value.VariablePay !== undefined) {
            this.SalaryForm.value.VariablePay = this.SalaryForm.value.VariablePay.toString();
        }
        if (this.SalaryForm.value.SpecialAllowance !== null && this.SalaryForm.value.SpecialAllowance !== undefined) {
            this.SalaryForm.value.SpecialAllowance = this.SalaryForm.value.SpecialAllowance.toString();
        }
        if (this.SalaryForm.value.ProvidentFund !== null && this.SalaryForm.value.ProvidentFund !== undefined) {
            this.SalaryForm.value.ProvidentFund = this.SalaryForm.value.ProvidentFund.toString();
        }
        if (this.SalaryForm.value.TelephoneAllowance !== null && this.SalaryForm.value.TelephoneAllowance !== undefined) {
            this.SalaryForm.value.TelephoneAllowance = this.SalaryForm.value.TelephoneAllowance.toString();
        }
        if (this.SalaryForm.value.MedicalInsurance !== null && this.SalaryForm.value.MedicalInsurance !== undefined) {
            this.SalaryForm.value.MedicalInsurance = this.SalaryForm.value.MedicalInsurance.toString();
        }
        if (this.SalaryForm.value.MedicalAllowance !== null && this.SalaryForm.value.MedicalAllowance !== undefined) {
            this.SalaryForm.value.MedicalAllowance = this.SalaryForm.value.MedicalAllowance.toString();
        }
        if (this.SalaryForm.value.MealAllowance !== null && this.SalaryForm.value.MealAllowance !== undefined) {
            this.SalaryForm.value.MealAllowance = this.SalaryForm.value.MealAllowance.toString();
        }
        if (this.SalaryForm.value.Conveyance !== null && this.SalaryForm.value.Conveyance !== undefined) {
            this.SalaryForm.value.Conveyance = this.SalaryForm.value.Conveyance.toString();
        }
        if (this.SalaryForm.value.Gratuity !== null && this.SalaryForm.value.Gratuity !== undefined) {
            this.SalaryForm.value.Gratuity = this.SalaryForm.value.Gratuity.toString();
        }
        if (this.SalaryForm.value.CostToCompany !== null && this.SalaryForm.value.CostToCompany !== undefined) {
            this.SalaryForm.value.CostToCompany = this.SalaryForm.value.CostToCompany.toString();
        }
        this.SalaryForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.SalaryForm.value.StartDate));
        if (this.SalaryForm.value.EndDate !== null) {
            this.SalaryForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.SalaryForm.value.EndDate));
        }
    }
    loadModalOptions() {
        this.modalOptions = {
            footerActions: [
                {
                    actionText: 'Save',
                    actionMethod: this.save,
                    styleClass: this.isShow ? 'btn-width-height p-button-raised p-mr-2 p-mb-2' : 'btn-width-height p-button-raised p-mr-2 p-mb-2 display-none',
                    iconClass: 'mdi mdi-content-save',
                    disabled: this.isSaveButtonDisabled
                },
                {
                    actionText: 'Cancel',
                    actionMethod: this.cancel,
                    styleClass: 'btn-width-height p-button-raised p-button-danger  p-mb-2',
                    iconClass: 'p-button-raised p-button-danger'
                }
            ]
        }
    }
    Salaries: any[] = [];
    loadEmployeeData(employeeID: number) {
        this.employeeService.getEmployeeByIdAsync(employeeID).subscribe(result => {
            if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                if (result['data'].employeeID !== 0) {
                    this.Salaries = result['data'].salaries
                }
            }
        }, error => {
            //toastService.ShowError();
        })
    }

    //public void OnSalaryChangeBasicPayOnInput(ChangeEventArgs e) {
    //    var value = this.SalaryForm.value.BasicPay;

    //}
    OnSalaryBasicPay() {
        var BasicPay: any = this.SalaryForm.value.BasicPay !== null ? parseFloat(this.SalaryForm.value.BasicPay) : 0.00;
        if (BasicPay !== 0.00) {
            return parseFloat(BasicPay);
        } else {
            return BasicPay;
        }
    }
    OnSalaryChangeBasicPay() {
        var BasicPay: any = this.SalaryForm.value.BasicPay !== null ? parseFloat(this.SalaryForm.value.BasicPay) : 0.00;
        if (BasicPay !== 0) {
            this.SalaryBasic = (parseFloat(BasicPay) * 12);
            return this.SalaryBasic;
        } else {
            this.SalaryBasic = 0.00
            return this.SalaryBasic;
         
        }
    }

    OnSalaryHRA() {
        var Hra: any = this.SalaryForm.value.HRA !== null ? parseFloat(this.SalaryForm.value.HRA) : 0;
        if (Hra != 0) {
            return parseFloat(Hra);
        }
        else {
            return Hra;
        }
    }

    OnSalaryChangeHRA() {
        var Hra: any = this.SalaryForm.value.HRA !== null ? parseFloat(this.SalaryForm.value.HRA) : 0;
        if (Hra != 0) {
            this.SalaryHRA = (parseFloat(Hra) * 12);
            return this.SalaryHRA;
        }
        else {
            this.SalaryHRA = 0.00;
            return this.SalaryHRA;
        }
    }

    OnSalaryLTA() {
        var Lta: any = this.SalaryForm.value.LTA !== null ? parseFloat(this.SalaryForm.value.LTA) : 0;
        if (Lta !== 0) {
            return parseFloat(Lta);
        }
        else {
            return parseFloat(Lta);
        }
    }
     OnSalaryChangeLTA() {
         var Lta: any = this.SalaryForm.value.LTA !== null ? parseFloat(this.SalaryForm.value.LTA) : 0;
        if (Lta !== 0) {
            this.SalaryLTA = parseFloat(Lta) * 12;
            return this.SalaryLTA;
        }
        else {
            this.SalaryLTA = 0.00;
            return this.SalaryLTA;
        }
     }


    OnSalaryEducationAllowance() {
        var educationAllowance: any = this.SalaryForm.value.EducationAllowance !== null ? parseFloat(this.SalaryForm.value.EducationAllowance) : 0;;
        if (educationAllowance !== 0) {
            return parseFloat(educationAllowance);

        }
        else {
            return parseFloat(educationAllowance);
        }
    }
   OnSalaryChangeEducationAllowance() {
       var educationAllowance: any = this.SalaryForm.value.EducationAllowance !== null ? parseFloat(this.SalaryForm.value.EducationAllowance) : 0;;
       if (educationAllowance !== 0) {
            this.SalaryEducationAllowance = parseFloat(educationAllowance) * 12;
            return this.SalaryEducationAllowance;
        }
        else {
            this.SalaryEducationAllowance = 0.00;
           return this.SalaryEducationAllowance;
        }
   }


    OnSalarySpecialAllowance() {
        var specialAllowance: any = this.SalaryForm.value.SpecialAllowance !== null ? parseFloat(this.SalaryForm.value.SpecialAllowance) : 0;
        if (specialAllowance !== 0) {
            return parseFloat(specialAllowance);
        }
        else {
            return parseFloat(specialAllowance);
        }
    }
    OnSalaryChangeSpecialAllowance() {
        var specialAllowance: any = this.SalaryForm.value.SpecialAllowance !== null ? parseFloat(this.SalaryForm.value.SpecialAllowance) : 0;
        if (specialAllowance !== 0) {
            this.SalarySpecialAllowance = parseFloat(specialAllowance) * 12;
            return this.SalarySpecialAllowance;
        }
        else {
            this.SalarySpecialAllowance = 0.00;
            return this.SalarySpecialAllowance;
        }
    }

    OnSalaryBonus() {
        var bonus: any = this.SalaryForm.value.Bonus !== null ? parseFloat(this.SalaryForm.value.Bonus) : 0;
        if (bonus != 0) {
            return parseFloat(bonus);
        } else {
            return parseFloat(bonus);
        }
    }
   OnSalaryChangeBonus() {
       var bonus: any = this.SalaryForm.value.Bonus !== null ? parseFloat(this.SalaryForm.value.Bonus) : 0;
       if (bonus != 0) {
           this.Salarybonus = parseFloat(bonus) * 12;
            return this.Salarybonus;
        }
        else {
           this.Salarybonus = 0.00;
           return this.Salarybonus;
        }
   }


    OnSalaryChangeMedicalInsurance() {
        var medicalInsurance: any = this.SalaryForm.value.MedicalInsurance != null ? parseFloat(this.SalaryForm.value.MedicalInsurance) : 0;
        if (medicalInsurance != 0) {
            this.SalaryMedicalInsurance = parseFloat(medicalInsurance);
            return this.SalaryMedicalInsurance;
        }
        else {
           this.SalaryMedicalInsurance = 0.00;
            return this.SalaryMedicalInsurance;
        }
    }

     OnSalaryChangeVariablePay() {
         var variablePay: any = this.SalaryForm.value.VariablePay != null ? parseFloat(this.SalaryForm.value.VariablePay) : 0;
         if (variablePay != 0) {
             this.SalaryVariablePay = parseFloat(variablePay);
            return this.SalaryVariablePay;
        }
        else {
            this.SalaryVariablePay = 0.00;
             return this.SalaryVariablePay;
        }
     }

    OnSalaryMealAllowance() {
        var mealAllowance: any = this.SalaryForm.value.MealAllowance != null ? parseFloat(this.SalaryForm.value.MealAllowance) : 0;
        if (mealAllowance != 0) {
            return parseFloat(mealAllowance);
        }
        else {
            return parseFloat(mealAllowance);
        }
    }

     OnSalaryChangeMealAllowance() {
         var mealAllowance: any = this.SalaryForm.value.MealAllowance != null ? parseFloat(this.SalaryForm.value.MealAllowance) : 0;
         if (mealAllowance != 0) {
            this.SalaryMealAllowance = parseFloat(mealAllowance) * 12;
            return this.SalaryMealAllowance;
        }
        else {
            this.SalaryMealAllowance = 0.00;
             return this.SalaryMealAllowance = 0.00;
        }
     }

   OnSalaryTelephoneAllowance() {
       var telephoneAllowance: any = this.SalaryForm.value.TelephoneAllowance !== null ? parseFloat(this.SalaryForm.value.TelephoneAllowance) : 0;
        if (telephoneAllowance !== 0) {
            return parseFloat(telephoneAllowance);
        }
        else {
            return parseFloat(telephoneAllowance);
        }
    }
    OnSalaryChangeTelephoneAllowance() {
        var telephoneAllowance: any = this.SalaryForm.value.TelephoneAllowance !== null ? parseFloat(this.SalaryForm.value.TelephoneAllowance) : 0;
        if (telephoneAllowance !== 0) {
            this.SalaryTelephoneAllowance = parseFloat(telephoneAllowance) * 12;
            return this.SalaryTelephoneAllowance;
        }
        else {
            this.SalaryTelephoneAllowance = 0.00;
            return this.SalaryTelephoneAllowance;
        }
    }

    OnSalaryConveyance() {
        var conveyance: any = this.SalaryForm.value.Conveyance != null ? parseFloat(this.SalaryForm.value.Conveyance) : 0;
        if (conveyance != 0) {
            this.SalaryConveyance = parseFloat(conveyance);
            return this.SalaryConveyance;
        }
        else {
            this.SalaryConveyance = 0.00;
            return this.SalaryConveyance;
        }
    }
   OnSalaryChangeConveyance() {
       var conveyance: any = this.SalaryForm.value.Conveyance != null ? parseFloat(this.SalaryForm.value.Conveyance) : 0;
       if (conveyance != 0) {
            this.SalaryConveyance = parseFloat(conveyance) * 12;
            return this.SalaryConveyance;
        }
        else {
            this.SalaryConveyance = 0.00;
           return this.SalaryConveyance;
        }
   }


    OnSalaryMedicalAllowance() {
        var MedicalAllowance: any = this.SalaryForm.value.MedicalAllowance != null ? parseFloat(this.SalaryForm.value.MedicalAllowance) : 0;
        if (MedicalAllowance != 0) {
            this.SalaryMedicalAllowance = parseFloat(MedicalAllowance);
            return this.SalaryMedicalAllowance;
        }
        else {
            this.SalaryMedicalAllowance = 0.00;
            return this.SalaryMedicalAllowance;
        }
    }

    OnSalaryChangeMedicalAllowance() {
        var medicalAllowance: any = this.SalaryForm.value.MedicalAllowance != null ? parseFloat(this.SalaryForm.value.MedicalAllowance) : 0;
        if (medicalAllowance != 0) {
            this.SalaryMedicalAllowance = parseFloat(medicalAllowance) * 12;
            return this.SalaryMedicalAllowance;
        }
        else {
            this.SalaryMedicalAllowance = 0.00;
            return this.SalaryMedicalAllowance;
        }
    }

    GetEmployeeTotalGrossWagePM() {
        var BasicPay = this.SalaryForm.value.BasicPay !== null ?  parseFloat(this.SalaryForm.value.BasicPay) : 0;
        var HRA = this.SalaryForm.value.HRA !== null ? parseFloat(this.SalaryForm.value.HRA) : 0;
        var LTA = this.SalaryForm.value.LTA !== null ? parseFloat(this.SalaryForm.value.LTA) : 0;
        var EducationAllowance = this.SalaryForm.value.EducationAllowance !== null ? parseFloat(this.SalaryForm.value.EducationAllowance) : 0;
        var SpecialAllowance = this.SalaryForm.value.SpecialAllowance !== null ? parseFloat(this.SalaryForm.value.SpecialAllowance) : 0;
        var Bonus = this.SalaryForm.value.Bonus !== null ? parseFloat(this.SalaryForm.value.Bonus) : 0;
        var MealAllowance = this.SalaryForm.value.MealAllowance !== null ? parseFloat(this.SalaryForm.value.MealAllowance) : 0;
        var TelephoneAllowance = this.SalaryForm.value.TelephoneAllowance !== null ? parseFloat(this.SalaryForm.value.TelephoneAllowance) : 0;
        var Conveyance = this.SalaryForm.value.Conveyance !== null ? parseFloat(this.SalaryForm.value.Conveyance) : 0;
        var MedicalAllowance = this.SalaryForm.value.MedicalAllowance !== null ? parseFloat(this.SalaryForm.value.MedicalAllowance) : 0;
        var TotalEmployeeGrossWagePM: any = BasicPay + HRA + LTA + EducationAllowance + SpecialAllowance + Bonus + MealAllowance + TelephoneAllowance + Conveyance + MedicalAllowance;
        return parseFloat(TotalEmployeeGrossWagePM);
    }

    GetEmployeeTotalGrossWagePA() {
        var TotalEmployeeGrossWagePA: any = this.SalaryBasic + this.SalaryHRA + this.SalaryLTA + this.SalaryEducationAllowance + this.SalarySpecialAllowance + this.Salarybonus + this.SalaryVariablePay + this.SalaryMealAllowance + this.SalaryTelephoneAllowance + this.SalaryConveyance + this.SalaryMedicalAllowance + this.SalaryMedicalInsurance;
        return parseFloat(TotalEmployeeGrossWagePA);
    }
   // ProvidentFund: any = 0;
    OnSalaryProvidentFund() {
        var providentFund: any = this.SalaryForm.value.ProvidentFund !== null ? parseFloat(this.SalaryForm.value.ProvidentFund) : 0;
        if (providentFund != 0) {
            return parseFloat(providentFund);
        }
        else {
            return parseFloat(providentFund);
        }
    }
    OnSalaryChangeProvidentFund() {
        var providentFund: any = this.SalaryForm.value.ProvidentFund != "" ? parseFloat(this.SalaryForm.value.ProvidentFund) : 0;
        if (providentFund != 0) {
            this.ProvidentFund = parseFloat(providentFund) * 12;
            return this.ProvidentFund;
        }
        else {
            this.ProvidentFund = 0.00;
            return this.ProvidentFund;
        }
    }

    OnSalaryGratuity() {
        var gratuity:any = this.SalaryForm.value.Gratuity != "" ? parseFloat(this.SalaryForm.value.Gratuity) : 0;
        if (gratuity != 0) {
            return parseFloat(gratuity);
        }
        else {
            return parseFloat(gratuity);
        }
    }
    OnSalaryChangeGratuity() {
        var gratuity: any = this.SalaryForm.value.Gratuity != "" ? parseFloat(this.SalaryForm.value.Gratuity) : 0;
        if (gratuity != 0) {
            this.SalaryGratuity = parseFloat(gratuity) * 12;
            return this.SalaryGratuity;
        }
        else {
            this.SalaryGratuity = 0.00;
            return this.SalaryGratuity;
        }
    }

    TotalCTCPM: any;
    GetTotalCostToCompanyPM() {
        var BasicPay = this.SalaryForm.value.BasicPay !== null ? parseFloat(this.SalaryForm.value.BasicPay) : 0;
        var HRA = this.SalaryForm.value.HRA !== null ? parseFloat(this.SalaryForm.value.HRA) : 0;
        var LTA = this.SalaryForm.value.LTA !== null ? parseFloat(this.SalaryForm.value.LTA) : 0;
        var EducationAllowance = this.SalaryForm.value.EducationAllowance !== null ? parseFloat(this.SalaryForm.value.EducationAllowance) : 0;
        var SpecialAllowance = this.SalaryForm.value.SpecialAllowance !== null ? parseFloat(this.SalaryForm.value.SpecialAllowance) : 0;
        var Bonus = this.SalaryForm.value.Bonus !== null ? parseFloat(this.SalaryForm.value.Bonus) : 0;
        var MealAllowance = this.SalaryForm.value.MealAllowance !== null ? parseFloat(this.SalaryForm.value.MealAllowance) : 0;
        var TelephoneAllowance = this.SalaryForm.value.TelephoneAllowance !== null ? parseFloat(this.SalaryForm.value.TelephoneAllowance) : 0;
        var Conveyance = this.SalaryForm.value.Conveyance !== null ? parseFloat(this.SalaryForm.value.Conveyance) : 0;
        var MedicalAllowance = this.SalaryForm.value.MedicalAllowance !== null ? parseFloat(this.SalaryForm.value.MedicalAllowance) : 0;
        var ProvidentFund = this.SalaryForm.value.ProvidentFund !== null ? parseFloat(this.SalaryForm.value.ProvidentFund) : 0;
        var Gratuity = this.SalaryForm.value.Gratuity !== null ? parseFloat(this.SalaryForm.value.Gratuity) : 0;
        var totalCostToCompanyPM: any = BasicPay + HRA + LTA + EducationAllowance + SpecialAllowance + Bonus + MealAllowance + TelephoneAllowance + Conveyance + MedicalAllowance + ProvidentFund + Gratuity;
        this.TotalCTCPM = totalCostToCompanyPM;
        this.SalaryForm.controls.TotalCTCPM.patchValue(parseFloat(totalCostToCompanyPM))
        return parseFloat(totalCostToCompanyPM);
    }

    GettotalCostToCompanyPA() {
        var totalCostToCompanyPA: any = this.SalaryBasic + this.SalaryHRA + this.SalaryLTA + this.SalaryEducationAllowance + this.SalarySpecialAllowance + this.Salarybonus + this.SalaryVariablePay + this.SalaryMealAllowance + this.SalaryTelephoneAllowance + this.SalaryConveyance + this.SalaryMedicalAllowance + this.ProvidentFund + this.SalaryGratuity + this.SalaryMedicalInsurance;
        this.SalaryForm.controls.CostToCompany.patchValue(parseFloat(totalCostToCompanyPA))
        //this.SalaryForm.value.CostToCompany = totalCostToCompanyPA;
        return parseFloat(totalCostToCompanyPA);
    }

    ErrorMessage: string;

    save = () => {
        this.submitted = true;
        if (this.SalaryForm.invalid) {
            this.SalaryForm.markAllAsTouched();
            return;
        } else {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.ErrorMessage = "";
                if (this.Salaries.length === 0) {
                    //this.SalaryFormConvertDecimal();
                    this.SaveSalary();
                } else {
                    var salaryStartDate = new Date(moment(this.SalaryForm.value.StartDate).format('MM/DD/YYYY').toString())

                    var currentRec = this.Salaries.find(x => x.salaryID === this.SalaryId);
                    console.log(currentRec);
                    if (currentRec === null || currentRec === undefined || currentRec.endDate !== null) {
                        if (this.SalaryForm.value.EndDate === null) {
                            this.SalaryForm.value.EndDate = '12/31/9999'
                        }
                        var salaryEndDate = new Date(moment(this.SalaryForm.value.EndDate).format('MM/DD/YYYY').toString())

                        var salaryAlreadyexist: boolean = this.Salaries.findIndex(x => ((new Date(x.startDate) <= salaryStartDate && x.endDate === null) ||
                            (new Date(x.startDate) <= salaryEndDate && salaryStartDate <= new Date(x.endDate)) ||
                            (new Date(x.startDate) >= salaryStartDate && new Date(x.endDate) <= salaryEndDate) ||
                            (new Date(x.startDate) >= salaryStartDate && new Date(x.endDate) == null) ||
                            (new Date(x.startDate) <= salaryStartDate && new Date(x.endDate) >= salaryStartDate))) > -1;
                        if (salaryAlreadyexist) {
                            this.ErrorMessage = "Salary already exists for this period";
                            if (this.SalaryForm.value.EndDate === '12/31/9999') { this.SalaryForm.value.EndDate = null; }
                        }
                        else {
                          //  this.SalaryFormConvertDecimal();
                            this.SaveSalary();
                        }
                    }
                    else {
                       // this.SalaryFormConvertDecimal();
                        this.SaveSalary();
                    }
                }
            }
            this.isSaveButtonDisabled = false;
        }

    }
    SaveSalary() {
        if (this.SalaryForm.value.BasicPay !== null && this.SalaryForm.value.BasicPay !== undefined) {
            this.SalaryForm.value.BasicPay = this.SalaryForm.value.BasicPay.toString();
        }
        if (this.SalaryForm.value.HRA !== null && this.SalaryForm.value.HRA !== undefined) {
            this.SalaryForm.value.HRA = this.SalaryForm.value.HRA.toString();
        }
        if (this.SalaryForm.value.Bonus !== null && this.SalaryForm.value.Bonus !== undefined) {
            this.SalaryForm.value.Bonus = this.SalaryForm.value.Bonus.toString();
        }
        if (this.SalaryForm.value.LTA !== null && this.SalaryForm.value.LTA !== undefined) {
            this.SalaryForm.value.LTA = this.SalaryForm.value.LTA.toString();
        }
        if (this.SalaryForm.value.EducationAllowance !== null && this.SalaryForm.value.EducationAllowance !== undefined) {
            this.SalaryForm.value.EducationAllowance = this.SalaryForm.value.EducationAllowance.toString();
        }
        if (this.SalaryForm.value.VariablePay !== null && this.SalaryForm.value.VariablePay !== undefined) {
            this.SalaryForm.value.VariablePay = this.SalaryForm.value.VariablePay.toString();
        }
        if (this.SalaryForm.value.SpecialAllowance !== null && this.SalaryForm.value.SpecialAllowance !== undefined) {
            this.SalaryForm.value.SpecialAllowance = this.SalaryForm.value.SpecialAllowance.toString();
        }
        if (this.SalaryForm.value.ProvidentFund !== null && this.SalaryForm.value.ProvidentFund !== undefined) {
            this.SalaryForm.value.ProvidentFund = this.SalaryForm.value.ProvidentFund.toString();
        }
        if (this.SalaryForm.value.TelephoneAllowance !== null && this.SalaryForm.value.TelephoneAllowance !== undefined) {
            this.SalaryForm.value.TelephoneAllowance = this.SalaryForm.value.TelephoneAllowance.toString();
        }
        if (this.SalaryForm.value.MedicalInsurance !== null && this.SalaryForm.value.MedicalInsurance !== undefined) {
            this.SalaryForm.value.MedicalInsurance = this.SalaryForm.value.MedicalInsurance.toString();
        }
        if (this.SalaryForm.value.MedicalAllowance !== null && this.SalaryForm.value.MedicalAllowance !== undefined) {
            this.SalaryForm.value.MedicalAllowance = this.SalaryForm.value.MedicalAllowance.toString();
        }
        if (this.SalaryForm.value.MealAllowance !== null && this.SalaryForm.value.MealAllowance !== undefined) {
            this.SalaryForm.value.MealAllowance = this.SalaryForm.value.MealAllowance.toString();
        }
        if (this.SalaryForm.value.Conveyance !== null && this.SalaryForm.value.Conveyance !== undefined) {
            this.SalaryForm.value.Conveyance = this.SalaryForm.value.Conveyance.toString();
        }
        if (this.SalaryForm.value.Gratuity !== null && this.SalaryForm.value.Gratuity !== undefined) {
            this.SalaryForm.value.Gratuity = this.SalaryForm.value.Gratuity.toString();
        }
        if (this.SalaryForm.value.CostToCompany !== null && this.SalaryForm.value.CostToCompany !== undefined) {
            this.SalaryForm.value.CostToCompany = this.SalaryForm.value.CostToCompany.toString();
        }
        console.log(this.SalaryForm.value)
        //this.SalaryForm.value.StartDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.SalaryForm.value.StartDate));
        //if (this.SalaryForm.value.EndDate !== null) {
        //    this.SalaryForm.value.EndDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.SalaryForm.value.EndDate));
        //}
        if (this.SalaryId === 0) {
            this.salaryService.SaveSalary(this.SalaryForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'Salary saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
        }
        else if (this.SalaryId != 0) {
            this.salaryService.UpdateSalary(this.SalaryForm.value).subscribe(result => {
                if (result['data'] !== null && result['messageType'] === 1) {
                    this.messageService.add({ severity: 'success', summary: 'Salary saved successfully', detail: '' });
                    this.isSaveButtonDisabled = true;
                    this.loadModalOptions();
                    this.ListValueUpdated.emit();
                    this.cancel();
                } else {
                    this.isSaveButtonDisabled = false;
                    this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                }
            })
          
        }
    }
    cancel = () => {
        this.ErrorMessage = '';
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildSalaryForm({}, 'New');
        this.addEditSalaryModal.hide();
    }
    ResetDialog(){
        this.ErrorMessage = '';
        this.submitted = false;
        this.isSaveButtonDisabled = false;
        this.buildSalaryForm({}, 'New');
        this.addEditSalaryModal.hide();
    }
}
