import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ITimesheet } from '../../core/interfaces/Timesheet';

@Injectable({
    providedIn: 'root'
})
export class TimesheetService {
    getGetTimeSheetsApiMethod = "api/TimeSheet";
    getTimeSheetByIdApiMethod = "api/TimeSheet";
    UploadFileApiMethod = "api/TimeSheet/Upload";
    DownloadFileApiMethod = "api/TimeSheet/DownloadFile";
    getSaveTimesheetApiMethod = "api/TimeSheet";
    getUpdateTimesheetApiMethod = "api/TimeSheet";
    constructor(private http: HttpClient) { }

    GetTimeSheets(EmployeeID: number, SubmittedByID: number, StatusID?: number) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
                SubmittedByID: SubmittedByID,
                StatusID: 0
            }
        }
        return this.http.get<any[]>(`${this.getGetTimeSheetsApiMethod}?EmployeeID=${EmployeeID}&SubmittedByID=${SubmittedByID}&StatusID=0`);
        //return this.http.get<any[]>(`${this.getGetTimeSheetsApiMethod}`, options);
    }

    GetTimesheetByIdAsync(timesheetID: number): Observable<any> {
        return this.http.get<any>(`${this.getTimeSheetByIdApiMethod}/${timesheetID}`);
    }
    async uploadFile(employeeName: string, formData: FormData) {
        console.log(employeeName.replace(/ +/g, ''), formData)
       return await this.http.post<any>(`${this.UploadFileApiMethod}/${employeeName.replace(/ +/g, '')}`, formData).toPromise()
    }
    SaveTimesheet(expense: ITimesheet): Observable<any> {
        return this.http.post<any>(`${this.getSaveTimesheetApiMethod}`, expense);
    }

    UpdateTimesheet(ExpenseID: number, expense: ITimesheet): Observable<any[]> {
        return this.http.put<any>(`${this.getUpdateTimesheetApiMethod}/${ExpenseID}`, expense);
    }
    DownloadFile(clientName, Doc): Observable<any> {
        return this.http.get<any>(`${this.DownloadFileApiMethod}/?Client=${clientName}&Doc=${Doc}`);
    }
}
