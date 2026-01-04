import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { StudentService } from '../../../service/student-service';
import { DepartmentService } from '../../../service/department-service';
import { Router } from '@angular/router';

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
  filteredStudents = signal<any[]>([]); // students filtered by department/class

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
      departmentId: [null, [Validators.required]],
      classId: [null, [Validators.required]],
      examId: [null, [Validators.required]],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    this.loadDropdowns();

    // When department/class changes, filter students
    this.examFeeForm.get('departmentId')?.valueChanges.subscribe(() => this.filterStudents());
    this.examFeeForm.get('classId')?.valueChanges.subscribe(() => this.filterStudents());
  }

  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  loadDropdowns() {
    this.departmentService.getAll().subscribe(res => this.departments.set(res));
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  filterStudents() {
    const deptId = this.examFeeForm.value.departmentId;
    const classId = this.examFeeForm.value.classId;

    if (!deptId || !classId) {
      this.filteredStudents.set([]);
      this.feeCollections.clear();
      return;
    }

    const filtered = this.students().filter(s =>
      s.departmentId === deptId && s.classId === classId
    );

    this.filteredStudents.set(filtered);

    // Update FormArray
    this.feeCollections.clear();
    filtered.forEach(s => this.addStudentFee(s.studentId, 0, s.totalSubjects ?? 1));
  }

  addStudentFee(studentId?: any, examFeeAmount?: number, totalSubject?: number) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId ?? null, [Validators.required]],
      examFeeAmount: [examFeeAmount ?? 0, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject ?? 1, [Validators.required, Validators.min(1)]]
    }));
  }


  removeStudentFee(index: number) {
    if (this.feeCollections.length > 1) {
      this.feeCollections.removeAt(index);
    }
  }

  onSubmit() {
    if (this.examFeeForm.invalid) {
      this.examFeeForm.markAllAsTouched();
      return;
    }

    const formValue = this.examFeeForm.value;

    const dto: ExamFeesCreateDto = {
      educationYear: formValue.educationYear,
      departmentId: Number(formValue.departmentId),
      classId: Number(formValue.classId),
      examId: Number(formValue.examId),
      examAmount: Number(formValue.examAmount),
      feeCollections: this.feeCollections.value.map((fc: any) => ({
        studentId: Number(fc.studentId),
        examFeeAmount: Number(fc.examFeeAmount),
        totalSubject: Number(fc.totalSubject)
      }))
    };

    this.loading.set(true);
    this.service.create(dto).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Created Successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: (err) => {
        this.loading.set(false);
        const validationErrors = err.error?.errors ? Object.values(err.error.errors).flat().join('\n') : err.message;
        alert('Submission Failed:\n' + validationErrors);
      }
    });
  }
}
