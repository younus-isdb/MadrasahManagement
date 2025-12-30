import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExaminationReadDto, ExaminationCreateDto, ExaminationUpdateDto } from '../models/examination';

@Injectable({
  providedIn: 'root',
})
export class ExaminationService {
  private apiUrl = 'https://localhost:7113/api/examinatiion'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<ExaminationReadDto[]> {
    return this.http.get<ExaminationReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ExaminationReadDto> {
    return this.http.get<ExaminationReadDto>(`${this.apiUrl}/${id}`);
  }

  create(exam: ExaminationCreateDto): Observable<ExaminationReadDto> {
    return this.http.post<ExaminationReadDto>(this.apiUrl, exam);
  }

  update(id: number, exam: ExaminationUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, exam);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}

