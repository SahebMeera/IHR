import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { CommonUtils } from '../../../common/common-utils';
import { Constants, Countries, EmployeeAddresses, ErrorMsg, ListTypeConstants, SessionConstants } from '../../../constant';
import { IEmployee, IEmployeeDisplay } from '../../../core/interfaces/Employee';
import { IEmployeeAddress } from '../../../core/interfaces/EmployeeAddress';
import { IEmployeeAssignmentDisplay } from '../../../core/interfaces/EmployeeAssignment';
import { IModuleDisplay } from '../../../core/interfaces/Module';
import { IRolePermissionDisplay } from '../../../core/interfaces/RolePermission';
import { DataProvider } from '../../../core/providers/data.provider';
import { HolidayService } from '../../holidays/holidays.service';
import { LookUpService } from '../../lookup/lookup.service';
import { RolePermissionService } from '../../role-permission/role-permission.service';
import { DirectDepositComponent } from '../direct-deposit/direct-deposit.component';
import { EmployeeAssignmentComponent } from '../employee-assignment/employee-assignment.component';
import { EmployeeDependentComponent } from '../employee-dependent/employee-dependent.component';
import { EmployeeEmergencyComponent } from '../employee-emergency/employee-emergency.component';
import { EmployeeSkillComponent } from '../employee-skill/employee-skill.component';
import { EmployeeW4Component } from '../employee-w4/employee-w4.component';
import { EmployeeW4Service } from '../employee-w4/w4.service';
import { EmployeeService } from '../employee.service';
import { AddEditAddressComponent } from './add-edit-address/add-edit-address.component';

@Component({
  selector: 'app-employee-details',
  templateUrl: './employee-details.component.html',
  styleUrls: ['./employee-details.component.scss']
})
export class EmployeeDetailsComponent implements OnInit {
    pattern = { '0': { pattern: new RegExp('\\d'), symbol: '*' } };
    countries = Countries;
    commonUtils = new CommonUtils()
    myDate = new Date();
    RolePermissions: IRolePermissionDisplay[] = [];
    EmployeeRolePermission: IRolePermissionDisplay;
    user: any;
    isEditMode: boolean = false;
    canUpdate: boolean;
    employee: IEmployeeDisplay
    employeeDetails: IEmployeeDisplay;
    EmpEmail: string = '';
    EmpWorkEmail: string = '';
    isShowW4I9Info: boolean = false;
    
    EmployeeAssigmentRolePermission: IRolePermissionDisplay;
    EmployeeInfoRolePermission: IRolePermissionDisplay;
    W4InfoPermission: IRolePermissionDisplay;
    I9InfoPermission: IRolePermissionDisplay;
    SalaryInfoPermission: IRolePermissionDisplay;
    SKillInfoPermission: IRolePermissionDisplay;
    NPIPermission: IRolePermissionDisplay;

    dataProviderCountry: string;
    status: string;
    employeeType: string;

    constructor(private fb: FormBuilder, private dataProvider: DataProvider,
        private holidayService: HolidayService,
        private LookupService: LookUpService,
        private messageService: MessageService,
        private router: Router,
        private activeRoute: ActivatedRoute,
        private rolePermissionService: RolePermissionService,
        private employeeService: EmployeeService) {
        this.RolePermissions = [];
        this.RolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
        this.EmployeeRolePermission = this.RolePermissions.length > 0 ? this.RolePermissions.find(usr => usr.moduleShort == Constants.EMPLOYEE): null;

        this.EmployeeAssigmentRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.ASSIGNMENT);
        this.EmployeeInfoRolePermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.EMPLOYEEINFO);
        this.W4InfoPermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.W4INFO);
        this.I9InfoPermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.I9INFO);
        this.SalaryInfoPermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.SALARY);
        this.SKillInfoPermission = this.RolePermissions.find(usr => usr.moduleShort === Constants.SKILL);
        this.NPIPermission = this.RolePermissions.find(usr => usr.moduleShort == Constants.NPI);

        this.user = JSON.parse(localStorage.getItem("User"));
        this.LoadDropDown();
        this.bulidEmployeeAddEditForm({}, 'New');
        if (this.dataProvider.storage) {
            this.isEditMode = this.dataProvider.storage["isEditMode"]

            if (this.isEditMode) {
                var employeeDetails = this.dataProvider.storage["employeeDetails"]
                if (employeeDetails !== null && employeeDetails !== undefined) {
                    this.EmployeeID = employeeDetails.employeeID;
                    this.loadEmployeeDetails(employeeDetails.employeeID)
                }
            } else {
                this.isEditPermissionForNPIRole = false;
                this.isViewPermissionForNPIRole = false;
                this.EmployeeID = 0
                this.isShowW4I9Info = false;
                this.canUpdate = this.EmployeeRolePermission.add;
            }
        }


    }
    EmployeeAssignments: IEmployeeAssignmentDisplay[] = [];
    isViewPermissionForNPIRole: boolean = false;
    isEditPermissionForNPIRole: boolean = false;
    loadEmployeeDetails(empID: number) {
        if (empID !== undefined && empID !== null && empID !== 0) {
            this.employeeService.getEmployeeByIdAsync(empID).subscribe(result => {
                if (result !== undefined && result['data'] !== null && result['data'] !== undefined) {
                    this.employeeDetails = result['data'];
                    this.EmployeeAssignments = result['data'].assignments !== null ? result['data'].assignments : []
                    this.employeeAddressChangesCaps(this.employeeDetails)
                    this.employeeCode = this.employeeDetails.employeeCode;
                    this.Title = this.employeeDetails.title;
                    this.Department = this.employeeDetails.department;
                    this.WorkAuthorization = this.employeeDetails.workAuthorization;
                    this.EmploymentType = this.employeeDetails.employmentType;
                    this.EmpEmail = this.employeeDetails.email;
                    this.EmpWorkEmail = this.employeeDetails.workEmail;

                    this.isViewPermissionForNPIRole = this.NPIPermission.view;
                    this.isEditPermissionForNPIRole = this.NPIPermission.update;
                    
                    if (this.isEditMode && this.isEditPermissionForNPIRole && (this.isViewPermissionForNPIRole === false)) {
                        if (this.employeeDetails['ssn'] === '') {
                            this.isViewPermissionForNPIRole = true;
                        }
                    }
                   
                    this.bulidEmployeeAddEditForm(this.employeeDetails, 'Edit');
                    if (this.employeeDetails !== null && this.employeeDetails.country !== null && this.employeeDetails.country.toUpperCase() === Countries.UNITEDSTATES) {
                        this.isShowW4I9Info = false;
                    }
                    else {
                        this.isShowW4I9Info = true;
                    }
                    this.country = this.employeeDetails.country;
                    this.holidayService.getCountry().subscribe(resp => {
                        this.CountryList = resp['data'];
                        if (this.employeeDetails.country !== null) {
                            this.GetStates(this.employeeDetails.country);
                        } else {
                            this.StateList = [];
                        }
                    })
                    this.setEmployeeAddresses();
                }
            })
        }
        this.canUpdate = this.EmployeeRolePermission.update;
    }

    employeeAddressChangesCaps(employeeDetails: IEmployeeDisplay) {
        var employeeAddres: IEmployeeAddress[] = [];
        if (employeeDetails !== undefined && employeeDetails !== null) {
            var addressDisplay: any[] = [];
            addressDisplay = employeeDetails.employeeAddresses;
            this.employeeDetails.employeeAddresses = [];
            var address: IEmployeeAddress;
            addressDisplay.forEach(x => {
                if (x !== null && x !== undefined) {
                    address = {
                        EmployeeAddressID: x.employeeAddressID,
                        EmployeeID: x.employeeID,
                        AddressTypeID: x.addressTypeID,
                        Address1: x.address1,
                        Address2: x.address2,
                        City: x.city,
                        State: x.state,
                        Country: x.country,
                        ZipCode: x.zipCode,
                        StartDate: x.startDate,
                        EndDate: x.endDate,
                        EmployeeName: x.employeeName,
                        CreatedBy: x.createdBy,
                        CreatedDate: x.createdDate,
                        ModifiedBy: x.modifiedBy,
                        ModifiedDate: x.modifiedDate,
                        TimeStamp: x.timeStamp
                    }
                    this.employeeDetails.employeeAddresses.push(address)
                }
            })
        }
    }

    ngOnInit(): void {
        if (!this.dataProvider.storage) {
            this.router.navigate(['../'], { relativeTo: this.activeRoute })
        }
     
  }
    moduleList: IModuleDisplay[] = [];
    ManagerList: any[] = [];
    GenderList: any[] = [];
    EmployMentList: any[] = [];
    TitleList: any[] = [];
    WorkAuthorizationList: any[] = [];
    MaritalStautsList: any[] = [];
    WithHoldingList: any[] = [];
    AddressTypeList: any[] = [];
    DepartmentList: any[] = [];
    CountryList: any[] = [];
    StateList: any[] = [];

    employeeCode: string = 'SYSGENERATED';
    isCoun
    LoadDropDown() {
        if (this.isEditMode === false) {
            this.employeeCode = "SYSGENERATED";
        }
        this.rolePermissionService.getModules().subscribe(result => {
            this.moduleList = result;
        });
        this.employeeService.GetEmployees().subscribe(respEmployee => {
            if (respEmployee['messageType'] !== null && respEmployee['messageType'] === 1) {
                this.ManagerList = respEmployee['data']
            }
        })
        this.LookupService.getListValues().subscribe(resp => {
            if (resp['data'] !== undefined && resp['data'] !== null) {
                this.GenderList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.GENDER);
                this.EmployMentList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.EMPLOYMENTTYPE);
                this.TitleList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.TITLE);
                this.WorkAuthorizationList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.WORKAUTHORIZATION);
                this.MaritalStautsList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.MARITALSTATUS);
                this.WithHoldingList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.WITHHOLDINGSTATUS);
                this.AddressTypeList = resp['data'].filter(x => x.type.toUpperCase() == ListTypeConstants.ADDRESSTYPE);
                this.SetAddressTypeID();
                if (this.isEditMode === true) {
                    this.setEmployeeAddresses();
                }
            }
        })
        this.SetAddressTypeID();
        this.employeeService.GetDepartments().subscribe(resp => {
            this.DepartmentList = resp['data'];
        })
        this.holidayService.getCountry().subscribe(resp => {
            this.CountryList = resp['data'];
            if (this.isEditMode === false) {
                this.bulidEmployeeAddEditForm({}, 'New');
            }
        })
        if (this.country != null) {
            // await GetStates(Employee.Country);
        }
        else {
            this.StateList = [];
        }
    }
    country: string;
    countryId: number;
   
    EmployeeForm: FormGroup;
    submitted: boolean
    bulidEmployeeAddEditForm(data: any, keyName: string) {
        this.EmployeeForm = this.fb.group({
            EmployeeID: [keyName === 'New' ? 0 : data.employeeID],
            EmployeeCode: [keyName === 'New' ? 'SYSGENERATED' : data.employeeCode, Validators.required],
            FirstName: [keyName === 'New' ? '' : data.firstName, Validators.required],
            MiddleName: [keyName === 'New' ? '' : data.middleName],
            LastName: [keyName === 'New' ? '' : data.lastName, Validators.required],
            Country: [keyName === 'New' ? null : data.country, Validators.required],
            TitleID: [keyName === 'New' ? null : data.titleID, Validators.required],
            Title: [keyName === 'New' ? '' : data.title],
            GenderID: [keyName === 'New' ? null : data.genderID, Validators.required],
            Gender: [keyName === 'New' ? '' : data.gender],
            DepartmentID: [keyName === 'New' ? null : data.departmentID, Validators.required],
            Department: [keyName === 'New' ? '' : data.department],
            Phone: [keyName === 'New' ? null : data.phone, Validators.required],
            HomePhone: [keyName === 'New' ? null : data.homePhone],
            WorkPhone: [keyName === 'New' ? null : data.workPhone],
            Email: [keyName === 'New' ? '' : data.email, [Validators.required,Validators.email]],
            WorkEmail: [keyName === 'New' ? '' : data.workEmail, Validators.email],
            BirthDate: [keyName === 'New' ? null : new Date(data.birthDate), Validators.required],
            HireDate: [keyName === 'New' ? null : new Date(data.hireDate), Validators.required],
            TermDate: [keyName === 'New' ? null : data.termDate !== null ? new Date(data.termDate): null],
            WorkAuthorizationID: [keyName === 'New' ? null : data.workAuthorizationID, Validators.required],
            WorkAuthorization: [keyName === 'New' ? '' : data.workAuthorization],
            SSN: [keyName === 'New' ? '' : data.ssn],
            PAN: [keyName === 'New' ? '' : data.pan],
            AadharNumber: [keyName === 'New' ? '' : data.aadharNumber],
            Salary: [keyName === 'New' ? 0 : data.salary],
            VariablePay: [keyName === 'New' ? 0 : data.variablePay],
            MaritalStatusID: [keyName === 'New' ? null : data.maritalStatusID, Validators.required],
            MaritalStatus: [keyName === 'New' ? '' : data.maritalStatus],
            ManagerID: [keyName === 'New' ? null : data.managerID],
            Manager: [keyName === 'New' ? '' : data.manager],
            EmploymentTypeID: [keyName === 'New' ? null : data.employmentTypeID, Validators.required],
            EmploymentType: [keyName === 'New' ? '' : data.employmentType],
            CreatedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : this.user.firstName + " " + this.user.lastName],
            CreatedDate: [keyName === 'New' ? new Date() : data.createdDate],
            ModifiedDate: [keyName === 'New' ? new Date() : data.modifiedDate],
            ModifiedBy: [keyName === 'New' ? this.user.firstName + " " + this.user.lastName : data.modifiedBy],
            TimeStamp: [''],
            IsDeleted: false,
            EmployeeAddresses: [keyName === 'New' ? [] : data.employeeAddresses === null ? [] : data.employeeAddresses]
        });
        if (keyName === 'New' && this.CountryList.length > 0) {
            this.EmployeeForm.controls.Country.patchValue(this.CountryList.find(x => x.countryDesc.toUpperCase() === Countries.UNITEDSTATES).countryDesc);
            this.country = this.EmployeeForm.value.Country;

        }
       
    }
    get addEditEmployeeFormControls() { return this.EmployeeForm.controls; }
    EmployeeName: string = '';
    Title: string = '';
    Department: string = '';
    WorkAuthorization: string = '';
    EmploymentType: string = '';
    onChangeEmployeeCountry(event) {
        this.country = event.value;
        if (event.value !== null && this.CountryList != null) {
            this.country = this.CountryList.find(x => x.countryDesc.toLowerCase() === this.country.toLowerCase()).countryDesc;
             this.GetStates(this.country);
        }
        //await GetStates(country);
    }
    GetStates(country: string) {
        this.countryId = this.CountryList.find(x => x.countryDesc.toUpperCase() == this.country.toUpperCase()).countryID;
        if (this.countryId != 0 && this.countryId != null) {
            this.holidayService.GetCountryByIdAsync(this.countryId).subscribe(result => {
                if (result['data'] !== undefined) {
                    this.StateList = result['data'];
                }
            })
        }
    }
    // DropDown OnChange Methods Starts Here
    onDepartmentChange(e : any) {
        if (Number(e.value) != 0 && e.value != null && this.DepartmentList != null) {
            var department = Number(e.value);
            this.Department = this.DepartmentList.find(x => x.departmentID == department).deptName;
        }
    }

    onTitleChange(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.TitleList != null) {
            var title = Number(e.value);
            this.Title = this.TitleList.find(x => x.listValueID == title).valueDesc;
        }
    }
    onWorkAuthorizationChange(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.WorkAuthorizationList != null) {
            var workAuthorization = Number(e.value);
            this.WorkAuthorization = this.WorkAuthorizationList.find(x => x.listValueID == workAuthorization).valueDesc;
        }
    }

    onEmployeeTypeChange(e: any) {
        if (Number(e.value) != 0 && e.value != null && this.EmployMentList != null) {
            var employmentType = Number(e.value);
            this.EmploymentType = this.EmployMentList.find(x => x.listValueID == employmentType).valueDesc;
        }
    }
    checkEmailExist() {
        this.ErrorMessage = "";
        this.submitted = true;
        if (this.EmployeeForm.invalid) {
            this.EmployeeForm.markAllAsTouched();
            return;
        } else {
            if (this.EmployeeForm.value.EmployeeID != 0) {
                if ((this.EmpEmail.toUpperCase() !== this.EmployeeForm.value.Email.toUpperCase()) || (this.EmpWorkEmail !== null && this.EmpWorkEmail !== '' && this.EmployeeForm.value.WorkEmail !== '' && this.EmployeeForm.value.WorkEmail !== null && this.EmpWorkEmail.toUpperCase() !== this.EmployeeForm.value.WorkEmail.toUpperCase())) {
                    if ((this.ManagerList.filter(x => x.employeeID !== this.EmployeeForm.value.EmployeeID && x.email.toUpperCase() === this.EmployeeForm.value.Email.toUpperCase()).length > 0 || this.ManagerList.filter(x => x.workEmail !== null && this.EmployeeForm.value.WorkEmail !== null && this.EmployeeForm.value.WorkEmail !== '' && x.employeeID !== this.EmployeeForm.value.EmployeeID && x.workEmail.toUpperCase() === this.EmployeeForm.value.WorkEmail.toUpperCase()).length > 0)) {
                        this.ErrorMessage = "Employee Email already exists in the system";
                    }
                    else {
                        this.SaveEmployee();
                    }
                }
                else {
                    if ((this.ManagerList.filter(x => x.employeeID !== this.EmployeeForm.value.EmployeeID && x.email.toUpperCase() === this.EmployeeForm.value.Email.toUpperCase()).length > 0 || this.ManagerList.filter(x => x.workEmail !== null && this.EmployeeForm.value.WorkEmail !== '' && this.EmployeeForm.value.WorkEmail !== null && x.employeeID !== this.EmployeeForm.value.EmployeeID && x.workEmail.toUpperCase() === this.EmployeeForm.value.WorkEmail.toUpperCase()).length > 0)) {
                        this.ErrorMessage = "Employee Email already exists in the system";
                    } else {
                        this.SaveEmployee();
                    }
                }
            }
            else {
                if ((this.ManagerList.filter(x => x.email.toUpperCase() === this.EmployeeForm.value.Email.toUpperCase()).length > 0 || this.ManagerList.filter(x => x.workEmail != null && this.EmployeeForm.value.WorkEmail !== null && this.EmployeeForm.value.WorkEmail !== '' && x.workEmail.toUpperCase() === this.EmployeeForm.value.WorkEmail.toUpperCase()).length > 0)) {
                    this.ErrorMessage = "Employee Email already exists in the system";
                }
                else {
                    this.SaveEmployee();
                }
            }
        }
    }
     SaveEmployee() {
         
        this.isAddressInvalid = false;
         if (this.currentAddress !== undefined && this.currentAddress.Address1 !== '') {
            this.saveEmployeeDetails();
        }
        else {
            this.isAddressInvalid = true;
        }
       // showSpinner = false;
     }
    saveEmployeeDetails() {
            if (this.isSaveButtonDisabled) {
                return;
            } else {
                this.isSaveButtonDisabled = true;
                this.EmployeeForm.value.BirthDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.EmployeeForm.value.BirthDate));
                this.EmployeeForm.value.HireDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.EmployeeForm.value.HireDate));
                if (this.EmployeeForm.value.TermDate !== null) {
                    this.EmployeeForm.value.TermDate = this.commonUtils.defaultDateTimeLocalSet(new Date(this.EmployeeForm.value.TermDate));
                }
                if (this.EmployeeForm.value.EmployeeID != 0) {
                    if (this.EmployeeForm.value.SSN !== '' && this.EmployeeForm.value.SSN !== null && this.EmployeeForm.value.SSN !== undefined) {
                        this.removeSSNMask();
                    }
                   
                    this.employeeService.UpdateEmployee(this.EmployeeForm.value.EmployeeID, this.EmployeeForm.value).subscribe(result => {
                        if(result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Employee saved successfully', detail: '' });
                            this.LoadAfterSave(result['data'].employeeID)
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                }
                else {
                    if (this.EmployeeForm.value.SSN !== '' && this.EmployeeForm.value.SSN !== null && this.EmployeeForm.value.SSN !== undefined) {
                        this.removeSSNMask();
                    }
                    this.employeeService.SaveEmployee(this.EmployeeForm.value).subscribe(result => {
                        if (result['data'] !== null && result['messageType'] === 1) {
                            this.messageService.add({ severity: 'success', summary: 'Employee saved successfully', detail: '' });
                            //this.loadModalOptions();
                            this.LoadAfterSave(result['data'].employeeID)
                        } else {
                            this.isSaveButtonDisabled = false;
                            this.messageService.add({ severity: 'error', summary: ErrorMsg.ERRORMSG, detail: '' });
                        }
                    })
                }
        }
        this.isSaveButtonDisabled = false;
    }
    removeSSNMask() {
        var ssn = this.EmployeeForm.value.SSN.replace('-', '').replace('-', '');
        this.EmployeeForm.value.SSN = '';
        this.EmployeeForm.value.SSN = ssn;
    }

    Back() {
        this.router.navigate(['employees']);
    }
    @ViewChild('AddEditAddressModal') AddEditAddressModal: AddEditAddressComponent;
    Employee: IEmployeeDisplay;
    currentAddress: IEmployeeAddress;
    permanentAddress: IEmployeeAddress;
    mailingAddress: IEmployeeAddress;
    currentAddressTypeID: number;
    permanentAddressTypeID: number;
    mailingAddressTypeID: number;
    isAddressInvalid: boolean;
    employeeAddresses = EmployeeAddresses ;

    addAddress(AddressType: string) {
        this.SetAddressTypeID();
        if (this.currentAddress != null) {
            this.AddEditAddressModal.currentAddress = this.currentAddress;
        }
        var item = this.AddressTypeList.find(x => x.value === AddressType);
        this.AddEditAddressModal.Show(0, item.listValueID, new IEmployeeAddress(), item.value);
    }
    EditAddress(employeeAddress: IEmployeeAddress) {
        this.AddEditAddressModal.Show(this.EmployeeForm.value.EmployeeID, employeeAddress.AddressTypeID, employeeAddress, "");
    }

    setEmployeeAddresses() {
        if (this.employeeDetails !== undefined && this.employeeDetails.employeeAddresses !== undefined) {
            var currentAdd = this.employeeDetails.employeeAddresses.find(x => x.AddressTypeID == this.currentAddressTypeID);
            if (currentAdd != null) {
                this.currentAddress = currentAdd;
            }
            var permAdd = this.employeeDetails.employeeAddresses.find(x => x.AddressTypeID == this.permanentAddressTypeID);
            if (permAdd != null) {
                this.permanentAddress = permAdd;
            }
            var mailingAdd = this.employeeDetails.employeeAddresses.find(x => x.AddressTypeID == this.mailingAddressTypeID);
            if (mailingAdd != null) {
                this.mailingAddress = mailingAdd;
            }
        }
    }
    SetAddressTypeID() {
        if (this.AddressTypeList.length > 0) {
            this.currentAddressTypeID = this.AddressTypeList.find(x => x.value === EmployeeAddresses.CURRADD).listValueID;
            this.permanentAddressTypeID = this.AddressTypeList.find(x => x.value === EmployeeAddresses.PERMADD).listValueID;
            this.mailingAddressTypeID = this.AddressTypeList.find(x => x.value === EmployeeAddresses.MAILADD).listValueID;
        }
      }
    updateEmpAddressList(employeeAddress: IEmployeeAddress) {
        if (employeeAddress.EmployeeAddressID !== 0) {
            var index = this.employeeDetails.employeeAddresses.findIndex(x => x.EmployeeAddressID === employeeAddress.EmployeeAddressID);
            this.employeeDetails.employeeAddresses[index] = employeeAddress;
            this.setEmployeeAddresses();
        }
        else {
            
            employeeAddress.EndDate = null;
            if (employeeAddress.AddressTypeID === this.currentAddressTypeID) {
                this.currentAddress = employeeAddress;
                this.isAddressInvalid = false;
            } else if (employeeAddress.AddressTypeID === this.permanentAddressTypeID) {
                this.permanentAddress = employeeAddress;
            } else if (employeeAddress.AddressTypeID === this.mailingAddressTypeID) {
                this.mailingAddress = employeeAddress;
            }
            this.EmployeeForm.value.EmployeeAddresses.push(employeeAddress);
        }
    }

    LoadAfterSave(EmpID: number) {
        this.loadEmployeeDetails(EmpID);
    }

    ErrorMessage: string = '';
    isSaveButtonDisabled: boolean = false;
    onEmpManagerChange(e: any) {
        if (e.value != "" && e.value != null && this.ManagerList != null) {
            this.ErrorMessage = "";
            var manager = Number(e.value);
            this.employee = this.ManagerList.find(x => x.employeeID === manager).employeeName;
            if (this.ManagerList.find(x => x.employeeID === manager).termDate != null) {
                this.isSaveButtonDisabled = true;
                this.ErrorMessage = "Please select active manager.";
            } else {
             this.isSaveButtonDisabled = false;
            }
        }
    }

    onchangeTermDate() {
        this.ErrorMessage = "";
        if (this.EmployeeAssignments != null && this.EmployeeAssignments.length > 0) {
            var EmployeeAssignment = this.EmployeeAssignments.find(x => x.endDate === null);
            if (EmployeeAssignment !== null && EmployeeAssignment !== undefined) {
               
                this.ErrorMessage = "Employee has active Assignments. Please terminate Assignments before terminating Employee.";
                this.EmployeeForm.controls.TermDate.patchValue(null)
                //  isSaveButtonDisabled = true;
            } else {
                this.ErrorMessage = "";
            }
        }
    }

    @ViewChild('emergencyModal') emergencyModal: EmployeeEmergencyComponent;
    @ViewChild('directDopositModal') directDopositModal: DirectDepositComponent;
    @ViewChild('dependentModal') dependentModal: EmployeeDependentComponent;
    @ViewChild('employeeW4Modal') employeeW4Modal: EmployeeW4Component;
    @ViewChild('assignmentModal') assignmentModal: EmployeeAssignmentComponent;
    @ViewChild('employeeSkillModal') employeeSkillModal: EmployeeSkillComponent;
    EmployeeID: number;
    tab2() {
        this.emergencyModal.EmployeeID = this.EmployeeID;
        this.emergencyModal.LoadList();
        this.assignmentModal.isExpandRowRate = false;
    }
    tab3() {
        this.directDopositModal.EmployeeID = this.EmployeeID;
        this.assignmentModal.isExpandRowRate = false;
    }
    tab4() {
        this.dependentModal.EmployeeID = this.EmployeeID;
        setTimeout(() => this.assignmentModal.isExpandRowRate = true);
    }
    tab5() {
        this.assignmentModal.EmployeeID = this.EmployeeID;
        this.assignmentModal.isExpandRowRate = false;
    }
    tab6() {
        this.employeeW4Modal.EmployeeID = this.EmployeeID;
        this.assignmentModal.isExpandRowRate = false;
    }
    tab9() {
        this.employeeSkillModal.EmployeeID = this.EmployeeID;
        this.assignmentModal.isExpandRowRate = false;
    }
}
