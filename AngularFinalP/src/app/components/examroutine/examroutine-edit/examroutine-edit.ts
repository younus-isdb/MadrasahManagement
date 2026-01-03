import { Component, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ExamroutineService } from '../../../service/examroutine-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

interface DropdownItem {
  id: number;
  name: string;
}

@Component({
  selector: 'app-examroutine-edit',
  standalone: false,
  templateUrl: './examroutine-edit.html',
  styleUrls: ['./examroutine-edit.css']
})
export class ExamroutineEdit implements OnInit {
  form!: FormGroup;
  loading = signal(false);

  classes = signal<any[]>([]);
  exams = signal<any[]>([]);
  subjects = signal<any[]>([]);

  private examRoutineId!: number;

  constructor(
    private fb: FormBuilder,
    private service: ExamroutineService,
    private classService: ClassService,
    private examService: ExaminationService,
    private subjectService: SubjectService,
    private router: Router,
    private route: ActivatedRoute
  ) { }

  ngOnInit(): void {
    this.examRoutineId = Number(this.route.snapshot.paramMap.get('id'));
    this.initForm();
    this.loadDropdowns();
    this.loadExistingData();
  }

  private initForm(): void {
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
  }

  private loadDropdowns(): void {
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

  private loadExistingData(): void {
    this.service.getById(this.examRoutineId).subscribe({
      next: (data) => {
        this.form.patchValue({
          educationYear: data.educationYear,
          classId: data.classId,
          examId: data.examId,
          subjectId: data.subjectId,
          roomNumber: data.roomNumber,
          examDate: data.examDate,
          examDay: data.examDay,
          examStartTime: data.examStartTime,
          examEndTime: data.examEndTime
        });
      },
      error: (err) => {
        console.error(err);
        alert('Failed to load exam routine data.');
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const f = this.form.value;
    this.loading.set(true);

    this.service.update(this.examRoutineId, {
      ...f,
      classId: Number(f.classId),
      examId: Number(f.examId),
      subjectId: Number(f.subjectId),
      roomNumber: Number(f.roomNumber)
    }).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Exam routine updated successfully!');
        this.router.navigate(['/examroutine']);
      },
      error: (err) => {
        this.loading.set(false);
        console.error(err);
        alert('Failed to update exam routine: ' + (err.error?.message ?? err.message));
      }
    });
  }
}
