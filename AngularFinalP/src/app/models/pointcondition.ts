// PointCondition Read DTO (for displaying/fetching)
export interface PointConditionReadDto {
  pointConditionId: number;
  educationYear: string;
  classId: number;
  className: string;      // For UI display
  examId: number;
  examName: string;       // For UI display
  subjectId: number;
  subjectName: string;    // For UI display
  passMarks: number;
  highestMark: number;
  details: PointConditionDetailReadDto[];
}

// PointCondition Detail Read DTO
export interface PointConditionDetailReadDto {
  pointConditionDetailId: number;
  fromMark: number;
  toMark: number;
  division: string;
  isSilverColor: boolean;
}

// PointCondition Create DTO (for creating new records)
export interface PointConditionCreateDto {
  educationYear: string;
  classId: number;
  examId: number;
  subjectId: number;
  passMarks: number;
  highestMark: number;
  details: PointConditionDetailCreateDto[];
}

// PointCondition Detail Create DTO
export interface PointConditionDetailCreateDto {
  fromMark: number;
  toMark: number;
  division: string;
  isSilverColor?: boolean; // optional for defaults
}

// PointCondition Update DTO (for updating existing records)
export interface PointConditionUpdateDto {
  pointConditionId: number;
  educationYear: string;
  classId: number;
  examId: number;
  subjectId: number;
  passMarks: number;
  highestMark: number;
  details: PointConditionDetailCreateDto[];
}
