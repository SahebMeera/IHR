import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { CommonUtils } from '../../common/common-utils';
import { IAppraisal } from '../../core/interfaces/Appraisal';
import { ILeave } from '../../core/interfaces/Leave';

@Injectable({
    providedIn: 'root'
})
export class AppraisalService {
    commonUtils = new CommonUtils();
    getAppraisalApiMethod = "api/Appraisal";
    getGetAppraisalDaysApiMethod = "api/Leave/GetLeaveDays";
    getGetAppraisalByIdAsyncApiMethod = "api/Appraisal";
    getSaveAppraisalApiMethod = "api/Appraisal";
    getUpdateAppraisalApiMethod = "api/Appraisal";

    constructor(private http: HttpClient) { }

    GetAppraisalList(ID: number) {
        return this.http.get<any[]>(`${this.getAppraisalApiMethod}?EmployeeID=${ID}`);
    }
    GetAppraisalById(id: number) {

        return this.http.get<any>(`${this.getGetAppraisalByIdAsyncApiMethod}/${id}`)
    }

    SaveAppraisal(appraisal: IAppraisal): Observable<any> {
        return this.http.post<any>(`${this.getSaveAppraisalApiMethod}`, appraisal);
    }
    UpdateAppraisal(ID: number, appraisal: IAppraisal): Observable<any[]> {
        return this.http.put<any>(`${this.getUpdateAppraisalApiMethod}/${ID}`, appraisal);
    }
}
