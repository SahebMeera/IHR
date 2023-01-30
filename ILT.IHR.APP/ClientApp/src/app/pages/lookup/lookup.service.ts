import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IListValue } from '../../core/interfaces/ListValue';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class LookUpService {
    getLookupApiMethod = "api/ListType";
    getLookupByIdAsyncApiMethod = "api/ListType";
    getListValueByIdAsyncApiMethod = "api/ListValue";
    getListValuesApiMethod = "api/ListValue";
    SaveLookupApiMethod = "api/ListValue";
    UpdateLookupApiMethod = "api/ListValue";

  constructor(private http: HttpClient) { }

    getLookUpList() {
        return this.http.get<any[]>(`${this.getLookupApiMethod}`);
    }
    getListValues() {
        return this.http.get<any[]>(`${this.getListValuesApiMethod}`);
    }
    getLookupByIdAsync(typeID: number) {
        return this.http.get<any[]>(`${this.getLookupByIdAsyncApiMethod}/${typeID}`);
    }
    getListValueByIdAsync(listValueID: number) {
        return this.http.get<any[]>(`${this.getListValueByIdAsyncApiMethod}/${listValueID}`);
    }
    SaveLookup(holiday: IListValue): Observable<any> {
        return this.http.post<any>(`${this.SaveLookupApiMethod}`, holiday);
    }
    UpdateLookup(client: IListValue): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateLookupApiMethod}/${client.ListValueID}`, client);
    }
}
