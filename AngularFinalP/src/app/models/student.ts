export interface StudentCreateDto {
  userId: string;
  studentName: string;
  arabicStudentName?: string;
  banglaStudentName?: string;

  departmentId: number;
  classId: number;
  sectionId: number;

  regNo: string;
  nationalId?: string;
  admissionDate?: string; // DateOnly → string (yyyy-MM-dd)

  gender?: GenderType;
  dob?: string; // DateTime → string ISO format

  bloodGroup?: string;

  fatherName?: string;
  fatherPhone?: string;
  motherName?: string;
  motherPhone?: string;
  guardianName?: string;
  guardianPhone?: string;
  guardianEmail?: string;

  address?: string;
  city?: string;
  country?: string;

  emergencyContactName?: string;
  emergencyPhone?: string;
  medicalNotes?: string;

  previousSchoolName?: string;
  previousResult?: number;

  profileImageUrl?: string;
  documentUrl?: string;

  isActive?: boolean;
  leavingDate?: string; // DateTimeOffset → string ISO
  leavingReason?: string;
}
export interface StudentUpdateDto extends StudentCreateDto {
  studentId: number;
}
export interface StudentReadDto {
  studentId: number;
  studentName: string;
  arabicStudentName?: string;
  banglaStudentName?: string;

  departmentId: number;
  departmentName: string;

  classId: number;
  className: string;

  sectionId: number;
  sectionName: string;

  regNo: string;
  nationalId?: string;
  admissionDate?: string;

  gender?: GenderType;
  dob?: string;
  bloodGroup?: string;

  isActive: boolean;
  leavingDate?: string;
  leavingReason?: string;
}
export enum GenderType {
  Male = 1,
  Female = 2,
  Other = 3
}
