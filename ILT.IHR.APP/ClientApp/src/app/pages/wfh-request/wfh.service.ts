import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmployee } from '../../core/interfaces/Employee';
import { IWFH } from '../../core/interfaces/WFH';

@Injectable()

@Injectable({
    providedIn: 'root'
})
export class WFHService {
    getGetWFHApiMethod = "api/WFH";
    GetWFHByIdAsyncApiMethod = "api/WFH";
    SaveWFHApiMethod = "api/WFH"
    UpdateWFHApiMethod = "api/WFH"
    constructor(private http: HttpClient) { }

    GetWFH(EmployeeId: string, EmployeeID: number) {
          const options = {
            params: {
                  [EmployeeId]: EmployeeID
            }
        }
        return this.http.get<any[]>(`${this.getGetWFHApiMethod}/`, options);
    }
    GetWFHByIdAsync(WFHID: number,EmployeeID: number, ApproverID: number) {
        return this.http.get<any[]>(`${this.GetWFHByIdAsyncApiMethod}/${WFHID}`);
    }
    SaveWFH(holiday: IWFH): Observable<any> {
        return this.http.post<any>(`${this.SaveWFHApiMethod}`, holiday);
    }
    UpdateWFH(WFHID: number, employee: IWFH): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateWFHApiMethod}/${WFHID}`, employee);
    }
}

