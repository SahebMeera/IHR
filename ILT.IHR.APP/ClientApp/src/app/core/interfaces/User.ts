import { IUserRoleDisplay } from "./UserRole";

export class IUser {
    UserID: number;
    EmployeeID?: number;
    EmployeeCode?: number;
    Email: string;
    FirstName: string;
    LastName: string;
    RoleName: string;
    RoleShort: string;
    Password: string;
    IsOAuth: boolean;
    IsActive: boolean;
    NewPassword: string;
    ConfirmPassword?: string;
    ClientID: string;
    CompanyID?: number;
    CompanyName?: string;
    CreatedDate: Date;
    CreatedBy: string;
    ModifiedDate: Date;
    ModifiedBy: string;
    
}
export class IUserDisplay {
    UserID: number;
    EmployeeID?: number;
    EmployeeCode?: number;
    Email: string;
    FirstName: string;
    LastName: string;
    RoleName: string;
    RoleShort: string;
    Password: string;
    IsOAuth: boolean;
    IsActive: boolean;
    NewPassword: string;
    ConfirmPassword: string;
    ClientID: string;
    CompanyID?:number;
    CompanyName?: string;
    UserRoles: IUserRoleDisplay[]
    CreatedDate: Date;
    CreatedBy: string;
    ModifiedDate: Date;
    ModifiedBy: string;
}
