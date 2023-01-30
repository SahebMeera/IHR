import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ITicket, ITicketForDisplay } from '../../core/interfaces/Ticket';


@Injectable({
  providedIn: 'root'
})

export class TicketService {
    GetTicketApiMethod = "api/Ticket";
    getgetTicketsListApiMethod = "api/Ticket";
    SaveTicketApiMethod = "api/Ticket";
    UpdateTicketApiMethod = "api/Ticket";
    getTicketByIdAsyncApiMethod = "api/Ticket";
 

    constructor(private http: HttpClient) { }

    GetTicket() {
        return this.http.get<any[]>(`${this.GetTicketApiMethod}`);
    }
    getTicketByIdAsync(id: number) {
        return this.http.get<any>(`${this.getTicketByIdAsyncApiMethod}/${id}`)
    }

    GetTicketsList(RequestedByID: number, AssignedToID?: number) {
        const options = {
            params: {
                RequestedByID: RequestedByID,
                AssignedToID: AssignedToID
            }
        }
        return this.http.get<any[]>(`${this.getgetTicketsListApiMethod}`, options);
    }

    SaveTicket(holiday: ITicket): Observable<any> {
        return this.http.post<any>(`${this.SaveTicketApiMethod}`, holiday);
    }
    UpdateTicket(TicketID: number, employee: ITicket): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateTicketApiMethod}/${TicketID}`, employee);
    }


}
