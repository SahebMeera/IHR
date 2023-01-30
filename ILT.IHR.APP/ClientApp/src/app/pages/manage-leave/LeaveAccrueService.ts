import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ILeaveAccrual } from '../../core/interfaces/LeaveAccrual';
import { ILeaveBalance } from '../../core/interfaces/LeaveBalance';

@Injectable({
    providedIn: 'root'
})
export class LeaveAccrueService {
    constructor(private http: HttpClient) { }
    getGetLeaveAccrualApiMethod = "api/LeaveAccrual";
    getLeaveAccrualByIdAsyncApiMethod = "api/LeaveAccrual";
    updateLeaveAccrualApiMethod = "api/LeaveAccrual";
    SaveLeaveAccrualApiMethod = "api/LeaveAccrual";

    GetLeaveAccrual(Country: string) {
        return this.http.get<any>(`${this.getGetLeaveAccrualApiMethod}?Country=${Country}`);
    }

    GetLeaveAccrualByIdAsync(ID: number) {
        return this.http.get<any>(`${this.getLeaveAccrualByIdAsyncApiMethod}/${ID}`);
    }
    updateLeaveAccrual(ID: number, leaveBalance: ILeaveAccrual): Observable<any[]> {
        return this.http.put<any>(`${this.updateLeaveAccrualApiMethod}/${ID}`, leaveBalance);
    }
    SaveLeaveAccrual(leaveBalance: ILeaveAccrual): Observable<any[]> {
        return this.http.post<any>(`${this.SaveLeaveAccrualApiMethod}`, leaveBalance);
    }
}
