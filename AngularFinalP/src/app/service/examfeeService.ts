import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamFeeReadDto, ExamFeeCreateDto, ExamFeeUpdateDto } from '../models/examfee';


@Injectable({
  providedIn: 'root',
})
export class ExamfeeService {
  private apiUrl = 'https://localhost:7113/api/examfee'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<ExamFeeReadDto[]> {
    return this.http.get<ExamFeeReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ExamFeeReadDto> {
    return this.http.get<ExamFeeReadDto>(`${this.apiUrl}/${id}`);
  }

  create(examfee: ExamFeeCreateDto): Observable<ExamFeeReadDto> {
    return this.http.post<ExamFeeReadDto>(this.apiUrl, examfee);
  }

  update(id: number, examfee: ExamFeeUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, examfee);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
