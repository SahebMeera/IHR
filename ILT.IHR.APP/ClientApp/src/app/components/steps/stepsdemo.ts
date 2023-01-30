import {Component,OnInit,ViewEncapsulation} from '@angular/core';
import {MenuItem, MessageService} from 'primeng/api';
import { TicketService } from './ticketservice';
import { Subscription } from 'rxjs';
import { Router } from '@angular/router';

@Component({
    templateUrl: './stepsdemo.html',
    styleUrls: ['stepsdemo.scss'],
    providers: [MessageService]
})
export class StepsDemo implements OnInit {

    items: MenuItem[];
    
    subscription: Subscription;

    constructor(public messageService: MessageService, 
        private router: Router,
        public ticketService: TicketService) {}

    ngOnInit() {
        this.router.navigate(['steps/login']);
        this.items = [{
                label: 'Login',
                routerLink: 'login'
            },
            {
                label: 'Registration',
                routerLink: 'registration'
            },
            {
                label: 'Confirmation',
                routerLink: 'confirmation'
            },
        ];

        this.subscription = this.ticketService.paymentComplete$.subscribe((personalInformation) =>{
            this.messageService.add({severity:'success', summary:'Order submitted', detail: 'Dear, ' + personalInformation.firstname + ' ' + personalInformation.lastname + ' your order completed.'});
        });
    }

    ngOnDestroy() {
        if (this.subscription) {
            this.subscription.unsubscribe();
        }
    }
}
