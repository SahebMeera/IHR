import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class DirectDepositService {
    getDirectDepositByIdAsyncApiMethod = "api/DirectDeposit";
    SaveDirectDepositApiMethod = "api/DirectDeposit"
    UpdateDirectDeposittApiMethod = "api/DirectDeposit"

  constructor(private http: HttpClient) { }

   
    getDirectDepositByIdAsync(directDepositID: number) {
        //const options = {
        //    params: {
        //        employeeId: employeeId,
        //    }
        //}
        return this.http.get<any[]>(`${this.getDirectDepositByIdAsyncApiMethod}/${directDepositID}`);
    }
  
    SaveDirectdeposit(direct: IDirectDeposit): Observable<any> {
        return this.http.post<any>(`${this.SaveDirectDepositApiMethod}`, direct);
    }
    UpdateDirectdeposit(direct: IDirectDeposit): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateDirectDeposittApiMethod}/${direct.DirectDepositID}`, direct);
    }

}
