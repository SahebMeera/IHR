import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { CommonUtils } from '../../common/common-utils';
import { ILeave } from '../../core/interfaces/Leave';

@Injectable({
    providedIn: 'root'
})
export class LeaveRequestService {
    commonUtils = new CommonUtils();
    getLeaveApiMethod = "api/Leave";
    getGetLeaveDaysApiMethod = "api/Leave/GetLeaveDays";
    getGetLeaveByIdAsyncApiMethod = "api/Leave";
    getSaveLeaveApiMethod = "api/Leave";
    getUpdateLeaveApiMethod = "api/Leave";

    constructor(private http: HttpClient) { }

    GetLeave(parmeter: string, ID: number) {
        const options = {
            params: {
                [parmeter]: ID,
            }
        }
        return this.http.get<any[]>(`${this.getLeaveApiMethod}`, options);
    }

    GetLeaveReport() {
        return this.http.get<any[]>(`${this.getLeaveApiMethod}`);
    }

    GetLeaveDays(clientID: string, employeeId: number, startDate: Date, endDate: Date, includesHalfDay: boolean) {
        return this.http.get<any>(`${this.getGetLeaveDaysApiMethod}?clientId=${clientID}&employeeId=${employeeId}&startDate=${this.commonUtils.formatDateDefault(startDate).toString()}&endDate=${this.commonUtils.formatDateDefault(endDate).toString()}&includesHalfDay=${includesHalfDay}`);
    }
    getGetLeaveByIdAsync(id: number, EmployeeID: number, ApproverID: number) {

        return this.http.get<any>(`${this.getGetLeaveByIdAsyncApiMethod}/${id}`)
    }

    SaveLeave(holiday: ILeave): Observable<any> {
        return this.http.post<any>(`${this.getSaveLeaveApiMethod}`, holiday);
    }
    UpdateLeave(LeaveID: number, employee: ILeave): Observable<any[]> {
        return this.http.put<any>(`${this.getUpdateLeaveApiMethod}/${LeaveID}`, employee);
    }
}
