import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
//import { IUser, IUserToken } from '../../core/interfaces/user';
import { Params } from '@angular/router';
// import { environment } from '../../environments/environment';

// import { IUser } from '../core/data/Interfaces/user';

@Injectable({
  providedIn: 'root'
})
export class LoginService {

  
  private validateLoginUserApiMethod = 'api/User/ValidateLoginUser';

  constructor(private http: HttpClient) {
  }

  // public get currentUserValue(): IUser {
  //   return this.currentUserSubject.value;
  // }

  //login(username: string, password: string) {
  //  const LoginData = {
  //      UserId: username,
  //      Password: password
  //    };

  //  validateUser(email: string, password: string): Observable<IUserToken> {
  //      //let queryParams: Params  = {
  //      //      Email: email,
  //      //      Password: password
  //      //};
  //      let user: IUser = {
  //          Email: email,
  //          Password: password,
  //          UserID: null
  //      }
  //      return this.http.post<IUserToken>(`${this.validateLoginUserApiMethod}`, user);
  //    };

  //  // return this.http.post<IUser>(`${environment.apiUrl}${this.validateLoginUserApiMethod}`, LoginData) //8087
  //  //   .pipe(map(user => {
  //  //     // store user details and jwt token in local storage to keep user logged in between page refreshes
  //  //     localStorage.setItem('pathx-current-loggedin-user', JSON.stringify(user));
  //  //     this.currentUserSubject.next(user);
  //  //     return user;
  //  //   }));
  //// }

  //logout() {
  //  // remove user from local storage to log user out
  //  // localStorage.removeItem('pathx-current-loggedin-user');
  //  // this.currentUserSubject.next(null);
  //}

}
