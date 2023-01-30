import { Component, OnInit } from '@angular/core';
import { TicketService } from '../ticketservice';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { DataProvider } from 'src/app/core/providers/data.provider';


@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss']
})
export class ConfirmationComponent implements OnInit {

  paymentInformation: any;
  confirmationForm: FormGroup;

  constructor(private fb: FormBuilder,
    public ticketService: TicketService,
    private dataProvider: DataProvider, 
    private router: Router) { 
      if (this.dataProvider.storage) {
        var confirmationForm: any = this.dataProvider.storage['confirmationForm']
        if(confirmationForm != undefined) {
        this.buildConfirmationForm(confirmationForm, 'Edit');
        } else {
          this.buildConfirmationForm({}, 'New');
        }
      } else{
        this.buildConfirmationForm({}, 'New');
      }
    }

  ngOnInit() { 
    
      this.paymentInformation = this.ticketService.ticketInformation.paymentInformation;
  }

  buildConfirmationForm(data: any, keyName: string) {
    this.confirmationForm = this.fb.group({
      IsComplete: [keyName === 'New' ? false : data.IsComplete, Validators.required],
      IsActivated: [keyName === 'New' ? false : data.IsActivated, Validators.required]
    });
  }

  nextPage() {
      console.log(this.dataProvider.storage)
      this.dataProvider.storage = {
        registrationForm: this.dataProvider.storage['registrationForm'],
        loginForm: this.dataProvider.storage['loginForm'],
        confirmationForm: this.confirmationForm.value
      };
      this.router.navigate(['steps/confirmation']);
  }

  prevPage() {
    this.dataProvider.storage = {
      registrationForm: this.dataProvider.storage['registrationForm'],
      loginForm: this.dataProvider.storage['loginForm'],
      confirmationForm: this.confirmationForm.value
    };
      this.router.navigate(['steps/registration']);
  }
  get confirmationFormControls() { return this.confirmationForm.controls; }

}
