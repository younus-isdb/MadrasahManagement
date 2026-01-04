// =================== ExamFeeCollection DTOs ===================

// For creating a student fee entry
export interface ExamFeeCollectionCreateDto {
  studentId: number;
  examFeeAmount: number;
  totalSubject: number;
}

// For updating a student fee entry
export interface ExamFeeCollectionUpdateDto {
  feeCollectionId?: number;   // undefined or null = new
  studentId: number;
  examFeeAmount: number;
  totalSubject: number;
}

// For reading a student fee entry
export interface ExamFeeCollectionReadDto {
  feeCollectionId: number;
  studentId: number;
  studentName: string;
  examFeeAmount: number;
  totalSubject: number;
}
// =================== ExamFee DTOs ===================

// For creating a new exam fee with students
export interface ExamFeesCreateDto {
  educationYear: string;
  departmentId: number;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionCreateDto[];
}

// For updating an existing exam fee with students
export interface ExamFeesUpdateDto {
  educationYear: string;
  departmentId: number;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionUpdateDto[];
}

// For reading an exam fee from backend
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
// =================== ExamFeeCollection Update DTO ===================
export interface ExamFeeCollectionUpdateDto {
  feeCollectionId?: number;   // undefined or null = new collection
  studentId: number;
  examFeeAmount: number;
  totalSubject: number;
}

// =================== ExamFee Update DTO ===================
export interface ExamFeesUpdateDto {
  educationYear: string;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionUpdateDto[];
}
