import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class LeaveBalanceService {
    getLeaveBalanceApiMethod = "api/LeaveBalance";
  
    constructor(private http: HttpClient) { }

    GetLeaveBalance(EmployeeID: any) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
            }
        }
        return this.http.get<any[]>(`${this.getLeaveBalanceApiMethod}`, options);
    }

}
