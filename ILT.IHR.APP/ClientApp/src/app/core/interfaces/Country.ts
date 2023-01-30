import { IStateDisplay } from "./States";

export class ICountry {
    HolidayID: number;
    Name: string;
    StartDate: string;
    Country: string;
}
export class ICountryDisplay {
    countryID: number;
    countryDesc: string;
    States: IStateDisplay[]
}
