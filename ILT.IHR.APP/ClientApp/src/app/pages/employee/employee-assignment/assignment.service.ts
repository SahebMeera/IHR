import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAssignmentRate } from '../../../core/interfaces/AssignmentRate';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';
import { IEmployeeAssignment } from '../../../core/interfaces/EmployeeAssignment';
import { IEmployeeDependent } from '../../../core/interfaces/EmployeeDependent';
import { IDirectDeposit } from '../../../core/interfaces/EmployeeDirectDeposit';
import { IEmployeeW4, IEmployeeW4Display } from '../../../core/interfaces/Employeew4';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class EmployeeAssignmentService {
    getAssignmentApiMethod = "api/Assignment";
    getEmployeeAssignmentByIdApiMethod = "api/Assignment";
    GetAssignmentRateByIdApiMethod = "api/AssignmentRate";
    SaveEmployeeAssignmentApiMethod = "api/Assignment"
    UpdateEmployeeAssignmentApiMethod = "api/Assignment"
    SaveAssignmentRateApiMethod = "api/AssignmentRate"
    UpdateAssignmentRateApiMethod = "api/AssignmentRate"


  constructor(private http: HttpClient) { }
    getEmployeeAssignments(EmployeeID: number) {
        const options = {
            params: {
                EmployeeID: EmployeeID,
            }
        }
        return this.http.get<any[]>(`${this.getAssignmentApiMethod}`, options);
    }
   
    getEmployeeAssignmentById(assignmentID: number) {
        return this.http.get<any[]>(`${this.getEmployeeAssignmentByIdApiMethod}/${assignmentID}`);
    }
    GetAssignmentRateById(assignmentRateID: number) {
        return this.http.get<any[]>(`${this.GetAssignmentRateByIdApiMethod}/${assignmentRateID}`);
    }
  
    SaveEmployeeAssignment(assignment: IEmployeeAssignment): Observable<any> {
        return this.http.post<any>(`${this.SaveEmployeeAssignmentApiMethod}`, assignment);
    }
    UpdateEmployeeAssignment(assignment: IEmployeeAssignment): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateEmployeeAssignmentApiMethod}/${assignment.AssignmentID}`, assignment);
    }

    SaveAssignmentRate(assignment: IAssignmentRate): Observable<any> {
        return this.http.post<any>(`${this.SaveAssignmentRateApiMethod}`, assignment);
    }
    UpdateAssignmentRate(assignment: IAssignmentRate): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateAssignmentRateApiMethod}/${assignment.AssignmentRateID}`, assignment);
    }

}
