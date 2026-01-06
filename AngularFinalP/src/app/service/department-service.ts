import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { DepartmentReadDto, DepartmentCreateDto, DepartmentUpdateDto } from '../models/department';

@Injectable({
  providedIn: 'root',
})
export class DepartmentService {
  private apiUrl = 'https://localhost:7113/api/department';

  constructor(private http: HttpClient) { }

  getAll(): Observable<DepartmentReadDto[]> {
    return this.http.get<{ data: DepartmentReadDto[] }>(this.apiUrl)
      .pipe(map(res => res.data));
  }

  getById(id: number): Observable<DepartmentReadDto> {
    return this.http.get<{ data: DepartmentReadDto }>(`${this.apiUrl}/${id}`)
      .pipe(map(res => res.data));
  }

  create(dto: DepartmentCreateDto): Observable<DepartmentReadDto> {
    return this.http.post<{ data: DepartmentReadDto }>(this.apiUrl, dto)
      .pipe(map(res => res.data));
  }

  update(id: number, dto: DepartmentUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, dto);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
