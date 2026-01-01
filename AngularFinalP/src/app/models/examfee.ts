export interface ExamFeeReadDto {
  examFeeId: number;
  educationYear: string;
  classId: number;
  className: string;
  examId: number;
  examName: string;
  examAmount: number;
}

export interface ExamFeeCreateDto {
  educationYear: string;
  classId: number;
  examId: number;
  examAmount: number;
}

export interface ExamFeeUpdateDto {
  examFeeId: number;
  educationYear: string;
  classId: number;
  examId: number;
  examAmount: number;
}
