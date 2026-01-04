import { Component, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs'; // Important for multiple requests
import { ExamroutineService } from '../../../service/examroutine-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

@Component({
  selector: 'app-examroutinecreate',
  standalone: false,
  templateUrl: './examroutinecreate.html',
  styleUrls: ['./examroutinecreate.css']
})
export class Examroutinecreate implements OnInit {
  form!: FormGroup;
  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  subjects = signal<any[]>([]);
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private service: ExamroutineService,
    private classService: ClassService,
    private examService: ExaminationService,
    private subjectService: SubjectService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      educationYear: ['', [Validators.required]],
      classId: [null, Validators.required],
      examId: [null, Validators.required],
      subjects: this.fb.array([])
    });
    this.loadDropdowns();
    this.addSubject();
  }

  get subjectsArray() { return this.form.get('subjects') as FormArray; }

  addSubject(): void {
    this.subjectsArray.push(this.fb.group({
      subjectId: [null, Validators.required],
      roomNumber: [null, Validators.required],
      examDate: ['', Validators.required],
      examDay: ['', Validators.required],
      examStartTime: ['', Validators.required],
      examEndTime: ['', Validators.required]
    }));
  }

  removeSubject(index: number) { this.subjectsArray.removeAt(index); }

  loadDropdowns(): void {
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.subjectService.getAll().subscribe(res => this.subjects.set(res));
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const formValue = this.form.value;

    // Create a list of API requests
    const requests = formValue.subjects.map((s: any) => this.service.create({
      educationYear: formValue.educationYear,
      classId: Number(formValue.classId),
      examId: Number(formValue.examId),
      subjectId: Number(s.subjectId),
      roomNumber: Number(s.roomNumber),
      examDate: s.examDate,
      examDay: s.examDay,
      examStartTime: s.examStartTime,
      examEndTime: s.examEndTime
    }));

    // Execute all requests together
    forkJoin(requests).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Routine Created Successfully!');
        this.router.navigate(['/examroutine']);
      },
      error: (err) => {
        this.loading.set(false);
        alert('Failed to save some subjects.');
      }
    });
  }
}
