import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmailApproval } from '../../core/interfaces/EmailApproval';
import { IEmployee } from '../../core/interfaces/Employee';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class EmailApprovalService {
    getGetEmailApprovalApiMethod = "api/EmailApproval/EmailApproval";
    GetEmailApprovalsApiMethod = "api/EmailApproval";
    GetEmailApprovalByIdAsyncApiMethod = "api/EmailApproval";
    EamilApprovalActionApiMethod = "api/EmailApproval";
    SaveEmailApprovalApiMethod = "api/EmailApproval"
    UpdateEmailApprovalApiMethod = "api/EmailApproval"
  constructor(private http: HttpClient) { }

    getEmailApproval(ClientID: any, LinkID: any, Value: any) {
        return this.http.get<any>(`${this.getGetEmailApprovalApiMethod}?ClientID=${ClientID}&LinkID=${LinkID}&Value=${Value}`);
    }

    GetEmailApprovals() {
        return this.http.get<any[]>(`${this.GetEmailApprovalsApiMethod}`);
    }
    GetEmailApprovalByIdAsync(linkID: any) {
        return this.http.get<any[]>(`${this.GetEmailApprovalByIdAsyncApiMethod}/${linkID}`);
    }
    EamilApprovalAction(ClientID: string, LinkID: string, Value: string, Module: string = "Timesheet"): Observable<any>{
        return this.http.get<any>(`${this.EamilApprovalActionApiMethod}/${ClientID}/${LinkID}/${Value}/${Module}`);
    }
    eamilApprovalAction(ClientID: string, LinkID: string, Value: string, Module: string = "Timesheet"): Observable<any> {
        return this.http.get<any>(`${this.EamilApprovalActionApiMethod}/${ClientID}/${LinkID}/${Value}/${Module}`);
       
    }
    SaveEmailApproval(holiday: IEmailApproval): Observable<any> {
        return this.http.post<any>(`${this.SaveEmailApprovalApiMethod}`, holiday);
    }
    UpdateEmailApproval(EmployeeID: number, employee: IEmailApproval): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmailApprovalApiMethod}/${EmployeeID}`, employee);
    }
}
