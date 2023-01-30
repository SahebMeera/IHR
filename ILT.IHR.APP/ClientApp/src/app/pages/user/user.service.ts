import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IUser } from '../../core/interfaces/User';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class UserService {
  getUserApiMethod = "api/User";
  GetUserByIdAsyncApiMethod = "api/User";
  SaveUserApiMethod = "api/User"
  UpdateUserApiMethod = "api/User"
  CommonApiMethod = "api/Common"

  constructor(private http: HttpClient) { }

    getUserList() {
        return this.http.get<any[]>(`${this.getUserApiMethod}`);
    }

    GetUserByIdAsync(userID: number) {
        return this.http.get<any[]>(`${this.GetUserByIdAsyncApiMethod}/${userID}`);
    }

    getUsers() {
    return this.http.get<any>('assets/demo/data/UserInfo.json')
      .toPromise()
      .then(res => res.data as any[])
      .then(data => data);
    }

    SaveUser(user: IUser): Observable<any> {
        return this.http.post<any>(`${this.SaveUserApiMethod}`, user);
    }
    UpdateUser(user: any): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateUserApiMethod}/${user.UserID}`, user);
    }

    SendEmail(common: any): Observable<any> {
        return this.http.post<any>(`${this.CommonApiMethod}`, common);
    }
}
