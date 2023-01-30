export class IRegistration {
    FirstName: string;
    LastName: string;
    Phone: number;
    Company: string;
    WebSite: string;
    City: string;
    State: string;
    Country: string;
    Zip: number;
}

export class ILogin {
    email: string;
    password: string;
}

export class IConfirmation {
    IsComplete: boolean;
    IsActivated: boolean;
}

export class IRegistrationDisplay {
    email?: string;
    password?: string;
    FirstName?: string;
    LastName?: string;
    Phone?: number;
    Company?: string;
    WebSite?: string;
    City?: string;
    State?: string;
    Country?: string;
    Zip?: number;
    IsComplete?: boolean;
    IsActivated?: boolean;
}

