// =================== ExamFeeCollection DTO ===================
export interface ExamFeeCollectionCreateDto {
  studentId: number;
  examFee: number;
  totalSubject: string;
  educationYear: string;
}

export interface ExamFeeCollectionReadDto {
  feeCollectionId: number;
  studentId: number;
  studentName: string;
  examFee: number;
  totalSubject: string;
  educationYear: string;
}

// =================== ExamFee DTO ===================
export interface ExamFeesCreateDto {
  educationYear: string;
  classId: number;
  examId: number;
  examAmount: number;
  feeCollections: ExamFeeCollectionCreateDto[];
}

export interface ExamFeesReadDto {
  examFeeId: number;
  educationYear: string;
  classId: number;
  className: string;
  examId: number;
  examName: string;
  examAmount: number;
  feeCollections: ExamFeeCollectionReadDto[];
}
