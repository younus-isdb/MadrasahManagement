import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { DepartmentService } from '../../../service/department-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { StudentService } from '../../../service/student-service';

@Component({
  selector: 'app-examfeecollection-edit',
  standalone: false,
  templateUrl: './examfeecollection-edit.html',
  styleUrls: ['./examfeecollection-edit.css']
})
export class ExamfeecollectionEdit implements OnInit {

  examFeeForm!: FormGroup;
  loading = signal(false);

  departments = signal<any[]>([]);
  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  students = signal<any[]>([]);

  collectionId!: number;

  constructor(
    private fb: FormBuilder,
    private service: ExamfeecollectionService,
    private departmentService: DepartmentService,
    private classService: ClassService,
    private examService: ExaminationService,
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.collectionId = Number(this.route.snapshot.paramMap.get('id'));

    this.examFeeForm = this.fb.group({
      educationYear: ['', [Validators.required, Validators.maxLength(10)]],
      departmentId: [null, Validators.required],
      classId: [null, Validators.required],
      examId: [null, Validators.required],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    this.loadDropdowns();
    this.loadExistingCollection();

    this.examFeeForm.get('departmentId')?.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.populateStudents());

    this.examFeeForm.get('classId')?.valueChanges
      .pipe(takeUntilDestroyed())
      .subscribe(() => this.populateStudents());
  }

  // ---------- FormArray ----------
  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  addStudentFee(studentId: number | null = null, examFeeAmount = 0, totalSubject = 1) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, Validators.required],
      examFeeAmount: [examFeeAmount, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject, [Validators.required, Validators.min(1)]]
    }));
  }

  removeStudentFee(index: number) {
    if (this.feeCollections.length > 1) {
      this.feeCollections.removeAt(index);
    }
  }

  // ---------- Load dropdowns ----------
  loadDropdowns() {
    this.departmentService.getAll().subscribe(res => this.departments.set(res));
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  // ---------- Populate students ----------
  populateStudents() {
    const deptId = Number(this.examFeeForm.value.departmentId);
    const classId = Number(this.examFeeForm.value.classId);

    if (!deptId || !classId) return;

    if (this.feeCollections.length > 0) return; // do not overwrite edit data

    const filtered = this.students().filter(s =>
      Number(s.departmentId) === deptId &&
      Number(s.classId) === classId
    );

    if (filtered.length === 0) {
      this.addStudentFee();
      return;
    }

    filtered.forEach(s =>
      this.addStudentFee(s.studentId, 0, s.totalSubjects ?? 1)
    );
  }

  // ---------- Load existing data ----------
  loadExistingCollection() {
    this.loading.set(true);
    this.service.getById(this.collectionId).subscribe({
      next: (data: any) => {
        this.loading.set(false);

        this.examFeeForm.patchValue({
          educationYear: data.educationYear,
          departmentId: data.departmentId,
          classId: data.classId,
          examId: data.examId,
          examAmount: data.examAmount
        });

        this.feeCollections.clear();
        data.feeCollections.forEach((fc: any) => {
          this.addStudentFee(fc.studentId, fc.examFeeAmount, fc.totalSubject);
        });
      },
      error: () => {
        this.loading.set(false);
        alert('Failed to load existing exam fee collection.');
      }
    });
  }

  // ---------- Submit ----------
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
    this.service.update(this.collectionId, dto).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Updated Successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: (err) => {
        this.loading.set(false);
        const errors = err?.error?.errors
          ? Object.values(err.error.errors).flat().join('\n')
          : err.message;
        alert('Update Failed:\n' + errors);
      }
    });
  }
}
