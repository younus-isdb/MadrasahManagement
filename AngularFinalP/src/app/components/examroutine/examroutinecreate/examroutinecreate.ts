import { Component, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ExamroutineService } from '../../../service/examroutine-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

interface DropdownItem {
  id: number;
  name: string;
}

@Component({
  selector: 'app-examroutinecreate',
  standalone: false,
  templateUrl: './examroutinecreate.html',
  styleUrls: ['./examroutinecreate.css']
})
export class Examroutinecreate implements OnInit {
  form!: FormGroup;

  // Signals for dropdowns
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
    // Form setup
    this.form = this.fb.group({
      educationYear: ['', [Validators.required, Validators.maxLength(10)]],
      classId: [null, Validators.required],
      examId: [null, Validators.required],
      subjectId: [null, Validators.required],
      roomNumber: [0, [Validators.required, Validators.min(1)]],
      examDate: ['', Validators.required],
      examDay: ['', Validators.required],
      examStartTime: ['', Validators.required],
      examEndTime: ['', Validators.required]
    });

    this.loadDropdowns();
  }

  loadDropdowns(): void {
    this.classService.getAll().subscribe(res =>
      this.classes.set(res.map(c => ({ id: c.classId, name: c.className })))
    );

    this.examService.getAll().subscribe(res =>
      this.exams.set(res.map(e => ({ id: e.examId, name: e.examName })))
    );

    this.subjectService.getAll().subscribe(res =>
      this.subjects.set(res.map(s => ({ id: s.subjectId, name: s.subjectName })))
    );
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const formValue = this.form.value;

    // Directly pass formValue but ensure numbers are converted
    this.loading.set(true);
    this.service.create({
      ...formValue,
      classId: Number(formValue.classId),
      examId: Number(formValue.examId),
      subjectId: Number(formValue.subjectId),
      roomNumber: Number(formValue.roomNumber)
    }).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Exam routine created successfully!');
        this.router.navigate(['/examroutine']);
      },
      error: (err) => {
        this.loading.set(false);
        console.error(err);
        alert('Failed to create exam routine: ' + (err.error?.message ?? err.message));
      }
    });
  }
}
