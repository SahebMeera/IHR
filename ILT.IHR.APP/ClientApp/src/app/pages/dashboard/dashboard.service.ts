import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, observable } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class DashboardService {
    getDashboardApiMethod = "api/Dashboard";

    constructor(private http: HttpClient) { }

    //getDashboardCounts(CompanyID: number): Observable<IDashboardDisplay> {
    //    return this.http.get<IDashboardDisplay>(`${this.getDashboardApiMethod}/${CompanyID}`);
    //}
}
