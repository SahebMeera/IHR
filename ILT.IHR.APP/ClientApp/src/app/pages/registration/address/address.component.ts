import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { DataProvider } from 'src/app/core/providers/data.provider';
//import { IRegistration } from 'src/app/core/interfaces/registration';
import { RegistrationService } from '../registration.service';
import { forkJoin } from 'rxjs';
//import { ICountryDisplay } from '../../../core/interfaces/country';
//import { IStateDisplay } from '../../../core/interfaces/state';
// import { CandidateService } from '../../candidate/candidate.service';


@Component({
  selector: 'app-address',
  templateUrl: './address.component.html',
  styleUrls: ['./address.component.scss']
})
export class AddressComponent implements OnInit {

  registrationForm: FormGroup;
  personalDetailForm: any;
  registration: any;
    submitted: boolean = false;
    countryList: any[] = [];
    stateList: any[] = [];

  constructor(private fb: FormBuilder,
    private dataProvider: DataProvider,
      private registrationService: RegistrationService,
      // private candidateService: CandidateService,
      private router: Router) {
      this.loadDropDownList();
        if (this.dataProvider.storage) {
            var registrationForm: any = this.dataProvider.storage['registrationForm']
            this.personalDetailForm = this.dataProvider.storage['personalDetailForm']
            if(registrationForm != undefined) {
                this.buildAddressForm(registrationForm, 'Edit');
            } else {
                this.buildAddressForm({}, 'New');
            }
          } else{
            this.buildAddressForm({}, 'New');
      }
    }

    ngOnInit(): void {
    }


    loadDropDownList() {
        // forkJoin(
        //     this.candidateService.getCountryList(),
        //     this.candidateService.getStateList()
        // ).subscribe(result => {
        //     this.countryList = result[0];
        //     this.stateList = result[1];
        // })
    }

  buildAddressForm(data: any, keyName: string) {
    this.registrationForm = this.fb.group({
        Company: [keyName === 'New' ? '' : data.Company, Validators.required],
        WebSite: [keyName === 'New' ? '' : data.WebSite, Validators.required],
        City: [keyName === 'New' ? '' : data.City, Validators.required],
        StateID: [keyName === 'New' ? null : data.StateID,Validators.required],
        CountryID: [keyName === 'New' ? null : data.CountryID, Validators.required],
        ZipCode: [keyName === 'New' ? null : data.ZipCode, Validators.required],
      });
  }

  get registrationFormControls() { return this.registrationForm.controls; }

  Submit() {
    this.submitted = true;
    if (this.registrationForm.invalid) {
        this.registrationForm.markAllAsTouched();
        return;
      } else {
        if(this.dataProvider.storage != undefined){
            this.registration = {
                RegistrationID: 0,
                FirstName: this.personalDetailForm.FirstName,
                LastName: this.personalDetailForm.LastName,
                Email: this.personalDetailForm.Email,
                Phone: this.personalDetailForm.Phone,
                Password: this.personalDetailForm.Password,
                ConfirmPassword: this.personalDetailForm.ConfirmPassword,
                Company: this.registrationForm.value.Company,
                CompanyURL: this.registrationForm.value.WebSite,
                City: this.registrationForm.value.City,
                StateID: this.registrationForm.value.StateID,
                CountryID: this.registrationForm.value.CountryID,
                ZIPCode: (this.registrationForm.value.ZipCode).toString(),
                GUID: null,
                IsActive: false,
                IsComplete: true,
                RecordID: 0,
                DateCreated: new Date(),
                CreatedBy: "",
                DateModified: new Date(),
                ModifiedBy: "",
               // TimeStamp: new Date(),
                //ReturnCode: 0
           }

            this.registrationService.updateRegistration(this.registration).subscribe(result => {
             this.dataProvider.storage = {
              registrationForm: this.registrationForm.value,
             personalDetailForm: this.dataProvider.storage['personalDetailForm'],
            // RegistrationId: result.RegistrationId
            };
            this.router.navigate(['registration/confirmation']);
          })
        }
          return;
        }

  }

  prevPage() {
    if(this.dataProvider.storage != undefined){
    this.dataProvider.storage = {
        registrationForm: this.registrationForm.value,
        personalDetailForm: this.dataProvider.storage['personalDetailForm']
      };
    }
      this.router.navigate(['registration/personalDetails']);
  }


}
