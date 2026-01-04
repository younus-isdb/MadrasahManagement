import { Component, OnInit ,signal} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ExamincomeService } from '../../../service/examincome-service';
import { ExamIncomeUpdateDto } from '../../../models/ExamIncome';
import { ExaminationService } from '../../../service/examinationService';

@Component({
  selector: 'app-exam-income-edit',
  standalone: false,
  templateUrl: './exam-income-edit.html',
  styleUrl: './exam-income-edit.css',
})
export class ExamIncomeEdit implements OnInit {
  form!: FormGroup;
  exams = signal<any[]>([]);
  loading = signal(false);
  id!: number;

  constructor(
    private fb: FormBuilder,
    private service: ExamincomeService,
    private examService: ExaminationService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.id = Number(this.route.snapshot.paramMap.get('id'));

    this.form = this.fb.group({
      incomeExpenseId: [this.id],
      examId: [null, Validators.required],
      typesOfExpense: [''],
      amount: [null, [Validators.required, Validators.min(0)]]
    });

    this.loadExams();
  }

  loadExams() {
    this.examService.getAll().subscribe(res => {
      this.exams.set(res);

      // load record after exams loaded
      this.service.getById(this.id).subscribe(record => {
        this.form.patchValue({
          incomeExpenseId: record.incomeExpenseId,
          examId: record.examId,
          typesOfExpense: record.typesOfExpense,
          amount: record.amount
        });
      });
    });
  }

  update() {
    if (this.form.invalid) return;

    const dto: ExamIncomeUpdateDto = {
      incomeExpenseId: this.form.value.incomeExpenseId,
      examId: Number(this.form.value.examId),
      typesOfExpense: this.form.value.typesOfExpense ?? '',
      amount: Number(this.form.value.amount)
    };

    this.loading.set(true);
    this.service.update(dto.incomeExpenseId, dto).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/exam-income']);
      },
      error: (err) => {
        this.loading.set(false);
        alert('Update Failed: ' + (err.error?.message ?? err.message));
      }
    });
  }
}
