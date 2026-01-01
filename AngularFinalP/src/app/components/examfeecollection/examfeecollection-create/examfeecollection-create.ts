import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { StudentService } from '../../../service/student-service';
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
  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  students = signal<any[]>([]);

  constructor(
    private fb: FormBuilder,
    private service: ExamfeecollectionService,
    private classService: ClassService,
    private examService: ExaminationService,
    private studentService: StudentService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.examFeeForm = this.fb.group({
      educationYear: ['', [Validators.required, Validators.maxLength(10)]],
      classId: [null, [Validators.required]],
      examId: [null, [Validators.required]],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    this.loadDropdowns();
    this.addStudentFee();
  }

  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  addStudentFee(studentId: any = null, examFeeAmount = 0, totalSubject = 0) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, [Validators.required]],
      examFeeAmount: [examFeeAmount, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject, [Validators.required, Validators.min(1)]]
    }));
  }

  removeStudentFee(index: number) {
    if (this.feeCollections.length > 1) {
      this.feeCollections.removeAt(index);
    }
  }

  loadDropdowns() {
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  onSubmit() {
    if (this.examFeeForm.invalid) {
      this.examFeeForm.markAllAsTouched();
      return;
    }

    const formValue = this.examFeeForm.value;

    // FORCED TYPE CASTING: Ensures int/decimal for .NET
    const dto: ExamFeesCreateDto = {
      educationYear: formValue.educationYear,
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
        console.error('Server Response:', err);
        // This alerts the exact C# validation error (e.g., "The ExamId field is required")
        const validationErrors = err.error?.errors ? JSON.stringify(err.error.errors) : err.message;
        alert('Submission Failed:\n' + validationErrors);
      }
    });
  }
}
