import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { DataProvider } from 'src/app/core/providers/data.provider';
import { LoginComponent } from '../../login/login.component';


@Component({
  selector: 'app-confirmation',
  templateUrl: './confirmation.component.html',
  styleUrls: ['./confirmation.component.scss']
})
export class ConfirmationComponent implements OnInit {

  paymentInformation: any;
  confirmationForm: FormGroup;

  constructor(private fb: FormBuilder,
    private dataProvider: DataProvider, 
    private router: Router) { 
  
    }

  ngOnInit() { 
    
  }

Cancel() {
    this.router.navigate(['/login']);
}
}
