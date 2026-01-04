import { Component, signal, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ExamroutineService } from '../../../service/examroutine-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

@Component({
  selector: 'app-examroutine-edit',
  standalone: false,
  templateUrl: './examroutine-edit.html'
})
export class ExamroutineEdit implements OnInit {
  public form!: FormGroup;
  public loading = signal(false);
  public examRoutineId!: number;

  public classes = signal<any[]>([]);
  public exams = signal<any[]>([]);
  public subjects = signal<any[]>([]);

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
      educationYear: ['', [Validators.required]],
      classId: [null, Validators.required],
      examId: [null, Validators.required],
      subjects: this.fb.array([])
    });
  }

  get subjectsArray() {
    return this.form.get('subjects') as FormArray;
  }

  addSubject(data?: any): void {
    const fg = this.fb.group({
      // এটি ডাটাবেসের সঠিক রো আইডেন্টিফাই করবে
      examRoutineId: [data?.examRoutineId || 0],
      subjectId: [data?.subjectId || null, Validators.required],
      roomNumber: [data?.roomNumber || 0, [Validators.required, Validators.min(1)]],
      examDate: [data?.examDate ? data.examDate.split('T')[0] : '', Validators.required],
      examDay: [data?.examDay || '', Validators.required],
      examStartTime: [data?.examStartTime || '', Validators.required],
      examEndTime: [data?.examEndTime || '', Validators.required]
    });
    this.subjectsArray.push(fg);
  }

  removeSubject(index: number): void {
    this.subjectsArray.removeAt(index);
  }

  private loadDropdowns(): void {
    this.classService.getAll().subscribe(res => this.classes.set(res));
    this.examService.getAll().subscribe(res => this.exams.set(res));
    this.subjectService.getAll().subscribe(res => this.subjects.set(res));
  }

  private loadExistingData(): void {
    this.loading.set(true);
    this.service.getById(this.examRoutineId).subscribe({
      next: (data) => {
        this.form.patchValue({
          educationYear: data.educationYear,
          classId: data.classId,
          examId: data.examId
        });

        this.subjectsArray.clear();
        if (data.subjects && data.subjects.length > 0) {
          data.subjects.forEach((s: any) => this.addSubject(s));
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        alert('Data load failed!');
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading.set(true);
    const f = this.form.value;

    const requests = f.subjects.map((s: any) => this.service.update({
      examRoutineId: Number(s.examRoutineId),
      educationYear: f.educationYear,
      classId: Number(f.classId),
      examId: Number(f.examId),
      subjectId: Number(s.subjectId),
      roomNumber: Number(s.roomNumber),
      examDate: s.examDate,
      examDay: s.examDay,
      examStartTime: s.examStartTime,
      examEndTime: s.examEndTime
    }));

    forkJoin(requests).subscribe({
      next: () => {
        this.loading.set(false);
        alert('Routine updated successfully!');
        this.router.navigate(['/examroutine']);
      },
      error: () => {
        this.loading.set(false);
        alert('Update failed!');
      }
    });
  }
}
