import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IEmployeeDependent } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class DependentService {
    geDependentByIdAsyncApiMethod = "api/Dependent";
    SaveDependentApiMethod = "api/Dependent"
    UpdateDependentApiMethod = "api/Dependent"

  constructor(private http: HttpClient) { }

   
    getDependentByIdAsync(directDepositID: number) {
        //const options = {
        //    params: {
        //        employeeId: employeeId,
        //    }
        //}
        return this.http.get<any[]>(`${this.geDependentByIdAsyncApiMethod}/${directDepositID}`);
    }
  
    SaveDependent(dependent: IEmployeeDependent): Observable<any> {
        return this.http.post<any>(`${this.SaveDependentApiMethod}`, dependent);
    }
    UpdateDependent(dependent: IEmployeeDependent): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateDependentApiMethod}/${dependent.DependentID}`, dependent);
    }

}
