import { Component, OnInit, HostListener, Renderer2, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { trigger, state, style, transition, animate } from '@angular/animations';
import { PrimeNGConfig } from 'primeng/api';
import { AppComponent } from '../../app.component';

import { AuthenticationService } from '../../_services/authentication.service';
import { User } from '../../_models';
import { DataProvider } from '../../core/providers/data.provider';
import { BreadcrumbService } from '../../app.breadcrumb.service';
import { Subscription } from 'rxjs';
import { MenuItem } from 'primeng/api'
import { AppMenuComponent } from 'src/app/app.menu.component';
import { AddEditUserComponent } from '../../pages/user/add-edit-user/add-edit-user.component';
import { AllowedClients, SessionConstants } from '../../constant';
import { IRolePermission, IRolePermissionDisplay } from '../../core/interfaces/RolePermission';
import { HelpDocumentationComponent } from '../../pages/help-documentation/help-documentation.component';
import { UserNotificationComponent } from '../../pages/user-notification/user-notification.component';


declare var $: any;

//import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';

@Component({
  selector: 'app-master-layout',
  templateUrl: './master-layout.component.html',
    styleUrls: ['./master-layout.component.css'],
    animations: [
        trigger('submenu', [
            state('hidden', style({
                height: '0px'
            })),
            state('visible', style({
                height: '*'
            })),
            transition('visible => hidden', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)')),
            transition('hidden => visible', animate('400ms cubic-bezier(0.86, 0, 0.07, 1)'))
        ])
    ]
})
export class MasterLayoutComponent implements OnInit {
    @ViewChild('appMenu') AppMenuComponent: AppMenuComponent;
    @ViewChild('AddEditUserModal') addEditUserModal: AddEditUserComponent;


  tabStatus = 'justified';

  public isCollapsed = false;

  public innerWidth: any;
  public defaultSidebar: any;
  public showSettings = false;
  public showMobileMenu = false;
  public expandLogo = false;

  options = {
    theme: 'light', // two possible values: light, dark
    dir: 'ltr', // two possible values: ltr, rtl
    layout: 'vertical', // fixed value. shouldn't be changed.
    sidebartype: 'full', // four possible values: full, iconbar, overlay, mini-sidebar
    sidebarpos: 'fixed', // two possible values: fixed, absolute
    headerpos: 'fixed', // two possible values: fixed, absolute
    boxed: 'full', // two possible values: full, boxed
    navbarbg: 'skin4', // six possible values: skin(1/2/3/4/5/6)
    sidebarbg: 'skin5', // six possible values: skin(1/2/3/4/5/6)
    logobg: 'skin6' // six possible values: skin(1/2/3/4/5/6)
    };
    public menuInactiveDesktop: boolean;

    public menuActiveMobile: boolean;

    public profileActive: boolean;

    public topMenuActive: boolean;

    public topMenuLeaving: boolean;

    documentClickListener: () => void;

    menuClick: boolean;

    topMenuButtonClick: boolean;

    configActive: boolean;
    menuMode = 'static';

    configClick: boolean;
    userName: string = '';
    userDetails: User;
    UserNameShort: string;
    userRoles: any[] = [];
    RoleID: number = 1;
    clientID: string = '';
    ClientName: string = '';

    sideBarWidth: string = '200px';
    topLeftWidth: string = '200px';
    marginLeft: string = '197px';
    showSideBar: boolean = true;
    role: string;
    subscription: Subscription;


    items: MenuItem[];

    constructor(public breadcrumbService: BreadcrumbService,
        public renderer: Renderer2, private primengConfig: PrimeNGConfig,
        private dataProvider: DataProvider,
        private router: Router,
        private authorizationService: AuthenticationService,
        public app: AppComponent) {
        var UserDetails = JSON.parse(localStorage.getItem('IHR-current-loggedin-user'));
        this.clientID = localStorage.getItem('ClientID');
        if (this.clientID !== '' && this.clientID !== undefined) {
            this.loadClientName();
        }
        this.userDetails = UserDetails['user'];
        if (this.userDetails != null) {
            this.userName = this.userDetails.firstName + " " + this.userDetails.lastName;
            this.UserNameShort = this.userDetails.firstName.substring(0, 1) + this.userDetails.lastName.substring(0, 1);
            this.userRoles = this.userDetails['userRoles'];
            this.RoleID = JSON.parse(localStorage.getItem("RoleID"));
            //if (this.userRoles.findIndex(x => x['isDefault'] === true) === -1) {
            //    this.RoleID = this.userRoles[0]['roleID'];
            //    localStorage.removeItem("RoleShort")
            //    localStorage.setItem("RoleShort", this.userRoles[0]['roleShort']);
            //}
            
        }
        this.subscription = breadcrumbService.itemsHandler.subscribe(response => {
            this.items = response;
        });
    }

    loadClientName() {
        if ((Object.keys(AllowedClients) as string[]).includes(this.clientID.toString())) {
            this.ClientName = AllowedClients[this.clientID.toString()];
            if (this.ClientName === 'Development') {

            }
        }

    }
  Logo() {
    this.expandLogo = !this.expandLogo;
  }

 ngOnInit() {
     if (this.isDesktop()) {
         this.sideBarWidth = '200px';
         this.showSideBar = true;
         setTimeout(() => this.AppMenuComponent.showSideBars = true);
     } else {
         this.sideBarWidth = '0px';
         this.marginLeft = "0px";
         this.topLeftWidth = "0px";
         this.showSideBar = false;
         //if (this.AppMenuComponent.showSideBars !== undefined) {
             setTimeout(() =>  this.AppMenuComponent.showSideBars = false );
         //}
       
     }
    
    //if (this.router.url === '/') {
    //  this.router.navigate(['/dashboard/classic']);
    //}
    //this.AppMenuComponent.showSideBars = this.showSideBar;
    this.defaultSidebar = this.options.sidebartype;
      this.handleSidebar();
      this.primengConfig.ripple = true;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event) {
    this.handleSidebar();
  }

  handleSidebar() {
    this.innerWidth = window.innerWidth;
    switch (this.defaultSidebar) {
      case 'full':
      case 'iconbar':
        if (this.innerWidth < 1170) {
          this.options.sidebartype = 'mini-sidebar';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      case 'overlay':
        if (this.innerWidth < 767) {
          this.options.sidebartype = 'mini-sidebar';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      default:
    }
  }

  toggleSidebarType() {
    switch (this.options.sidebartype) {
      case 'full':
      case 'iconbar':
        this.options.sidebartype = 'mini-sidebar';
        break;

      case 'overlay':
        this.showMobileMenu = !this.showMobileMenu;
        break;

      case 'mini-sidebar':
        if (this.defaultSidebar === 'mini-sidebar') {
          this.options.sidebartype = 'full';
        } else {
          this.options.sidebartype = this.defaultSidebar;
        }
        break;

      default:
    }
    }
    ngAfterViewInit() {
        // hides the overlay menu and top menu if outside is clicked
        this.documentClickListener = this.renderer.listen('body', 'click', (event) => {
            if (!this.isDesktop()) {
                if (!this.menuClick) {
                    this.menuActiveMobile = false;
                }

                if (!this.topMenuButtonClick) {
                    this.hideTopMenu();
                }
            }
            else {
                if (!this.menuClick && this.isOVerlay()) {
                    this.menuInactiveDesktop = true;
                }
            }

            if (this.configActive && !this.configClick) {
                this.configActive = false;
            }

            this.configClick = false;
            this.menuClick = false;
            this.topMenuButtonClick = false;
        });
    }

    toggleMenu(event: Event) {
        this.menuClick = true;
        if (this.isDesktop()) {
            this.menuInactiveDesktop = !this.menuInactiveDesktop;
            if (this.menuInactiveDesktop) {
                this.menuActiveMobile = false;
            }
        } else {
            this.menuActiveMobile = !this.menuActiveMobile;
            if (this.menuActiveMobile) {
                this.menuInactiveDesktop = false;
            }
        }

        if (this.topMenuActive) {
            this.hideTopMenu();
        }

        event.preventDefault();
    }

    toggleProfile(event: Event) {
        this.profileActive = !this.profileActive;
        event.preventDefault();
    }

    toggleTopMenu(event: Event) {
        this.topMenuButtonClick = true;
        this.menuActiveMobile = false;

        if (this.topMenuActive) {
            this.hideTopMenu();
        } else {
            this.topMenuActive = true;
        }

        event.preventDefault();
    }

    hideTopMenu() {
        this.topMenuLeaving = true;
        setTimeout(() => {
            this.topMenuActive = false;
            this.topMenuLeaving = false;
        }, 500);
    }

    onMenuClick() {
        this.dataProvider.storage = null;
        this.menuClick = true;
    }

    onRippleChange(event) {
        this.app.ripple = event.checked;
        this.primengConfig.ripple = event.checked;
    }

    onConfigClick(event) {
        this.configClick = true;
    }

    isStatic() {
        return this.app.menuMode === 'static';
    }

    isOVerlay() {
        return this.app.menuMode === 'overlay';
    }

    isDesktop() {
        return window.innerWidth > 1024;
    }

    onSearchClick() {
        this.topMenuButtonClick = true;
    }

    logout() {
        this.authorizationService.logout();
    }

    ngOnDestroy() {
        if (this.documentClickListener) {
            this.documentClickListener();
        }
    }
    openCloseMenu() {
        if (this.showSideBar) {
            this.sideBarWidth = "4.7rem";
            this.topLeftWidth = "65px";
            this.marginLeft = "69px";
            this.showSideBar = false;
        } else {
            this.sideBarWidth = "200px";
            this.topLeftWidth = "200px";
            this.marginLeft = "197px";
            this.showSideBar = true;
        }
        setTimeout(() => this.AppMenuComponent.showSideBars = this.showSideBar);
    }
    openCloseMenuMobile() {
        if (this.showSideBar) {
            this.sideBarWidth = "0px";
            this.topLeftWidth = "0px";
            this.marginLeft = "0px";
            this.showSideBar = false;
            //document.getElementById("mySidebar").style.display = "none";
        } else {
            this.sideBarWidth = "100%";
            this.topLeftWidth = "0px";
            this.marginLeft = "0px";
            this.showSideBar = true;
           // document.getElementById("mySidebar").style.display = "block";
        }

        this.AppMenuComponent.showSideBars = this.showSideBar;
    }
    mini: boolean = true;

 toggleSidebar() {
    if (this.mini) {
        document.getElementById("mySidebar").style.width = "250px";
        document.getElementById("main").style.marginLeft = "250px";
        this.mini = false;
    } else {
        document.getElementById("mySidebar").style.width = "85px";
        document.getElementById("main").style.marginLeft = "85px";
        this.mini = true;
    }
    }
    onConfigButtonClick(event) {
        this.configActive = !this.configActive;
        this.configClick = true;
        event.preventDefault();
    }
    signout() {
        this.authorizationService.logout();
    }
    myProfile() {
        this.addEditUserModal.Show(this.userDetails.userID, 'Myprofile');
    }

    rolePermissions: IRolePermissionDisplay[] = [];
    userRolePerm: IRolePermissionDisplay[] = [];

    onRoleChange(e) {
        if (e.value !== null) {
            if (this.userRoles.length > 0) {
                let roleName = this.userRoles.find(x => x.roleID === Number(e.value)).roleName;
                let roleShort = this.userRoles.find(x => x.roleID === Number(e.value)).roleShort;
                this.rolePermissions = JSON.parse(localStorage.getItem(SessionConstants.ROLEPERMISSION));
                this.userRolePerm = JSON.parse(localStorage.getItem(SessionConstants.USERROLEPERMISSIONS));
                this.rolePermissions = this.userRolePerm.filter(x => x.roleID === Number(e.value));
                localStorage.setItem("RoleID", e.value);
                // localStorage.removeItem("RoleShort");
                localStorage.setItem("RoleShort", roleShort);
                localStorage.setItem("RoleName", roleName);
                localStorage.removeItem(SessionConstants.ROLEPERMISSION);
                localStorage.setItem(SessionConstants.ROLEPERMISSION, JSON.stringify(this.rolePermissions));
                this.RoleID = Number(e.value);
                this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
                    this.router.navigate(['/dashboard']); // navigate to same route
                });
            }
        }
    }
    @ViewChild('HelpDocumentation') HelpDocumentation: HelpDocumentationComponent
    helpDocumentation() {
        this.HelpDocumentation.Show()
    }
    @ViewChild('userNotification') userNotification: UserNotificationComponent
    notificationDocumentation() {
        this.userNotification.show()
    }


}
