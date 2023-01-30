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
export class SkillService {
    getEmployeeSkillApiMethod = "api/EmployeeSkill";
    getEmployeeSkillByIdAsyncApiMethod = "api/EmployeeSkill";
    SaveEmployeeSkillApiMethod = "api/EmployeeSkill"
    UpdateEmployeeSkillApiMethod = "api/EmployeeSkill"

  constructor(private http: HttpClient) { }
    getEmployeeSkill(EmployeeID: number) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
            }
        }
        return this.http.get<any[]>(`${this.getEmployeeSkillApiMethod}`, options);
    }
   
    getEmployeeSkillByIdAsync(EmployeeSkillID: number) {
        return this.http.get<any[]>(`${this.getEmployeeSkillByIdAsyncApiMethod}/${EmployeeSkillID}`);
    }
  
    SaveEmployeeSkill(dependent: any): Observable<any> {
        return this.http.post<any>(`${this.SaveEmployeeSkillApiMethod}`, dependent);
    }
    UpdateEmployeeSkill(dependent: any): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmployeeSkillApiMethod}/${dependent.EmployeeSkillID}`, dependent);
    }

}
