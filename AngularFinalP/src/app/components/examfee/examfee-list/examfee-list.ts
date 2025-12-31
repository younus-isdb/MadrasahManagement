import { Component, signal, OnInit } from '@angular/core';
import { ExamFeeReadDto } from '../../../models/examfee';
import { ExamfeeService } from '../../../service/examfeeService';

@Component({
  selector: 'app-examfee-list',
  standalone: false,
  templateUrl: './examfee-list.html',
  styleUrl: './examfee-list.css',
})

export class ExamfeeList implements OnInit {

  examfee = signal<ExamFeeReadDto[]>([]);
  loading = signal(false);
  errorMessage = signal('');

  constructor(private examfeeService: ExamfeeService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.loading.set(true);
    this.examfeeService.getAll().subscribe({
      next: (data) => {
        this.examfee.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to load exam fees.');
        this.loading.set(false);
        console.error(err);
      }
    });
  }
  trackById(index: number, item: ExamFeeReadDto) {
    return item.examFeeId;
  }


  deleteExamFee(id: number): void {
    if (confirm('Are you sure you want to delete this exam fee?')) {
      this.examfeeService.delete(id).subscribe({
        next: () => {
          this.examfee.update(list =>
            list.filter(e => e.examFeeId !== id)
          );
        },
        error: err => {
          alert('Could not delete the exam fee.');
          console.error(err);
        }
      });
    }
  }
}
