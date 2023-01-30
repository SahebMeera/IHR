import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IEmployeeDependent } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';
import { IEmployeeW4, IEmployeeW4Display } from '../../../core/interfaces/Employeew4';

@Injectable()

@Injectable({
    providedIn: 'root'
})
export class I9DocumentService {
    getI9DocumentApiMethod = "api/I9Document";
    getEmployeeW4ByIdAsyncApiMethod = "api/EmployeeW4";
    SaveEmployeeW4ApiMethod = "api/EmployeeW4"
    UpdateEmployeeW4ApiMethod = "api/EmployeeW4"

    constructor(private http: HttpClient) { }
    GetI9Documents() {
        return this.http.get<any[]>(`${this.getI9DocumentApiMethod}`);
    }

    getEmployeeW4ByIdAsync(directDepositID: number) {
        //const options = {
        //    params: {
        //        employeeId: employeeId,
        //    }
        //}
        return this.http.get<any[]>(`${this.getEmployeeW4ByIdAsyncApiMethod}/${directDepositID}`);
    }

    SaveEmployeeW4(dependent: IEmployeeW4): Observable<any> {
        return this.http.post<any>(`${this.SaveEmployeeW4ApiMethod}`, dependent);
    }
    UpdateEmployeeW4(dependent: IEmployeeW4): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmployeeW4ApiMethod}/${dependent.EmployeeW4ID}`, dependent);
    }

}
