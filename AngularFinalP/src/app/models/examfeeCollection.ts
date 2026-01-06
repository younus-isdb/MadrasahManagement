// =================== Fee Collection DTOs ===================

// Create student fee
export interface ExamFeeCollectionCreateDto {
  studentId: number;
  examFeeAmount: number;
  totalSubject: number;
}

// Update student fee
export interface ExamFeeCollectionUpdateDto {
  feeCollectionId?: number; // undefined = new
  studentId: number;
  examFeeAmount: number;
  totalSubject: number;
}

// Read student fee
export interface ExamFeeCollectionReadDto {
  feeCollectionId: number;
  studentId: number;
  studentName: string;
  examFeeAmount: number;
  totalSubject: number;
}

// =================== Exam Fee DTOs ===================

// Create exam fee
export interface ExamFeesCreateDto {
  educationYear: string;
  departmentId: number;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionCreateDto[];
}

// Update exam fee
export interface ExamFeesUpdateDto {
  educationYear: string;
  departmentId: number;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionUpdateDto[];
}

// Read exam fee
export interface ExamFeesReadDto {
  examFeeId: number;
  educationYear: string;
  departmentId: number;
  departmentName: string;
  classId: number;
  className: string;
  examId: number;
  examName: string;
  examAmount: number;
  feeCollections: ExamFeeCollectionReadDto[];
}
