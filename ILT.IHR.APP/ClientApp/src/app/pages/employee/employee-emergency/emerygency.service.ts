import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IEmergnecyContact } from '../../../core/interfaces/EmergencyContact';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class EmerygencyService {
    getContactApiMethod = "api/Contact";
    getCountryApiMethod = "api/Country";
    getCountryByIdApiMethod = "api/Country";
    getContactByIdAsyncApiMethod = "api/Contact/?:id";
    SaveContactApiMethod = "api/Contact"
    UpdateContactApiMethod = "api/Contact"

  constructor(private http: HttpClient) { }

    getContactList() {
        return this.http.get<any[]>(`${this.getContactApiMethod}`);
    }
    getcontactByIdAsync(contactID: number, employeeId: number) {
        const options = {
            params: {
                employeeId: employeeId,
            }
        }
        return this.http.get<any[]>(`api/Contact/${contactID}`, options);
    }
    getCountry() {
        return this.http.get<any[]>(`${this.getCountryApiMethod}`);
    }
    GetCountryByIdAsync(countryId: number) {
        return this.http.get<any[]>(`${this.getCountryByIdApiMethod}/${countryId}`);
    }

    SaveContact(conatct: IEmergnecyContact): Observable<any> {
        return this.http.post<any>(`${this.SaveContactApiMethod}`, conatct);
    }
    UpdateContact(client: IEmergnecyContact): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateContactApiMethod}/${client.ContactID}`, client);
    }

}
