import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
//import { IUser } from '../core/interfaces/user';
import { User, UserToken } from 'src/app/_models';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private userSubject: BehaviorSubject<UserToken>;
    public user: Observable<UserToken>;
    private validateLoginUserApiMethod = 'api/User/ValidateUser';

    constructor(
        private router: Router,
        private http: HttpClient
    ) {
        this.userSubject = new BehaviorSubject<UserToken>(JSON.parse(localStorage.getItem('IHR-current-loggedin-user')));
        this.user = this.userSubject.asObservable();
    }

    public get userValue(): UserToken {
        return this.userSubject.value;
    }

    login(email: string, password: string, clientID: string) {
        let user = {
            email: email,
            password: password,
            clientID: clientID
        };
        return this.http.post<any>(`${this.validateLoginUserApiMethod}`, user)
            .pipe(map(user => {
                console.log(user);
                localStorage.setItem('IHR-current-loggedin-user', JSON.stringify(user));
                this.userSubject.next(user);
                return user;
          }));
    }

    logout() {
        localStorage.removeItem('IHR-current-loggedin-user');
        this.userSubject.next(null);
        this.router.navigate(['/login']);
    }
}
