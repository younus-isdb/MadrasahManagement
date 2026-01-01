import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StudentCreateDto, StudentReadDto, StudentUpdateDto } from '../models/student';

@Injectable({
  providedIn: 'root',
})
export class StudentService {
  private apiUrl = 'https://localhost:7113/api/students'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<StudentReadDto[]> {
    return this.http.get<StudentReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<StudentReadDto> {
    return this.http.get<StudentReadDto>(`${this.apiUrl}/${id}`);
  }

  create(exam: StudentCreateDto): Observable<StudentReadDto> {
    return this.http.post<StudentReadDto>(this.apiUrl, exam);
  }

  update(id: number, exam: StudentUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, exam);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
  
}
