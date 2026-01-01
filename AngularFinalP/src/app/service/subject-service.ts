import { Injectable } from '@angular/core';
import { SubjectCreateDto, SubjectReadDto, SubjectUpdateDto } from '../models/subject';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class SubjectService {
  private apiUrl = 'https://localhost:7113/api/subjects'; // Adjust API URL

  constructor(private http: HttpClient) { }

  // Get all point conditions
  getAll(): Observable<SubjectReadDto[]> {
    return this.http.get<SubjectReadDto[]>(this.apiUrl);
  }

  // Get by ID
  getById(id: number): Observable<SubjectReadDto> {
    return this.http.get<SubjectReadDto>(`${this.apiUrl}/${id}`);
  }

  // Create new point condition
  create(pointCondition: SubjectCreateDto): Observable<SubjectReadDto> {
    return this.http.post<SubjectReadDto>(this.apiUrl, pointCondition);
  }

  // Update existing point condition
  update(id: number, pointCondition: SubjectUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, pointCondition);
  }

  // Delete point condition
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}


