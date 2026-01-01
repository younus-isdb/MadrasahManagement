import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamRoutineCreateDto, ExamRoutineReadDto, ExamRoutineUpdateDto } from '../models/ExamRoutine';

@Injectable({
  providedIn: 'root',
})
export class ExamroutineService {
  private apiUrl = 'https://localhost:7113/api/exammroutine';
  constructor(private http: HttpClient) { }

  getAll(): Observable<ExamRoutineReadDto[]> {
    return this.http.get<ExamRoutineReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ExamRoutineReadDto> {
    return this.http.get<ExamRoutineReadDto>(`${this.apiUrl}/${id}`);
  }

  create(exam: ExamRoutineCreateDto): Observable<ExamRoutineReadDto> {
    return this.http.post<ExamRoutineReadDto>(this.apiUrl, exam);
  }

  update(id: number, exam: ExamRoutineUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, exam);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
