import { Component, OnInit, signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormArray } from '@angular/forms';
import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { StudentService } from '../../../service/student-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-examfeecollection-edit',
  standalone: false,
  templateUrl: './examfeecollection-edit.html',
  styleUrls: ['./examfeecollection-edit.css']
})
export class ExamfeecollectionEdit implements OnInit {
  examFeeForm!: FormGroup;
  loading = signal(false);
  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  students = signal<any[]>([]);
  collectionId!: number;

  constructor(
    private fb: FormBuilder,
    private service: ExamfeecollectionService,
    private classService: ClassService,
    private examService: ExaminationService,
    private studentService: StudentService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    // Get collection ID from route
    this.collectionId = Number(this.route.snapshot.paramMap.get('id'));

    // Initialize form
    this.examFeeForm = this.fb.group({
      educationYear: ['', [Validators.required, Validators.maxLength(10)]],
      classId: [null, [Validators.required]],
      examId: [null, [Validators.required]],
      examAmount: [0, [Validators.required, Validators.min(0)]],
      feeCollections: this.fb.array([])
    });

    // Load dropdowns
    this.loadDropdowns();

    // Load existing data
    this.loadExistingCollection();

    // Update students when class changes
    this.examFeeForm.get('classId')?.valueChanges.subscribe(classId => {
      this.populateStudentsByClass(classId);
    });
  }

  // FormArray getter
  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  // Add student fee row
  addStudentFee(studentId: any = null, examFeeAmount = 0, totalSubject = 0) {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, [Validators.required]],
      examFeeAmount: [examFeeAmount, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject, [Validators.required, Validators.min(1)]]
    }));
  }

  // Remove student fee row
  removeStudentFee(index: number) {
    if (this.feeCollections.length > 1) {
      this.feeCollections.removeAt(index);
    }
  }

  // Load dropdown data
  loadDropdowns() {
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.studentService.getAll().subscribe(res => this.students.set(res));
  }

  // Populate students for selected class
  populateStudentsByClass(classId: number) {
    const filteredStudents = this.students().filter(s => s.classId === classId);
    // Only replace feeCollections if currently empty
    if (this.feeCollections.length === 0) {
      filteredStudents.forEach(student => this.addStudentFee(student.id, 0, 0));
      if (filteredStudents.length === 0) this.addStudentFee(); // at least one row
    }
  }

  // Load existing collection from API
  loadExistingCollection() {
    this.loading.set(true);
    this.service.getById(this.collectionId).subscribe({
      next: (data: any) => {
        this.loading.set(false);
        this.examFeeForm.patchValue({
          educationYear: data.educationYear,
          classId: data.classId,
          examId: data.examId,
          examAmount: data.examAmount
        });

        this.feeCollections.clear();
        data.feeCollections.forEach((fc: any) => {
          this.addStudentFee(fc.studentId, fc.examFeeAmount, fc.totalSubject);
        });
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Error loading collection:', err);
        alert('Failed to load existing data.');
      }
    });
  }

  // Submit edit
  onSubmit() {
    if (this.examFeeForm.invalid) {
      this.examFeeForm.markAllAsTouched();
      return;
    }

    const formValue = this.examFeeForm.value;

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
    this.service.update(this.collectionId, dto).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Updated Successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Server Response:', err);
        if (err.error?.errors) {
          const errors = Object.values(err.error.errors).flat().join('\n');
          alert('Update Failed:\n' + errors);
        } else {
          alert('Update Failed:\n' + err.message);
        }
      }
    });
  }
}
