import { Component, HostListener, OnInit, Renderer2, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { PrimeNGConfig } from 'primeng/api';
import { AppComponent } from '../../app.component';
import { DataProvider } from '../../core/providers/data.provider';
import { EmailApprovalService } from '../employee/emailApproval.service';



@Component({
  selector: 'app-emailapproval',
    templateUrl: './emailapproval.component.html',
    styleUrls: ['./emailapproval.component.scss']
})
export class EmailApprovalComponent implements OnInit {

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
  
    clientID: string = '';
    ClientName: string = '';

    sideBarWidth: string = '200px';
    topLeftWidth: string = '200px';
    marginLeft: string = '197px';
    showSideBar: boolean = true;
    role: string;
    CurrUrl: string;
    ClientID: string;
    LinkID: string;
    Value: string;


    constructor(
        public renderer: Renderer2, private primengConfig: PrimeNGConfig,
        private dataProvider: DataProvider,
        private router: Router,
        private route: ActivatedRoute,
        private emailApprovalService: EmailApprovalService,
        public app: AppComponent) {
        if (this.dataProvider.storage !== null && this.dataProvider.storage !== undefined) {
            console.log(this.dataProvider.storage)
            var uri = this.router.url;
            if (uri !== '' && uri !== null && uri !== undefined) {
                this.CurrUrl = this.router.url.split('/')[0];
                this.CurrUrl = this.router.url.split('/')[1];
                console.log(this.CurrUrl)
            }
        }
        var uri = this.router.url;
        if (uri !== '' && uri !== null && uri !== undefined) {
            this.CurrUrl = this.router.url.split('/')[1];
            if (this.route !== undefined && this.route.snapshot !== undefined && this.route.snapshot.params !== undefined) {
                if (this.route.snapshot.params.ClientID !== undefined && this.route.snapshot.params.ClientID !== '') {
                    this.ClientID = this.route.snapshot.params.ClientID;
                }
                if (this.route.snapshot.params.LinkID !== undefined && this.route.snapshot.params.LinkID !== '') {
                    this.LinkID = this.route.snapshot.params.LinkID;
                }
                if (this.route.snapshot.params.Value !== undefined && this.route.snapshot.params.Value !== '') {
                    this.Value = this.route.snapshot.params.Value;
                }

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
            //setTimeout(() => this.AppMenuComponent.showSideBars = true);
        } else {
            this.sideBarWidth = '0px';
            this.marginLeft = "0px";
            this.topLeftWidth = "0px";
            this.showSideBar = false;
            //if (this.AppMenuComponent.showSideBars !== undefined) {
           // setTimeout(() => this.AppMenuComponent.showSideBars = false);
            //}

        }

        //if (this.router.url === '/') {
        //  this.router.navigate(['/dashboard/classic']);
        //}
        //this.AppMenuComponent.showSideBars = this.showSideBar;
        this.defaultSidebar = this.options.sidebartype;
        this.handleSidebar();
        this.primengConfig.ripple = true;
        this.loadEmailApproval()
    }

    loadEmailApproval() {
        this.emailApprovalService.getEmailApproval(this.ClientID, this.LinkID, this.Value).subscribe(result => {
            console.log(result)
        })
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
     //   this.authorizationService.logout();
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
      //  setTimeout(() => this.AppMenuComponent.showSideBars = this.showSideBar);
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

        //this.AppMenuComponent.showSideBars = this.showSideBar;
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

    DisplayMessage: string = '';

   

}
