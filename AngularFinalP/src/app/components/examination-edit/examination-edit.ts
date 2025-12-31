import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExaminationService } from '../../service/examinationService';
import { ExaminationUpdateDto } from '../../models/examination';

@Component({
  selector: 'app-examination-edit',
  standalone: false,
  templateUrl: './examination-edit.html',
  styleUrls: ['./examination-edit.css']
 
})
export class ExaminationEdit implements OnInit {

  examName = signal<string>('');
  loading = signal<boolean>(false);
  currentId!: number;

  constructor(
    private route: ActivatedRoute,
    private examService: ExaminationService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.currentId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadData();
  }

  loadData() {
    this.loading.set(true);
    this.examService.getById(this.currentId).subscribe({
      next: res => {
        this.examName.set(res.examName);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  update() {
    if (!this.examName().trim()) return;

    const payload: ExaminationUpdateDto = {
      examId: this.currentId,
      examName: this.examName()
    };

    this.examService.update(this.currentId, payload).subscribe({
      next: () => this.router.navigate(['/examination']),
      error: () => this.loading.set(false)
    });
  }
}
