import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()

@Injectable({
    providedIn: 'root'
})
export class EndClientService {
    getEndClientApiMethod = "api/EndClient";
    getEndClientByIdApiMethod = "api/EndClient";
    SaveEndClientApiMethod = "api/EndClient";
    UpdateEndClientApiMethod = "api/EndClient";

    constructor(private http: HttpClient) { }


    GetEndClients() {
        return this.http.get<any[]>(`${this.getEndClientApiMethod}`);
    }
    getCompanies() {
        return this.http.get<any>('assets/demo/data/CompanyInfo.json')
            .toPromise()
            .then(res => res.data as any[])
            .then(data => data);
    }

    GetEndClientByIdAsync(companyID: number): Observable<any> {
        return this.http.get<any>(`${this.getEndClientByIdApiMethod}/${companyID}`);
    }
    SaveEndClient(company: any): Observable<any> {
        return this.http.post<any>(`${this.SaveEndClientApiMethod}`, company);
    }
    UpdateEndClient(CompanyID: number, Company: any): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEndClientApiMethod}/${CompanyID}`, Company);
    }
}
