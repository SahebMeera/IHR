import { IRolePermissionDisplay } from "./RolePermission";

export class IRoleDisplay {
    roleID: number;
    roleShort: string;
    roleName: string;
    rolePermissions: IRolePermissionDisplay[];
}



