import { Component, OnInit } from '@angular/core';
import { DataProvider } from 'src/app/core/providers/data.provider';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
//import { IRegistration, IRegistrationDisplay } from 'src/app/core/interfaces/registration';
import { RegistrationService } from '../registration.service';
//import { ConfirmedValidator } from './confirmed.validator';
import * as forge from 'node-forge';
import * as CryptoJS from 'crypto-js';

@Component({
  selector: 'app-personal-details',
  templateUrl: './personal-details.component.html',
  styleUrls: ['./personal-details.component.scss']
})
export class PersonalDetailsComponent implements OnInit {
  personalDetailForm: FormGroup;
    registration: any;
    registrations: any[] = [];
    submitted: boolean = false;
    ErrorMessage: string = '';

    secretKey = "7534275896345678";

  constructor(private dataProvider: DataProvider,
    private fb: FormBuilder, 
    private registrationService: RegistrationService, 
      private router: Router) {
      this.loadRegistrationList();
      if (this.dataProvider.storage) {
        console.log(this.dataProvider.storage);
        var personalDetailForm: any = this.dataProvider.storage['personalDetailForm']
        this.buildPersonalDetailsForm(personalDetailForm, 'Edit');
      } else{
        this.buildPersonalDetailsForm({}, 'New');
      }
    }

  ngOnInit(): void {
    }
    loadRegistrationList() {
        this.registrationService.getRegistrationList().subscribe(result => {
            this.registrations = result;
            console.log(this.registrations);
        });
    }

  buildPersonalDetailsForm(data: any, keyName: string) {
    this.personalDetailForm = this.fb.group({
      FirstName: [keyName === 'New' ? '' : data.FirstName, Validators.required],
      LastName: [keyName === 'New' ? '' : data.LastName, Validators.required],
      Phone: [keyName === 'New' ? null : data.Phone, Validators.required],
      Email: [keyName === 'New' ? '' : data.Email, [Validators.required, Validators.email]],
      Password: [keyName === 'New' ? '' : data.Password,  Validators.required],
      ConfirmPassword: [keyName === 'New' ? '' : data.ConfirmPassword,  Validators.required]
    },
     {
       validator: this.confirmedValidator("Password", "ConfirmPassword")
     }
    );
  }

  confirmedValidator(controlName: string, matchingControlName: string) {
    return (formGroup: FormGroup) => {
      const control = formGroup.controls[controlName];
      const matchingControl = formGroup.controls[matchingControlName];
      if (matchingControl.errors && !matchingControl.errors.confirmedValidator) {
          return;
      }
      if (control.value !== matchingControl.value) {
          matchingControl.setErrors({ confirmedValidator: true });
      } else {
          matchingControl.setErrors(null);
      }
  }
  }

  nextPage(){
    this.submitted = true;
    if (this.personalDetailForm.invalid) {
      this.personalDetailForm.markAllAsTouched();
      return;
    } else {
        this.ErrorMessage = '';
        if (this.registrations != null && this.registrations.length > 0) {
            var emailsList = this.registrations.map(e => ({ email: e.email.split('@')[1].toLowerCase(), isComplete: e.isComplete }));
            var email = this.personalDetailForm.value.Email.toLowerCase().split('@');
            var isEmailExist = emailsList.find(x => x.email === email[1] && x.isComplete === true);
            if (isEmailExist != null && isEmailExist != undefined) {
                this.ErrorMessage = "Email already exist with same domain";
            } else {
             this.addRegistration();
            }
        } else {
           this.addRegistration();
        }
      }
    }

    Encryption(number) {
        console.log(number);
        var key = CryptoJS.enc.Utf8.parse(this.secretKey);
        var iv = CryptoJS.enc.Utf8.parse(this.secretKey);
        var encrypted = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(number), key,
        {
            keySize: 128 / 8,
            iv: iv,
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        });
        return encrypted.toString();
    }

    addRegistration() {

        var encryptedPassword = this.Encryption(this.personalDetailForm.value.Password); 
        var encryptedConfirmPassword = this.Encryption(this.personalDetailForm.value.ConfirmPassword);
        
        this.registration = {
            RegistrationID: 0,
            FirstName: this.personalDetailForm.value.FirstName,
            LastName: this.personalDetailForm.value.LastName,
            Email: this.personalDetailForm.value.Email,
            Phone: this.personalDetailForm.value.Phone,
            Password: encryptedPassword,
            ConfirmPassword: encryptedConfirmPassword,
            Company: "",
            CompanyURL: "",
            City: "",
            StateID: null,
            CountryID: null,
            ZIPCode: "",
            GUID: null,
            IsActive: false,
            IsComplete: false,
            RecordID: 0,
            DateCreated: new Date(),
            CreatedBy: "",
            DateModified: new Date(),
            ModifiedBy: "",
        };

        this.registrationService.addRegistration(this.registration).subscribe(result => {
            if (this.dataProvider.storage != undefined) {
                this.dataProvider.storage = {
                    registrationForm: this.dataProvider.storage['registrationForm'] != undefined ? this.dataProvider.storage['registrationForm'] : undefined,
                    personalDetailForm: this.personalDetailForm.value
                };
            } else {
                this.dataProvider.storage = {
                    personalDetailForm: this.personalDetailForm.value
                };
            }
            this.router.navigate(['registration/address']);
        });
    }

    

  get caseFormControls() { return this.personalDetailForm.controls; }

}
