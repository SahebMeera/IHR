import { Component, Output, EventEmitter, ContentChildren, QueryList, AfterContentInit, HostListener, OnInit } from '@angular/core';
import { IFields } from '../../../core/interfaces/Processwizard';
import { WizardStepComponent } from './step.component';

@Component({
    selector: 'form-wizard',
    templateUrl: './wizard.component.html',
    styleUrls: ['./wizard.component.scss']
})
export class WizardComponent implements OnInit, AfterContentInit {
    @ContentChildren(WizardStepComponent)
    wizardSteps: QueryList<WizardStepComponent>;

    private _steps: Array<WizardStepComponent> = [];
    private _isCompleted: boolean = false;

    @Output()
    onStepChanged: EventEmitter<WizardStepComponent> = new EventEmitter<WizardStepComponent>();
    @Output() validationMessage = new EventEmitter<any>();
    @Output() submit = new EventEmitter<any>();
    isMobileResolution = false;
    @HostListener('window:resize', ['$event'])
    onResize(event) {
        event.target.innerWidth;
        if (window.innerWidth < 768) {
            this.isMobileResolution = true;
        } else {
            this.isMobileResolution = false;
        }
    }

    constructor() {
    }

    ngOnInit() {
        console.log(this._steps)
        console.log(this.wizardSteps)
    }
    ngAfterContentInit() {
        this.wizardSteps.forEach(step => this._steps.push(step));
        if (this.steps.length > 0) {
            setTimeout(() => this.steps[0].isActive = true);
        }
    }

    get steps(): Array<WizardStepComponent> {
        return this._steps.filter(step => !step.hidden);
    }

    get isCompleted(): boolean {
        return this._isCompleted;
    }

    get activeStep(): WizardStepComponent {
        return this.steps.find(step => step.isActive);
    }

    set activeStep(step: WizardStepComponent) {
        if (step !== this.activeStep && !step.isDisabled) {
            this.activeStep.isActive = false;
            step.isActive = true;
            this.onStepChanged.emit(step);
        }
    }

    public get activeStepIndex(): number {
        return this.steps.indexOf(this.activeStep);
    }

    get hasNextStep(): boolean {
        return this.activeStepIndex < this.steps.length - 1;
    }

  
    get hasPrevStep(): boolean {
        return this.activeStepIndex > 0;
    }

    public goToStep(step: WizardStepComponent): void {
        if (!this.isCompleted) {
            this.activeStep = step;
        }
    }
    isStepValid: boolean = false;

    ValidateWizardData(name: string, Fields: any[]) {
        if (Fields !== null && Fields !== undefined && Fields.length > 0) {
            const Elements = Fields;
            for (let element of Elements) {
                //Elements.forEach((element: IFields): boolean => {
                if (element.position == name && element.required.toString().toUpperCase() == "TRUE" && (element.value == "" || element.value == null)) {
                    this.validationMessage.emit(element.label + " cannot be blank")
                    return true;
                } else if (element.position == name && element.elementType == "EmailInput" && element.required.toString().toUpperCase() == "TRUE") {
                    var email = element.value;
                    var filter = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
                    //let regex = new RegExp('[a-z0-9]+@[a-z]+\.[a-z]{2,3}');
                    if (!filter.test(email)) {
                        this.validationMessage.emit(element.label + " not valid")
                        return true;
                    }
                }
               
            }
            return false;
        }
        return false;
    }
    public next(): void {
        if (this.hasNextStep) {
            if (this.activeStep.data !== undefined && this.activeStep.data.name !== null && this.activeStep.data.name !== undefined && this.activeStep.stepNumber === this.activeStep.data.name) {
                if (this.activeStep.data.fields !== undefined && this.activeStep.data.fields !== null && this.activeStep.data.fields.length > 0)
                if (!this.ValidateWizardData(this.activeStep.data.name, this.activeStep.data.fields)) {
                    let nextStep: WizardStepComponent = this.steps[this.activeStepIndex + 1];
                    this.validationMessage.emit('')
                    this.activeStep.onNext.emit(true);
                    nextStep.isDisabled = false;
                    this.activeStep = nextStep;
                } else {
                }
            }
        }
    }

    //public next(): void {
    //    if (this.hasNextStep) {
    //        let nextStep: WizardStepComponent = this.steps[this.activeStepIndex + 1];
    //        this.validationMessage.emit('')
    //        this.activeStep.onNext.emit(true);
    //        nextStep.isDisabled = false;
    //        this.activeStep = nextStep;
    //    }
    //}

    public previous(): void {
        if (this.hasPrevStep) {
            let prevStep: WizardStepComponent = this.steps[this.activeStepIndex - 1];
            this.activeStep.onPrev.emit();
            prevStep.isDisabled = false;
            this.activeStep = prevStep;
        }
    }

    public complete(): void {
        console.log(this.activeStep.data.name, this.activeStep.data.fields)
        if (!this.ValidateWizardData(this.activeStep.data.name, this.activeStep.data.fields)) {
            this.submit.emit()
            this.activeStep.onComplete.emit();
            this._isCompleted = true;
        }
    }

}
