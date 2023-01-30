import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IProcessData } from '../../core/interfaces/ProcessData';


@Injectable({
  providedIn: 'root'
})

export class ProcessDataService {
    GetProcessDataApiMethod = "api/ProcessData";
    GetWizardDataByIdAsyncApiMethod = "api/ProcessData";
    SaveProcessDataApiMethod = "api/ProcessData";
    UpdateProcessDataApiMethod = "api/ProcessData";
    GetProcessDataListApiMethod = "api/ProcessData"


    constructor(private http: HttpClient) { }

    GetWizardDatas() {
        return this.http.get<any[]>(`${this.GetProcessDataApiMethod}`);
    }

    GetWizardDataByIdAsync(Id: number) {
        console.log(Id)
        return this.http.get<any>(`${this.GetWizardDataByIdAsyncApiMethod}/${Id}`);
    }

    SaveWizardData(processData: IProcessData): Observable<any> {
        return this.http.post<any>(`${this.SaveProcessDataApiMethod}`, processData);
    }
    UpdateProcessData(processDataID: number, processData: IProcessData): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateProcessDataApiMethod}/${processDataID}`, processData);
    }

    GetProcessDataList(Id: number) {
        return this.http.get<any>(`${this.GetProcessDataListApiMethod}/${Id}/${true}`);
    }




}
