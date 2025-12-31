import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  PointConditionReadDto,
  PointConditionCreateDto,
  PointConditionUpdateDto
} from '../models/pointcondition';

@Injectable({
  providedIn: 'root',
})
export class PointService {
  private apiUrl = 'https://localhost:7113/api/point'; // Adjust API URL

  constructor(private http: HttpClient) { }

  // Get all point conditions
  getAll(): Observable<PointConditionReadDto[]> {
    return this.http.get<PointConditionReadDto[]>(this.apiUrl);
  }

  // Get by ID
  getById(id: number): Observable<PointConditionReadDto> {
    return this.http.get<PointConditionReadDto>(`${this.apiUrl}/${id}`);
  }

  // Create new point condition
  create(pointCondition: PointConditionCreateDto): Observable<PointConditionReadDto> {
    return this.http.post<PointConditionReadDto>(this.apiUrl, pointCondition);
  }

  // Update existing point condition
  update(id: number, pointCondition: PointConditionUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, pointCondition);
  }

  // Delete point condition
  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
