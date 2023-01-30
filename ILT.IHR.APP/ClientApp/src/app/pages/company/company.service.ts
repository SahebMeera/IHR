import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ICompany } from '../../core/interfaces/company';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  getCompanyApiMethod = "api/Company";
  getCompanyByIdApiMethod = "api/Company";
  SaveCompanyApiMethod = "api/Company";
  UpdateCompanyApiMethod = "api/Company";
  getEndClientApiMethod = "api/EndClient";

  constructor(private http: HttpClient) { }


  getCompanyList() {
    return this.http.get<any[]>(`${this.getCompanyApiMethod}`);
    }
    getEndClientList() {
        return this.http.get<any[]>(`${this.getEndClientApiMethod}`);
    }
    GetCompanyByIdAsync(companyID: number): Observable<any> {
        return this.http.get<any>(`${this.getCompanyByIdApiMethod}/${companyID}`);
    }
    //getCompanies() {
    //return this.http.get<any>('Companys/demo/data/CompanyInfo.json')
    //  .toPromise()
    //  .then(res => res.data as any[])
    //  .then(data => data);
    //}

    SaveCompany(company: ICompany): Observable<any> {
        return this.http.post<any>(`${this.SaveCompanyApiMethod}`, company);
    }
    UpdateCompany(CompanyID: number, Company: ICompany): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateCompanyApiMethod}/${CompanyID}`, Company);
    }
}
