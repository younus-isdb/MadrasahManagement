import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { ExamFeesCreateDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-examfeecollection-create',
  standalone:false,
  templateUrl: './examfeecollection-create.html',
  styleUrls: ['./examfeecollection-create.css']
})
export class ExamFeeCollectionCreate implements OnInit {
  examFeeForm!: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private service: ExamfeecollectionService,
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

    // Optionally, start with one student fee row
    this.addStudentFee();
  }

  get feeCollections(): FormArray {
    return this.examFeeForm.get('feeCollections') as FormArray;
  }

  addStudentFee(studentId = 0, examFee = 0, totalSubject = '', educationYear = '') {
    this.feeCollections.push(this.fb.group({
      studentId: [studentId, Validators.required],
      examFee: [examFee, [Validators.required, Validators.min(0)]],
      totalSubject: [totalSubject, Validators.required],
      educationYear: [educationYear, Validators.required]
    }));
  }

  removeStudentFee(index: number) {
    this.feeCollections.removeAt(index);
  }

  onSubmit() {
    if (this.examFeeForm.invalid) return;

    const dto: ExamFeesCreateDto = this.examFeeForm.value;
    this.loading = true;

    this.service.create(dto).subscribe({
      next: () => {
        this.loading = false;
        alert('Exam Fee with student collections created successfully!');
        this.router.navigate(['/examfeecollection']);
      },
      error: (err) => {
        this.loading = false;
        console.error(err);
        alert('Error creating Exam Fee');
      }
    });
  }
}
