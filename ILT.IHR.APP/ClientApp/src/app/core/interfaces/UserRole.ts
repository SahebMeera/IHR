
export class IUserRole {
    UserRoleID: number;
    UserID?: number;
    RoleID: number;
    RoleShort: string;
    RoleName: string;
    IsDefault: boolean;
    IsSelected: boolean;
    CreatedDate: Date;
    CreatedBy: string;
    ModifiedDate: Date;
    ModifiedBy: string;
}
export class IUserRoleDisplay {
    userRoleID: number;
    userID?: number;
    roleID: number;
    roleShort: string;
    roleName: string;
    isDefault: boolean;
    isSelected: boolean;
    createdDate: Date;
    createdBy: string;
    modifiedDate: Date;
    modifiedBy: string;
}
