import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IRolePermission } from '../../core/interfaces/RolePermission';

@Injectable()

@Injectable({
  providedIn: 'root'
})
export class RolePermissionService {
    getRoleApiMethod = "api/Role";
    getRoleByIdAsyncApiMethod = "api/Role";
    getModuleApiMethod = "api/Module";
    getRolePermissionApiMethod = "api/RolePermission";
    SaveRolePermissionApiMethod = "api/RolePermission";
    UpdateRolePermissionApiMethod = "api/RolePermission";

  constructor(private http: HttpClient) { }

    getRoles() {
        return this.http.get<any[]>(`${this.getRoleApiMethod}`);
    }
    getModules() {
        return this.http.get<any[]>(`${this.getModuleApiMethod}`);
    }
    getRoleByIdAsync(typeID: number) {
        return this.http.get<any[]>(`${this.getRoleByIdAsyncApiMethod}/${typeID}`);
    }

    getRolePermission() {
        return this.http.get<any[]>(`${this.getRolePermissionApiMethod}`);
    }

    getRolePermissionByIdAsync(rolePermissionID: number) {
        return this.http.get<any[]>(`${this.getRolePermissionApiMethod}/${rolePermissionID}`);
    }
    SaveRolePermission(rolePermission: IRolePermission): Observable<any> {
        return this.http.post<any>(`${this.SaveRolePermissionApiMethod}`, rolePermission);
    }
    UpdateRolePermission(client: IRolePermission): Observable<any[]> {
        return this.http.put<any>(`${this.UpdateRolePermissionApiMethod}/${client.RolePermissionID}`, client);
    }

}
