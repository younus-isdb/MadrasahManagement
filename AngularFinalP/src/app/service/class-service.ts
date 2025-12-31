import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ClassReadDto,ClassUpdateDto,ClassCreateDto } from '../models/class';

@Injectable({
  providedIn: 'root',
})
export class ClassService {
  private apiUrl = 'https://localhost:7113/api/classes'; // আপনার API URL

  constructor(private http: HttpClient) { }

  getAll(): Observable<ClassReadDto[]> {
    return this.http.get<ClassReadDto[]>(this.apiUrl);
  }

  getById(id: number): Observable<ClassReadDto> {
    return this.http.get<ClassReadDto>(`${this.apiUrl}/${id}`);
  }

  create(examfee: ClassCreateDto): Observable<ClassCreateDto> {
    return this.http.post<ClassReadDto>(this.apiUrl, examfee);
  }

  update(id: number, examfee: ClassUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, examfee);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
