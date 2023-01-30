import { Component, OnInit } from '@angular/core';
import { TicketService } from '../ticketservice';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { DataProvider } from 'src/app/core/providers/data.provider';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  personalInformation: any;

  loginForm: FormGroup;

  submitted: boolean = false;

  constructor( private dataProvider: DataProvider,
    private fb: FormBuilder, 
    public ticketService: TicketService, 
    private router: Router) {
      if (this.dataProvider.storage) {
        var loginForm: any = this.dataProvider.storage['loginForm']
        this.buildLoginForm(loginForm, 'Edit');
      } else{
        this.buildLoginForm({}, 'New');
      }
    
  }

  ngOnInit() { 
      
      this.personalInformation = this.ticketService.getTicketInformation().personalInformation;
  }

  buildLoginForm(data: any, keyName: string) {
    this.loginForm = this.fb.group({
      email: [keyName === 'New' ? '' : data.email, [Validators.required, Validators.email]],
      password: [keyName === 'New' ? '' : data.password,  Validators.required],
    });
  }

  nextPage() {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
    } else {
      console.log(this.dataProvider.storage);
      if(this.dataProvider.storage != undefined){
        this.dataProvider.storage = {
          registrationForm: this.dataProvider.storage['registrationForm'] != undefined ? this.dataProvider.storage['registrationForm'] : undefined,
          confirmationForm: this.dataProvider.storage['confirmationForm'] != undefined ? this.dataProvider.storage['confirmationForm'] : undefined,
          loginForm: this.loginForm.value
        };
       } else {
        this.dataProvider.storage = {
          loginForm: this.loginForm.value
        };
       }
          this.router.navigate(['steps/registration']);
          return;
      }
      this.submitted = true;
  }
  get caseFormControls() { return this.loginForm.controls; }

}
