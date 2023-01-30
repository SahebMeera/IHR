import { Component, OnInit } from '@angular/core';
import { TicketService } from '../ticketservice';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { DataProvider } from 'src/app/core/providers/data.provider';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {

  constructor(private fb: FormBuilder,
    public ticketService: TicketService,
    private dataProvider: DataProvider,
    private router: Router) { 
        if (this.dataProvider.storage) {
            var registrationForm: any = this.dataProvider.storage['registrationForm']
            console.log('registrationForm', registrationForm)
            if(registrationForm != undefined) {
                this.buildResgitrationForm(registrationForm, 'Edit');
            } else {
                this.buildResgitrationForm({}, 'New');
            }
          } else{
            this.buildResgitrationForm({}, 'New');
          }
    }

  classes: any[];

  vagons: any[];
  
  seats: any[];
  registrationForm: FormGroup;

  seatInformation: any;

  ngOnInit() { 
      this.seatInformation = this.ticketService.ticketInformation.seatInformation;
     
      this.classes = [
          {name: 'First Class', code: 'A', factor: 1},
          {name: 'Second Class', code: 'B', factor: 2},
          {name: 'Third Class', code: 'C', factor: 3}
      ];    
  }

  buildResgitrationForm(data: any, keyName: string) {
    this.registrationForm = this.fb.group({
        FirstName: [keyName === 'New' ? '' : data.FirstName, Validators.required],
        LastName: [keyName === 'New' ? '' : data.LastName,  Validators.required],
        Phone: [keyName === 'New' ? null : data.Phone,  Validators.required],
        Company: [keyName === 'New' ? '' : data.Company,  Validators.required],
        WebSite: [keyName === 'New' ? '' : data.WebSite,  Validators.required],
        City: [keyName === 'New' ? '' : data.City,  Validators.required],
        State: [keyName === 'New' ? '' : data.State,  Validators.required],
        Country: [keyName === 'New' ? '' : data.Country,  Validators.required],
        Zip: [keyName === 'New' ? null : data.Zip,  Validators.required],
      });
  }

  get registrationFormControls() { return this.registrationForm.controls; }

  setVagons(event) {
      if (this.seatInformation.class && event.value) {
          this.vagons = [];
          this.seats = [];
          for (let i = 1; i < 3 * event.value.factor; i++) {
              this.vagons.push({wagon: i + event.value.code, type: event.value.name, factor: event.value.factor});
          }
      }
  }
  
  setSeats(event) {
      if (this.seatInformation.wagon && event.value) {
          this.seats = [];
          for (let i = 1; i < 10 * event.value.factor; i++) {
              this.seats.push({seat: i, type: event.value.type});
          }
      }
  }

  nextPage() {
    if (this.registrationForm.invalid) {
        this.registrationForm.markAllAsTouched();
      } else {
        console.log(this.dataProvider.storage)
        if(this.dataProvider.storage != undefined){
            this.dataProvider.storage = {
              registrationForm: this.registrationForm.value,
              confirmationForm: this.dataProvider.storage['confirmationForm'] != undefined ? this.dataProvider.storage['confirmationForm'] : undefined,
              loginForm: this.dataProvider.storage['loginForm']
            };
           }
         // this.ticketService.ticketInformation.seatInformation = this.seatInformation;
          this.router.navigate(['steps/confirmation']);
        }
    
  }

  prevPage() {
    if(this.dataProvider.storage != undefined){
    this.dataProvider.storage = {
        registrationForm: this.registrationForm.value,
        confirmationForm: this.dataProvider.storage['confirmationForm'] != undefined ? this.dataProvider.storage['confirmationForm'] : undefined,
        loginForm: this.dataProvider.storage['loginForm']
      };
    }
      this.router.navigate(['steps/login']);
  }

}
