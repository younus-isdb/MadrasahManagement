export interface DepartmentCreateDto {
  departmentName: string;
  description?: string;
}

export interface DepartmentUpdateDto {
  departmentId: number;
  departmentName: string;
  description?: string;
}

export interface DepartmentReadDto {
  departmentId: number;
  departmentName: string;
  description?: string;
}
