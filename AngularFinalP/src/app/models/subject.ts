// subject.model.ts

export interface SubjectCreateDto {
  classId: number;
  departmentId: number;
  subjectName: string;
  subjectCode: string;
  isOptional?: boolean; // default false
}

export interface SubjectUpdateDto extends SubjectCreateDto {
  subjectId: number;
}

export interface SubjectReadDto {
  subjectId: number;
  classId: number;
  className: string;
  departmentId: number;
  departmentName: string;
  subjectName: string;
  subjectCode: string;
  isOptional: boolean;
}
