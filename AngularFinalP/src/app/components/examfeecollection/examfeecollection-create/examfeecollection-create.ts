import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { Router } from '@angular/router';
import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { DepartmentService } from '../../../service/department-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { StudentService } from '../../../service/student-service';

@Component({
  selector: 'app-examfeecollection-create',
  standalone: false,
  templateUrl: './examfeecollection-create.html',
  styleUrls: ['./examfeecollection-create.css']
})
export class ExamFeeCollectionCreate implements OnInit {

  examFeeForm!: FormGroup;
  loading = signal(false);

  departments = signal<any[]>([]);
  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  students = signal<any[]>([]);
  filteredStudents = signal<any[]>([]);

  constructor(
    private fb: FormBuilder,
    private service: ExamfeecollectionService,
    private departmentService: DepartmentService,
    private classService: ClassService,
    private examService: ExaminationService,
    private studentService: StudentService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.examFeeForm = this.fb.group({
      educationYear: ['', [Validators.required, Validators.maxLength(10)]],
      departmentId: [null, Validators.required],
      classId: [null, Validators.required],
      examId: [null, Validators.required],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    this.loadDropdowns();

    this.examFeeForm.get('departmentId')?.valueChanges.subscribe(() => this.filterStudents());
    this.examFeeForm.get('classId')?.valueChanges.subscribe(() => this.filterStudents());
  }

  // ---------- FormArray getter ----------
  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  addStudentFee(studentId?: number, amount = 0, total = 1) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, Validators.required],
      examFeeAmount: [amount, [Validators.required, Validators.min(0)]],
      totalSubject: [total, [Validators.required, Validators.min(1)]]
    }));
  }

  removeStudentFee(index: number) {
    if (this.feeCollections.length > 1) this.feeCollections.removeAt(index);
  }

  // ---------- Load dropdowns ----------
  loadDropdowns() {
    this.departmentService.getAll().subscribe(res => this.departments.set(res));
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  // ---------- Filter students ----------
  filterStudents() {
    const deptId = Number(this.examFeeForm.value.departmentId);
    const classId = Number(this.examFeeForm.value.classId);

    if (!deptId || !classId) {
      this.filteredStudents.set([]);
      this.feeCollections.clear();
      this.addStudentFee(); // keep at least 1 row
      return;
    }

    const filtered = this.students().filter(s =>
      Number(s.departmentId) === deptId &&
      Number(s.classId) === classId
    );

    this.filteredStudents.set(filtered);
    this.feeCollections.clear();

    if (filtered.length === 0) {
      this.addStudentFee();
      return;
    }

    filtered.forEach(s => this.addStudentFee(s.studentId, 0, s.totalSubjects ?? 1));
  }

  // ---------- Submit ----------
  onSubmit() {
    if (this.examFeeForm.invalid) {
      this.examFeeForm.markAllAsTouched();
      return;
    }

    const v = this.examFeeForm.value;
    const dto: ExamFeesCreateDto = {
      educationYear: v.educationYear,
      departmentId: +v.departmentId,
      classId: +v.classId,
      examId: +v.examId,
      examAmount: +v.examAmount,
      feeCollections: this.feeCollections.value.map((f: any) => ({
        studentId: +f.studentId,
        examFeeAmount: +f.examFeeAmount,
        totalSubject: +f.totalSubject
      }))
    };

    this.loading.set(true);
    this.service.create(dto).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Created Successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: err => {
        this.loading.set(false);
        alert(err.error?.message || 'Submission Failed');
      }
    });
  }
}
