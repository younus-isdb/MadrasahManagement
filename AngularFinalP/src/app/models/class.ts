// DTO sent to API when creating a new class
export interface ClassCreateDto {
  className: string;
  sessionYear?: string;   // optional
  departmentId: number;
}

// DTO sent to API when updating an existing class
export interface ClassUpdateDto extends ClassCreateDto {
  classId: number;
}

// DTO received from API when reading classes
export interface ClassReadDto {
  classId: number;
  className: string;
  sessionYear?: string;
  departmentId: number;
  departmentName: string;
}
