import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router } from '@angular/router';
import { DataProvider } from 'src/app/core/providers/data.provider';
import { LoginService } from './login.service';
import { Md5 } from 'ts-md5';
// import { UserService } from '../admin/user/user.service';

import { AuthenticationService } from 'src/app/_services';
import { User } from '../../_models/user';
import * as CryptoJS from 'crypto-js';
import { SessionConstants, Settings } from '../../constant';
import { environment } from '../../../environments/environment';
// import { RolePermissionService } from '../admin/role-permission/role-permission.service';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})

export class LoginComponent implements OnInit {
   environment = environment
  settings = new Settings()
  loginForm: FormGroup;
  submitted: boolean = false;
  isLoginValid: boolean = true;
  isPasswordAdded: boolean = false;
  passwordForm: FormGroup;
  //userInfo: IUser;
// updateUserForm: IUserForm;
  LoginErrorMessage = '';
    Clients: string;
  isLoading = false;
 
  

  constructor(private dataProvider: DataProvider,
    private fb: FormBuilder, 
    private router: Router,
    private md5: Md5,
    private loginService: LoginService,
      private authenticationService: AuthenticationService,
     ) {
      this.buildLoginForm({}, 'New');
      this.Clients = environment.ClientID;
     // this.Clients.push(clientID)
     }

  ngOnInit(): void {
  }

  buildLoginForm(data: any, keyName: string) {
    this.loginForm = this.fb.group({
      ClientID: [keyName === 'New' ? '' : data.ClientID, Validators.required],
      Email: [keyName === 'New' ? '' : data.Email, [Validators.required, Validators.email]],
      Password: [keyName === 'New' ? '' : data.Password,  Validators.required]
    });
  }

  get loginFormControls() { return this.loginForm.controls; }
  get passwordFormControls() { return this.passwordForm.controls; }

    loadUserDetails(ID: number) {
    // this.userService.getUserDetail(ID).subscribe(result => {
    //     console.log(result);
    // })
    }
    ValidateUser() {
        this.submitted = true;
        if (this.loginForm.invalid) {
            this.loginForm.markAllAsTouched();
            return;
        } else {
            this.LoginErrorMessage = '';
            if (!this.Clients.includes(this.loginForm.value.ClientID)) {
                this.LoginErrorMessage = "Invalid Email Address or Password";
            } else {
                this.isLoading = true;
                this.authenticationService.login(this.loginForm.value.Email, this.loginForm.value.Password, this.loginForm.value.ClientID).subscribe(result => {
                    if (result.user !== null) {
                        if (result.user.isActive == false) {
                            this.LoginErrorMessage = "Invalid Email Address or Password";
                            this.isLoading = false;
                        } else {
                            var currentUser = JSON.parse(JSON.stringify(result.user));
                            localStorage.setItem("email", currentUser['email']);
                            localStorage.setItem("token", result.token);
                            localStorage.setItem("User", JSON.stringify(currentUser));
                            localStorage.setItem("ClientID", currentUser['clientID']);
                            localStorage.setItem(SessionConstants.USERROLES, JSON.stringify(currentUser['userRoles']));
                            localStorage.setItem(SessionConstants.USERROLEPERMISSIONS, JSON.stringify(currentUser['rolePermissions']));
                            var roleID: any = 0;
                            var roleName: string = '';
                            let roleShort: string = '';
                            var user = JSON.stringify(currentUser)
                            var userroles = JSON.parse(localStorage.getItem(SessionConstants.USERROLES));
                            if (userroles !== undefined && userroles !== null && userroles.length == 1) {
                                roleID = userroles[0]['roleID'];
                                roleName = userroles[0]['roleName'];
                                roleShort = userroles[0]['roleShort'];
                            }
                            else {
                                var role = userroles.find(x => x.isDefault == true);
                                if (role != null) {
                                    roleID = role['roleID'];
                                    roleName = role['roleName'];
                                    roleShort = role['roleShort'];
                                }
                                else {
                                    roleID = userroles[0]['roleID'];
                                    roleName = userroles[0]['roleName'];
                                    roleShort = userroles[0]['roleShort'];
                                }
                            }
                            localStorage.setItem("RoleID", roleID);
                            localStorage.setItem("RoleName", roleName);
                            localStorage.setItem("RoleShort", roleShort);
                            var userrolesPermissions = JSON.parse(localStorage.getItem(SessionConstants.USERROLEPERMISSIONS))
                            if (userrolesPermissions !== undefined && userrolesPermissions !== null) {
                                var defaultRolePermissions = userrolesPermissions.filter(x => x.roleID === roleID);
                                localStorage.setItem(SessionConstants.ROLEPERMISSION, JSON.stringify(defaultRolePermissions));
                            }
                            this.router.navigate(['/dashboard']);
                            this.isLoading = false;
                        }

                    } else {
                        this.LoginErrorMessage = "Invalid Email Address or Password";
                        this.isLoading = false;
                    }
                })
            }
        }
        }
}
