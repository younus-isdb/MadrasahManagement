export interface ExamRoutineCreateDto {
  educationYear: string;
  classId: number;
  examId: number;
  subjectId: number;
  roomNumber: number;
  examDate: string;        // ISO date string (yyyy-MM-dd)
  examDay: string;
  examStartTime: string;   // e.g. "10:00 AM"
  examEndTime: string;     // e.g. "1:00 PM"
}

// Update DTO
export interface ExamRoutineUpdateDto extends ExamRoutineCreateDto {
  examRoutineId: number;
}

export interface ExamRoutineMasterReadDto {
  examRoutineId: number;
  educationYear: string;

  classId: number;
  className: string;

  examId: number;
  examName: string;

  subjects: ExamRoutineSubjectDetailReadDto[];
}
export interface ExamRoutineSubjectDetailReadDto {
  examRoutineId: number;
  subjectId: number;
  subjectName: string;

  roomNumber: number;

  examDate: string;
  examDay: string;
  examStartTime: string;
  examEndTime: string;
}
