import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ExamFeeCreateDto } from '../../../models/examfee';
import { ExamfeeService } from '../../../service/examfeeService';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';

@Component({
  selector: 'app-examfee-create',
  standalone: false,
  templateUrl: './examfee-create.html',
  styleUrls: ['./examfee-create.css']
})
export class ExamfeeCreate implements OnInit {
  educationYear = signal<string>('');
  classId = signal<number | null>(null);
  examId = signal<number | null>(null);
  examAmount = signal<number>(0);
  loading = signal<boolean>(false);

  classes = signal<{ id: number; name: string }[]>([]);
  exams = signal<{ id: number; name: string }[]>([]);

  constructor(
    private examFeeService: ExamfeeService,
    private classService: ClassService,
    private examService: ExaminationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadClasses();
    this.loadExams();
  }

  loadClasses() {
    this.classService.getAll().subscribe({
      next: res => {
        // Map ClassReadDto[] → { id, name }[]
        const mapped = res.map(c => ({ id: c.classId, name: c.className }));
        this.classes.set(mapped);
      },
      error: err => console.error('Error loading classes', err)
    });
  }

  loadExams() {
    this.examService.getAll().subscribe({
      next: res => {
        // Map ExaminationReadDto[] → { id, name }[]
        const mapped = res.map(e => ({ id: e.examId, name: e.examName }));
        this.exams.set(mapped);
      },
      error: err => console.error('Error loading exams', err)
    });
  }

  save() {
    if (!this.educationYear().trim() || !this.classId() || !this.examId()) return;

    const payload: ExamFeeCreateDto = {
      educationYear: this.educationYear(),
      classId: this.classId()!,
      examId: this.examId()!,
      examAmount: this.examAmount()
    };

    this.loading.set(true);
    this.examFeeService.create(payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/examfee']);
      },
      error: () => this.loading.set(false)
    });
  }
}
