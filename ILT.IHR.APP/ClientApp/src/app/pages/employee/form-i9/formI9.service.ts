import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IEmployeeDependent } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';
import { IEmployeeW4, IEmployeeW4Display } from '../../../core/interfaces/Employeew4';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class FormI9Service {
    getFormI9ApiMethod = "api/FormI9";
    getI9ExpiryFormApiMethod = "api/FormI9/GetI9ExpiryForm";
    getFormI9ByIdAsyncApiMethod = "api/FormI9";
    FormI9ChangeSetApiMethod = "api/FormI9ChangeSet";
    SaveFormI9ApiMethod = "api/FormI9"
    UpdateFormI9ApiMethod = "api/FormI9"

  constructor(private http: HttpClient) { }
    getFormI9s(EmployeeID: number) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
            }
        }
        return this.http.get<any[]>(`${this.getFormI9ApiMethod}`, options);
    }

    getI9ExpiryForm(expirydate: any) {
        const options = {
            params: {
                expirydate: expirydate,
            }
        }
        return this.http.get<any[]>(`${this.getI9ExpiryFormApiMethod}`, options);
    }
   
    getFormI9ByIdAsync(directDepositID: number) {
        return this.http.get<any[]>(`${this.getFormI9ByIdAsyncApiMethod}/${directDepositID}`);
    }
    GetFormI9Changeset(I9formID: number) {
        return this.http.get<any[]>(`${this.FormI9ChangeSetApiMethod}/${I9formID}`);
    }
  
    SaveFormI9(dependent: any): Observable<any> {
        return this.http.post<any>(`${this.SaveFormI9ApiMethod}`, dependent);
    }
    UpdateFormI9(dependent: any): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateFormI9ApiMethod}/${dependent.FormI9ID}`, dependent);
    }

}
