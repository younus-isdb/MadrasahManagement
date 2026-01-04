import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ExamroutineService } from '../../../service/examroutine-service';

@Component({
  selector: 'app-examroutine-list',
  standalone: false,
  templateUrl: './examroutine-list.html'
})
export class ExamroutineList implements OnInit {
  Routines = signal<any[]>([]);
  loading = signal(false);

  constructor(private service: ExamroutineService, private router: Router) { }

  ngOnInit(): void { this.loadData(); }

  loadData(): void {
    this.loading.set(true);
    this.service.getMasterDetails().subscribe({
      next: (data) => {
        // Grouping logic to merge same Exam/Class into one header
        const grouped = data.reduce((acc: any[], current: any) => {
          const key = `${current.examName}-${current.className}-${current.educationYear}`;
          const existing = acc.find(r => `${r.examName}-${r.className}-${r.educationYear}` === key);

          if (existing) {
            existing.subjects.push(...(current.subjects || []));
          } else {
            acc.push({ ...current, subjects: [...(current.subjects || [])] });
          }
          return acc;
        }, []);

        this.Routines.set(grouped);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  deleteExamRoutine(id: number) {
    if (confirm('Delete this routine?')) {
      this.service.delete(id).subscribe(() => this.loadData());
    }
  }

  editExam(id: number) { this.router.navigate(['/routineedit', id]); }
}
