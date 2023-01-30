import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ITimesheet } from '../../core/interfaces/Timesheet';

@Injectable({
    providedIn: 'root'
})
export class EmailApprovalService {
    getGetEmailApprovalApiMethod = "api/EmailApproval/EmailApproval";
    //getTimeSheetByIdApiMethod = "api/TimeSheet";
    //UploadFileApiMethod = "api/TimeSheet/Upload";
    //DownloadFileApiMethod = "api/TimeSheet/DownloadFile";
    //getSaveTimesheetApiMethod = "api/TimeSheet";
    //getUpdateTimesheetApiMethod = "api/TimeSheet";
    constructor(private http: HttpClient) { }

   getEmailApproval(ClientID: string, LinkID: string, Value?: string) {
      
       return this.http.get<any[]>(`${this.getGetEmailApprovalApiMethod}/${ClientID}/${LinkID}/${Value}`);
        //return this.http.get<any[]>(`${this.getGetTimeSheetsApiMethod}`, options);
    }

}
