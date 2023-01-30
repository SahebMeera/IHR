export class User {
    userID: number;
    employeeID?: number;
    employeeCode: number;
    firstName: string;
    lastName: string;
    email: string;
    roleName: string;
    roleShort: string;
    password: string;
    isOAuth: boolean;
    isActive: boolean;
    newPassword: string;
    confirmPassword: string;
    clientID: string;
    companyID: number;
    companyName: string;
}


export class UserToken {
    user: User;
    token: string;
}
