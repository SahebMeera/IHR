import { Component, OnInit, Input, EventEmitter, Output } from '@angular/core';
import { IModalPopupAlternateOptions } from './ihr-modal-popup-alternate-options';

@Component({
  selector: 'app-ihr-modal-popup',
  templateUrl: './ihr-modal-popup.component.html',
  styleUrls: ['./ihr-modal-popup.component.scss']
})
export class IHRModalPopupComponent implements OnInit {

 // @Input() visibleFooter: boolean = true;
 @Input() options: IModalPopupAlternateOptions;
 @Output() closealterModalpopup: EventEmitter<any> =  new EventEmitter<any>();
 @Input() width: number = 500;
 public visible = false;
 public visibleAnimate = false;
 innerWidth;
 data: any;

 constructor(
 ) { }

 ngOnInit() {
    // console.log(this.options)
 }

 getMinWidth() {
   return this.width + 'px';
 }

 public setInvisible(): void {
   this.visible = false;
   setTimeout(() => this.visibleAnimate = false, 100);
 }

 public show(data: any = null): void {
   this.visible = true;
   setTimeout(() => this.visibleAnimate = true, 100);
   this.data = data;
 }

 public hide(): void {
   this.hideWithoutConfirmation();
   this.closealterModalpopup.emit(true)

 }
 public close(): void {
   this.hideWithoutConfirmation();
   this.closealterModalpopup.emit(true)

 }


 hideWithoutConfirmation() {
   this.visibleAnimate = false;
   setTimeout(() => this.visible = false, 300);
 }
}

