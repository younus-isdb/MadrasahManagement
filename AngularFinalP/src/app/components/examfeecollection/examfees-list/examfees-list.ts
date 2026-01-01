import { Component, signal, effect, OnInit } from '@angular/core';
import { ExamFeesReadDto, ExamFeeCollectionReadDto } from '../../../models/examfeeCollection';
import { ExamfeecollectionService } from '../../../service/examfeecollection-service';

@Component({
  selector: 'app-examfees-list',
  standalone: false,
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

    // Optional: Watch for changes
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

  // --- Helper methods for totals & dues ---
  getCollected(fee: ExamFeesReadDto): number {
    return fee.feeCollections?.reduce((sum, fc) => sum + fc.examFeeAmount, 0) || 0;
  }

  getDue(fee: ExamFeesReadDto, fc: ExamFeeCollectionReadDto): number {
    return fee.examAmount - fc.examFeeAmount;
  }

  getTotalExamFee(): number {
    return this.examFees()?.reduce((sum, f) => sum + f.examAmount, 0) || 0;
  }

  getTotalCollected(): number {
    return this.examFees()?.reduce((sum, f) => sum + this.getCollected(f), 0) || 0;
  }

  getTotalDue(): number {
    return this.getTotalExamFee() - this.getTotalCollected();
  }

  // --- Optional: Delete Exam Fee ---
  delete(examFeeId: number) {
    if (!confirm('Are you sure you want to delete this exam fee?')) return;

    this.examFeeService.delete(examFeeId).subscribe({
      next: () => {
        // Remove from signal
        this.examFees.update(fees => fees.filter(f => f.examFeeId !== examFeeId));
      },
      error: (err) => {
        console.error(err);
        this.error.set('Failed to delete exam fee');
      }
    });
  }
}
