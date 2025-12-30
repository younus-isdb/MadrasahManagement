import { Component, OnInit, signal } from '@angular/core';
import { ExaminationService } from '../../service/examinationService';
import { ExaminationReadDto } from '../../models/examination';

@Component({
  selector: 'app-examination-list',
  standalone: false,
  templateUrl: './examination-list.html',
  styleUrls: ['./examination-list.css']
})
export class ExaminationList implements OnInit {

  // Signals for state management
  examinations = signal<ExaminationReadDto[]>([]);
  loading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private examService: ExaminationService) { }

  ngOnInit(): void {
    this.loadData();
  }

  // API theke data fetch kora
  loadData(): void {
    this.loading.set(true);
    this.examService.getAll().subscribe({
      next: (data) => {
        this.examinations.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to load examinations. Please try again later.');
        this.loading.set(false);
        console.error(err);
      }
    });
  }

  // Delete korar logic
  deleteExam(id: number): void {
    if (confirm('Are you sure you want to delete this examination?')) {
      this.examService.delete(id).subscribe({
        next: () => {
          // List theke delete kora item-ti remove kora (UI refresh)
          this.examinations.update(exams => exams.filter(e => e.examId !== id));
        },
        error: (err) => {
          alert('Could not delete the item.');
          console.error(err);
        }
      });
    }
  }
}
