import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api/primeng-api';
import { Router } from '@angular/router';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss']
})
export class RegistrationComponent implements OnInit {
  items: MenuItem[];

  constructor( private router: Router) { }

  ngOnInit(): void {
    this.router.navigate(['registration/personalDetails']);
    this.items = [{
            label: 'Personal Details',
            routerLink: 'personalDetails'
        },
        {
            label: 'Address',
            routerLink: 'address'
        },
        {
            label: 'Confirmation',
            routerLink: 'confirmation'
        },
    ];

  }

}
