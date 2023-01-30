import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IEmployeeDependent } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';
import { ISalary } from '../../../core/interfaces/EmployeeSalary';
import { IEmployeeW4, IEmployeeW4Display } from '../../../core/interfaces/Employeew4';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class EmployeeSalaryService {
    getEmployeeW4ApiMethod = "api/EmployeeW4";
    getEmployeeSalaryByIdAsyncApiMethod = "api/Salary";
    SaveEmployeeSalaryApiMethod = "api/Salary"
    UpdateEmployeeSalaryApiMethod = "api/Salary"

  constructor(private http: HttpClient) { }
    getEmployeeW4s(EmployeeID: number) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
            }
        }
        return this.http.get<any[]>(`${this.getEmployeeW4ApiMethod}`, options);
    }
   
    getEmployeeSalaryById(SalaryID: number) {
        return this.http.get<any[]>(`${this.getEmployeeSalaryByIdAsyncApiMethod}/${SalaryID}`);
    }
  
    SaveSalary(salary: ISalary): Observable<any> {
        return this.http.post<any>(`${this.SaveEmployeeSalaryApiMethod}`, salary);
    }
    UpdateSalary(salary: ISalary): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmployeeSalaryApiMethod}/${salary.SalaryID}`, salary);
    }

}
