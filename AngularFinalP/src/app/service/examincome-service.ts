import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamIncomeCreateDto, ExamIncomeReadDto, ExamIncomeUpdateDto } from '../models/ExamIncome';


@Injectable({
  providedIn: 'root',
})
export class ExamincomeService {
  private apiUrl = 'https://localhost:7113/api/examexpense'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<ExamIncomeReadDto[]> {
    return this.http.get<ExamIncomeReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ExamIncomeReadDto> {
    return this.http.get<ExamIncomeReadDto>(`${this.apiUrl}/${id}`);
  }

  create(examfee: ExamIncomeCreateDto): Observable<ExamIncomeCreateDto> {
    return this.http.post<ExamIncomeReadDto>(this.apiUrl, examfee);
  }

  update(id: number, examfee: ExamIncomeUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, examfee);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
