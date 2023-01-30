import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmployee } from '../../core/interfaces/Employee';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
    getEmployeeApiMethod = "api/Employee";
    getEmployeeInfoApiMethod = "api/Employee/GetEmployeeInfo";
    GetEmployeeByIdAsyncApiMethod = "api/Employee";
    getDepartmentApiMethod = "api/Department";
    SaveEmployeeApiMethod = "api/Employee"
    UpdateEmployeeApiMethod = "api/Employee"
    GetEmployeeChangesetApiMethod = "api/EmployeeChangeSet"
    SaveNotificationApiMethod = "api/Notification"
  constructor(private http: HttpClient) { }

    GetEmployees() {
        return this.http.get<any[]>(`${this.getEmployeeApiMethod}`);
    }

    getEmployeeInfo() {
        return this.http.get<any[]>(`${this.getEmployeeInfoApiMethod}`);
    }
    getEmployeeByIdAsync(EmployeeID: number) {
        return this.http.get<any[]>(`${this.GetEmployeeByIdAsyncApiMethod}/${EmployeeID}`);
    }
    GetDepartments() {
        return this.http.get<any[]>(`${this.getDepartmentApiMethod}`);
    }
    SaveEmployee(holiday: IEmployee): Observable<any> {
        return this.http.post<any>(`${this.SaveEmployeeApiMethod}`, holiday);
    }
    UpdateEmployee(EmployeeID: number,employee: any): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmployeeApiMethod}/${EmployeeID}`, employee);
    }
    GetEmployeeChangeset(EmployeeID: number) {
        return this.http.get<any[]>(`${this.GetEmployeeChangesetApiMethod}/${EmployeeID}`);
    }
    SaveNotification(EmployeeID: number, employee: any): Observable<any[]> {
        return this.http.put<any>(`${this.SaveNotificationApiMethod}/${EmployeeID}`, employee);
    }
}
