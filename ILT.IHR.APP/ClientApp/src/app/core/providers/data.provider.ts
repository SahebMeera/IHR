import { Injectable } from '@angular/core';

@Injectable()
export class DataProvider {
    public storage: any;
    public caseSearchInfo: any;
    public EmployeeFilters: any;
    public country: string
    public status: string
    public employeeType: any[]
    public DefaultPageSize: number;
    public table: any;
    public TabIndex: number;
    public constructor(){
    }
}
