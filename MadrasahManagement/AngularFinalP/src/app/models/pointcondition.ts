export interface PointConditionDetailDto {
  pointConditionDetailId?: number;
  fromMark: number;
  toMark: number;
  division: string;
  isSilverColor: boolean;
}

export interface PointConditionDto {
  pointConditionId?: number;
  educationYear: string;
  classId: number;
  examId: number;
  subjectId: number;
  passMarks: number;
  highestMark: number;
  details: PointConditionDetailDto[];
}

export interface PointConditionReadDto extends PointConditionDto {
  className?: string;
  examName?: string;
  subjectName?: string;
}
