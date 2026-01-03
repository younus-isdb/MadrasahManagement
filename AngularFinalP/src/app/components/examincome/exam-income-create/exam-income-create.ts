import { Component, OnInit,signal } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ExamincomeService } from '../../../service/examincome-service';
import { ExamIncomeCreateDto } from '../../../models/ExamIncome';
import { ExaminationService } from '../../../service/examinationService';

@Component({
  selector: 'app-exam-income-create',
  standalone: false,
  templateUrl: './exam-income-create.html',
  styleUrls: ['./exam-income-create.css'] // note: corrected property 'styleUrls'
})
export class ExamIncomeCreate implements OnInit {
  form!: FormGroup;
  exams = signal<any[]>([]);
  loading = signal(false);

  constructor(
    private fb: FormBuilder,
    private service: ExamincomeService,
    private examService: ExaminationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      examId: [null, Validators.required],
      typesOfExpense: [''],
      amount: [null, [Validators.required, Validators.min(0)]]
    });

    this.loadExams();
  }

  loadExams() {
    this.examService.getAll().subscribe(res => this.exams.set(res));
  }

  submit() {
    if (this.form.invalid) return;

    const dto: ExamIncomeCreateDto = {
      examId: Number(this.form.value.examId),
      typesOfExpense: this.form.value.typesOfExpense ?? '',
      amount: Number(this.form.value.amount)
    };

    this.loading.set(true);
    this.service.create(dto).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/exam-income']);
      },
      error: (err) => {
        this.loading.set(false);
        alert('Error: ' + (err.error?.message ?? err.message));
      }
    });
  }
}
