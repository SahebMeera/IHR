import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IHoliday } from '../../core/interfaces/Holiday';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class HolidayService {
    getHolidayApiMethod = "api/Holiday";
    getCountryApiMethod = "api/Country";
    getCountryByIdApiMethod = "api/Country";
    getHolidayByIdAsyncApiMethod = "api/Holiday";
    SaveHolidayApiMethod = "api/Holiday"
    UpdateHolidayApiMethod = "api/Holiday"

  constructor(private http: HttpClient) { }

    getHolidayList() {
        return this.http.get<any[]>(`${this.getHolidayApiMethod}`);
    }
    getHolidayByIdAsync(holiday: number) {
        return this.http.get<any[]>(`${this.getHolidayByIdAsyncApiMethod}/${holiday}`);
    }
    getCountry() {
        return this.http.get<any[]>(`${this.getCountryApiMethod}`);
    }
    GetCountryByIdAsync(countryId: number) {
        return this.http.get<any[]>(`${this.getCountryByIdApiMethod}/${countryId}`);
    }

    SaveHoliday(holiday: IHoliday): Observable<any> {
        return this.http.post<any>(`${this.SaveHolidayApiMethod}`, holiday);
    }
    UpdateHoliday(client: IHoliday): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateHolidayApiMethod}/${client.HolidayID}`, client);
    }

}
