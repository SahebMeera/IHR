export class IRolePermissionDisplay {
    rolePermissionID: number;
    roleID: number;
    roleName: string;
    moduleID: number;
    moduleShort: string;
    moduleName: string;
    view: boolean;
    add: boolean;
    update: boolean;
    delete: boolean;
}

export class IRolePermission {
    RolePermissionID: number;
    RoleID: number;
    ModuleID: number;
    View: boolean;
    Add: boolean;
    Update: boolean;
    Delete: boolean;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: string;
}

