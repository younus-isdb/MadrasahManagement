import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ExamFeesCreateDto, ExamFeesUpdateDto, ExamFeesReadDto } from '../models/examfeeCollection';

@Injectable({
  providedIn: 'root'
})
export class ExamfeecollectionService {
  private apiUrl = 'https://localhost:7113/api/examfeecollection'; // আপনার API URL

  constructor(private http: HttpClient) { }

  // ---------------- Create ExamFee with FeeCollections ----------------
  create(examFee: ExamFeesCreateDto): Observable<ExamFeesReadDto> {
    return this.http.post<ExamFeesReadDto>(this.apiUrl, examFee);
  }

  // ---------------- Get ExamFee by Id (with nested collections) ----------------
  getById(id: number): Observable<ExamFeesReadDto> {
    return this.http.get<ExamFeesReadDto>(`${this.apiUrl}/${id}`);
  }

  // ---------------- Get all ExamFees (with nested collections) ----------------
  getAll(): Observable<ExamFeesReadDto[]> {
    return this.http.get<ExamFeesReadDto[]>(this.apiUrl);
  }

  // ---------------- Update ExamFee + nested collections ----------------
  update(id: number, examFee: ExamFeesUpdateDto): Observable<ExamFeesReadDto> {
    return this.http.put<ExamFeesReadDto>(`${this.apiUrl}/${id}`, examFee);
  }

  // ---------------- Delete ExamFee ----------------
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
