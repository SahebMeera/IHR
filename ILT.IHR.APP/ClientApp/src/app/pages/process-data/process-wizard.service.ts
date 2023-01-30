import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})

export class ProcessWizardService {
    GetProcessWizardApiMethod = "api/ProcessWizard";
    GetProcessWizardListApiMethod = "api/ProcessWizard";

    constructor(private http: HttpClient) { }

    GetWizards() {
        return this.http.get<any[]>(`${this.GetProcessWizardApiMethod}`);
    }


    GetProcessWizardList(Id: number) {
        return this.http.get<any>(`${this.GetProcessWizardListApiMethod}/${Id}/${true}`);
    }




}
