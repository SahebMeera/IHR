import { Component, AfterViewInit, EventEmitter, Output, HostListener, ViewChild, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import * as forge from 'node-forge';
import * as CryptoJS from 'crypto-js';


declare var $: any;

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.css']
})
export class NavigationComponent implements OnInit,AfterViewInit {
  @Output() toggleSidebar = new EventEmitter<void>();
  preferencesForm: FormGroup;
  changePasswordForm: FormGroup;
  passwordIsValid = false;

  key: string = 'fdsfdfrw12vdfsdfdfdwewe';
  isISMUSER: boolean = false;

  fontNameAndSpecimenList: any[] = [
    {
      fontName: 'Arial'
    },
    {
      fontName: 'Tahoma'
    },
    {
      fontName: 'Times New Roman'
    },
    {
      fontName: 'Verdana'
    }
  ]
  fontSizeAndSpecimenList: any[] = [
    {
      fontSize: '12.0'
    },
    {
      fontSize: '14.0'
    },
    {
      fontSize: '16.0'
    },
    {
      fontSize: '18.0'
    },
    {
      fontSize: '20.0'
    }
  ]

  screenAndLaunchPadList: any[] = [];


  constructor() {
   
    }

  // This is for Notifications
  notifications: Object[] = [
    {
      btn: 'btn-danger',
      icon: 'ti-link',
      title: 'Luanch Admin',
      subject: 'Just see the my new admin!',
      time: '9:30 AM'
    }
  ];

  // This is for Mymessages
  mymessages: Object[] = [
    {
      useravatar: 'assets/images/users/1.jpg',
      status: 'online',
      from: 'Pavan kumar',
      subject: 'Just see the my admin!',
      time: '9:30 AM'
    }
  ];

  ngOnInit() {
 
  }
  ngAfterViewInit() {}


}
