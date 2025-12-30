export interface ExaminationReadDto {
  examId: number;
  examName: string;
  examFeeCount: number;
  pointConditionCount: number;
  examRoutineCount: number;
}

export interface ExaminationCreateDto {
  examName: string;
}

export interface ExaminationUpdateDto {
  examId: number;
  examName: string;
}
