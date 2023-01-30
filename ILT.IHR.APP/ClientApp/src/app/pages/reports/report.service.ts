import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IReport } from '../../core/interfaces/Report';


@Injectable({
  providedIn: 'root'
})
export class ReportService {
    getAuditLogApiMethod = "api/AuditLog/GetAuditLogInfo";
    GetReportLeaveInfoApiMethod = "api/LeaveBalance/GetLeaveDetail";
    GetLeavesCountInfoApiMethod = "api/LeaveBalance/GetLeavesCount";


  constructor(private http: HttpClient) { }

    getAuditLogReport(report: IReport): Observable<any[]> {
        return this.http.post<any[]>(`${this.getAuditLogApiMethod}`, report);
    }
    GetReportLeaveInfo(report: IReport) {
        return this.http.post<any[]>(`${this.GetReportLeaveInfoApiMethod}`, report);
    }
    GetLeavesCountInfo(report: IReport) {
        return this.http.post<any[]>(`${this.GetLeavesCountInfoApiMethod}`, report);
    }

}
