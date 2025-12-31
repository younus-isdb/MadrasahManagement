import { Component, signal, effect, OnInit } from '@angular/core';
import { ExamFeesReadDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';


@Component({
  selector: 'app-examfees-list',
  standalone:false,
  templateUrl: './examfees-list.html',
  styleUrls: ['./examfees-list.css']
})
export class ExamFeesList implements OnInit {

  // Signals
  examFees = signal<ExamFeesReadDto[]>([]);
  loading = signal(false);
  error = signal('');

  constructor(private examFeeService: ExamfeecollectionService) { }

  ngOnInit(): void {
    this.loadExamFees();

    // Optional: Watch for changes (example)
    effect(() => {
      console.log('Exam Fees updated:', this.examFees());
    });
  }

  loadExamFees() {
    this.loading.set(true);
    this.examFeeService.getAll().subscribe({
      next: (res) => {
        this.examFees.set(res);
        this.loading.set(false);
      },
      error: (err) => {
        console.error(err);
        this.error.set('Failed to load exam fees');
        this.loading.set(false);
      }
    });
  }
}
