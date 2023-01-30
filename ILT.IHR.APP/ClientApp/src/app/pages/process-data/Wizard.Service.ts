import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';


@Injectable({
    providedIn: 'root'
})

export class WizardService {
    GetWizardsApiMethod = "api/ProcessWizard";
  //  GetWizardDataByIdAsyncApiMethod = "api/Employee";

    constructor(private http: HttpClient) { }

    GetWizards() {
        return this.http.get<any[]>(`${this.GetWizardsApiMethod}`);
    }

    //GetWizardDataByIdAsync(Id: number) {
    //    return this.http.get<any[]>(`${this.GetWizardDataByIdAsyncApiMethod}/${Id}`);
    //}



}
