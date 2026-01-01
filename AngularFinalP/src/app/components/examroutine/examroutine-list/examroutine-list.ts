import { Component ,signal,OnInit} from '@angular/core';
import { ExamRoutineReadDto } from '../../../models/ExamRoutine';
import { ExamroutineService } from '../../../service/examroutine-service';

@Component({
  selector: 'app-examroutine-list',
  standalone: false,
  templateUrl: './examroutine-list.html',
  styleUrl: './examroutine-list.css',
})
export class ExamroutineList implements OnInit {
  // Signals for state management
  Routines = signal<ExamRoutineReadDto[]>([]);
  loading = signal<boolean>(false);
  errorMessage = signal<string>('');

  constructor(private examRoutine: ExamroutineService) { }

  ngOnInit(): void {
    this.loadData();
  }

  // API theke data fetch kora
  loadData(): void {
    this.loading.set(true);
    this.examRoutine.getAll().subscribe({
      next: (data) => {
        this.Routines.set(data);
        this.loading.set(false);
      },
      error: (err) => {
        this.errorMessage.set('Failed to load examinations. Please try again later.');
        this.loading.set(false);
        console.error(err);
      }
    });
  }
  trackById(index: number, item: ExamRoutineReadDto) {
    return item.examRoutineId;
  }


  // Delete korar logic
  deleteExam(id: number): void {
    if (confirm('Are you sure you want to delete this examination?')) {
      this.examRoutine.delete(id).subscribe({
        next: () => {
          // List theke delete kora item-ti remove kora (UI refresh)
          this.Routines.update(exams => exams.filter(e => e.examId !== id));
        },
        error: (err) => {
          alert('Could not delete the item.');
          console.error(err);
        }
      });
    }
  }

}
