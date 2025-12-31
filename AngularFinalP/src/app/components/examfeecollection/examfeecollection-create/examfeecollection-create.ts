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
  standalone:false,
  templateUrl: './examfeecollection-create.html',
  styleUrls: ['./examfeecollection-create.css']
})
export class ExamFeeCollectionCreate implements OnInit {

  examFeeForm!: FormGroup;

  // ---------------- Signals ----------------
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
      educationYear: ['', Validators.required],
      classId: [0, Validators.required],
      examId: [0, Validators.required],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    this.loadDropdowns();
    this.addStudentFee();
  }

  // ---------------- FormArray Getter ----------------
  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  // ---------------- Add / Remove Student Fee ----------------
  addStudentFee(studentId = 0, examFeeAmount = 0, totalSubject = 0) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, Validators.required],
      examFeeAmount: [examFeeAmount, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject, [Validators.required, Validators.min(1)]]
    }));
  }

  removeStudentFee(index: number) {
    this.feeCollections.removeAt(index);
  }

  // ---------------- Load Dropdown Data ----------------
  loadDropdowns() {
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  // ---------------- Submit Form ----------------
  onSubmit() {
    if (this.examFeeForm.invalid) return;

    const dto: ExamFeesCreateDto = {
      ...this.examFeeForm.value,
      feeCollections: this.feeCollections.value
    };

    this.loading.set(true);

    this.service.create(dto).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Exam Fee with student collections created successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: (err) => {
        this.loading.set(false);
        console.error(err);
        alert('Error creating Exam Fee');
      }
    });
  }
}
