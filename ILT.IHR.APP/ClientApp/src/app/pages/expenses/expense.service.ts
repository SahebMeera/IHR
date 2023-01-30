import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IExpense, IExpenseForDisplay } from '../../core/interfaces/Expense';


@Injectable({
    providedIn: 'root'
})

export class ExpenseService {
    GetExpenseApiMethod = "api/Expense";
    getDepartmentApiMethod = "api/Department";
    SaveExpenseApiMethod = "api/Expense";
    UpdateExpenseApiMethod = "api/Expense";
    UploadFileApiMethod = "api/Expense/Upload";
    DownloadFileApiMethod = "api/Expense/Download";
    getExpenseByIdApiMethod = "api/Expense";
    getSaveExpenseApiMethod = "api/Expense";
    getUpdateExpenseApiMethod = "api/Expense";


    constructor(private http: HttpClient) { }

    GetExpense() {
        return this.http.get<any[]>(`${this.GetExpenseApiMethod}`);
    }


    async uploadFile(employeeName, formData: FormData) {
        return await this.http.post<any>(`${this.UploadFileApiMethod}/${employeeName}`, formData).toPromise();
    }

    DownloadFile(clientName, FileName): Observable<any> {
        return this.http.get<any>(`${this.DownloadFileApiMethod}/${clientName}/${FileName}`);
    }
    GetExpenseByIdAsync(expensesID: number): Observable<any> {
        return this.http.get<any>(`${this.getExpenseByIdApiMethod}/${expensesID}`);
    }

    SaveExpense(expense: IExpense): Observable<any> {
        return this.http.post<any>(`${this.getSaveExpenseApiMethod}`, expense);
    }

    UpdateExpense(ExpenseID: number, expense: IExpense): Observable<any[]> {
        return this.http.put<any>(`${this.getUpdateExpenseApiMethod}/${ExpenseID}`, expense);
    }
}
