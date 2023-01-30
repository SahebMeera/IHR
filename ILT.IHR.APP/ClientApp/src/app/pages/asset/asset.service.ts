import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IAsset } from '../../core/interfaces/Asset';
import { ITicket, ITicketForDisplay } from '../../core/interfaces/Ticket';


@Injectable({
  providedIn: 'root'
})

export class AssetService {
    GetAssetApiMethod = "api/Asset";
    getgetAssetsListApiMethod = "api/Asset";
    SaveAssetApiMethod = "api/Asset";
    UpdateAssetApiMethod = "api/Asset";
    getAssetByIdAsyncApiMethod = "api/Asset";
    AssetChangeSetApiMethod = "api/AssetChangeSet";
 

    constructor(private http: HttpClient) { }

    GetAsset() {
        return this.http.get<any[]>(`${this.GetAssetApiMethod}`);
    }
    getAssetByIdAsync(id: number) {
        return this.http.get<any>(`${this.getAssetByIdAsyncApiMethod}/${id}`)
    }
    GetAssetChangeset(id: number) {
        return this.http.get<any>(`${this.AssetChangeSetApiMethod}/${id}`)
    }

    GetTicketsList(RequestedByID: number, AssignedToID?: number) {
        const options = {
            params: {
                RequestedByID: RequestedByID,
                AssignedToID: AssignedToID
            }
        }
        return this.http.get<any[]>(`${this.getgetAssetsListApiMethod}`, options);
    }

    SaveAsset(asset: IAsset): Observable<any> {
        return this.http.post<any>(`${this.SaveAssetApiMethod}`, asset);
    }
    UpdateAsset(AssetID: number, asset: IAsset): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateAssetApiMethod}/${AssetID}`, asset);
    }


}
