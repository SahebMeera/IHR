import { OnInit } from '@angular/core';
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
    selector: 'wizard-step',
    template:`<div class="p-col-12" [hidden]="!isActive">
      <ng-content></ng-content>
    </div>
  `
})
export class WizardStepComponent implements  OnInit {
    @Input() title: string;
    @Input() hidden: boolean = false;
    @Input() isValid: boolean = true;
    @Input() showNext: boolean = true;
    @Input() showPrev: boolean = true;
    @Input() stepNumber: number;
    @Input() data: any;
    @Output() onNext: EventEmitter<any> = new EventEmitter<any>();
    @Output() onPrev: EventEmitter<any> = new EventEmitter<any>();
    @Output() onComplete: EventEmitter<any> = new EventEmitter<any>();


    private _isActive: boolean = false;
    isDisabled: boolean = true;

    constructor() { }

    @Input('isActive')
    set isActive(isActive: boolean) {
        this._isActive = isActive;
        this.isDisabled = false;
    }

    get isActive(): boolean {
        return this._isActive;
    }
    ngOnInit(): void {
        //this.loadGridData();
    }

}
