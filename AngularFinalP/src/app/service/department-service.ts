import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DepartmentCreateDto, DepartmentReadDto, DepartmentUpdateDto } from '../models/department';


@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private apiUrl = 'https://localhost:7113/api/department'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<DepartmentReadDto[]> {
    return this.http.get<DepartmentReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<DepartmentReadDto> {
    return this.http.get<DepartmentReadDto>(`${this.apiUrl}/${id}`);
  }

  create(exam: DepartmentCreateDto): Observable<DepartmentReadDto> {
    return this.http.post<DepartmentReadDto>(this.apiUrl, exam);
  }

  update(id: number, exam: DepartmentUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, exam);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
