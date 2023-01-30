import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ILeaveBalance } from '../../core/interfaces/LeaveBalance';

@Injectable({
    providedIn: 'root'
})
export class LeaveBalanceService {
    constructor(private http: HttpClient) { }
    getGetLeaveBalanceApiMethod = "api/LeaveBalance";
    getGetLeaveBalanceByIDApiMethod = "api/LeaveBalance";
    updateLeaveBalanceApiMethod = "api/LeaveBalance";
    GetReportLeaveDetailInfoApiMethod = "api/LeaveBalance/GetLeaveDetail";

    GetLeaveBalance(employeeId: number) {
        if (employeeId === 0) {
            return this.http.get<any>(`${this.getGetLeaveBalanceApiMethod}`);
        } else {
            return this.http.get<any>(`${this.getGetLeaveBalanceApiMethod}?EmployeeID=${employeeId}`);
        }
    }

    GetLeaveBalanceById(ID: number) {
        return this.http.get<any>(`${this.getGetLeaveBalanceByIDApiMethod}/${ID}`);
    }
    UpdateLeaveBalance(ID: number, leaveBalance: ILeaveBalance): Observable<any[]> {
        return this.http.put<any>(`${this.updateLeaveBalanceApiMethod}/${ID}`, leaveBalance);
    }
    GetReportLeaveDetailInfo(reportReq: any, LeaveDetailStatus: any) {
        console.log('Heree', reportReq)
      return this.http.post<any>(`${this.GetReportLeaveDetailInfoApiMethod}`, reportReq);
        //console.log(resp)
        //if ((LeaveDetailStatus !== null && LeaveDetailStatus !== '') && resp['data'] !== null) {
        //    if (LeaveDetailStatus == "All") {
        //        var json = resp['data'];
        //        //  DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof (DataTable)));
        //        return json;
        //    } else {
        //         var json = resp['data'];
        //        //  DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof (DataTable)));
        //        return json;
        //    }
        //    //else if (LeaveDetailStatus != "Active") {
        //    //    var data = resp.Data.Where(x => x.TermDate != null && x.TermDate <= DateTime.Now).ToList();
        //    //    string json = JsonConvert.SerializeObject(data);
        //    //    DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof (DataTable)));
        //    //    return dt;
        //    //}
        //    //else {
        //    //    var data = resp.Data.Where(x => x.TermDate == null || x.TermDate > DateTime.Now).ToList();
        //    //    string json = JsonConvert.SerializeObject(data);
        //    //    DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof (DataTable)));
        //    //    return dt;
        //    //}
        //} else {
        //    var json = resp['data'];
        //    console.log(json)
        //    //string json = JsonConvert.SerializeObject(resp.Data);
        //    //DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof (DataTable)));
        //    //return dt;
        //}

    }
}
