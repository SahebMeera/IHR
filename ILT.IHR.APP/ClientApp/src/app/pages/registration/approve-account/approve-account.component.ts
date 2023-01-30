import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { RegistrationService } from '../registration.service';

@Component({
  selector: 'app-approve-account',
  templateUrl: './approve-account.component.html',
  styleUrls: ['./approve-account.component.scss']
})
export class ApproveAccountComponent implements OnInit {

    Guid: string;
    constructor(private route: ActivatedRoute,
        private router: Router,
        private registartionService: RegistrationService) { }

    ngOnInit(): void {
        
        this.route.params.subscribe(param => {
            this.Guid = param['Guid'];
            console.log(this.Guid)
            this.registartionService.approveAccount(this.Guid).subscribe(result => {
                if (result === true) {

                }
            });
      });
    }
    login() {
        this.router.navigate(['/login']);
    }

}
