import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ExamRoutineCreateDto,
  ExamRoutineUpdateDto,
  ExamRoutineMasterReadDto,
  ExamRoutineSubjectDetailReadDto
} from '../models/ExamRoutine';

@Injectable({
  providedIn: 'root'
})
export class ExamroutineService {
  private apiUrl = 'https://localhost:7113/api/exammroutine';

  constructor(private http: HttpClient) { }

  // =========================
  // MASTER–DETAILS READ
  // =========================
  getMasterDetails(): Observable<ExamRoutineMasterReadDto[]> {
    return this.http.get<ExamRoutineMasterReadDto[]>(`${this.apiUrl}/master-details`);
  }

  // =========================
  // GET SINGLE ROW (optional)
  // =========================
  getById(id: number): Observable<ExamRoutineMasterReadDto> {
    // <-- change return type to Master + subjects
    return this.http.get<ExamRoutineMasterReadDto>(`${this.apiUrl}/master-details/${id}`);
  }

  // =========================
  // CREATE SINGLE SUBJECT (details row)
  // =========================
  create(dto: ExamRoutineCreateDto): Observable<any> {
    return this.http.post(this.apiUrl, dto);
  }

  // =========================
  // UPDATE SINGLE SUBJECT
  // =========================
  update(dto: ExamRoutineUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${dto.examRoutineId}`, dto);
  }

  // =========================
  // DELETE SINGLE SUBJECT
  // =========================
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
