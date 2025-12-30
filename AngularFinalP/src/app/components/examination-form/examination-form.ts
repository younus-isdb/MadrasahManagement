import { Component,signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ExaminationService } from '../../service/examinationService';
import { ExaminationCreateDto, ExaminationUpdateDto } from '../../models/examination';

@Component({
  selector: 'app-examination-form',
  standalone: false,
  templateUrl: './examination-form.html',
  styleUrls:['./examination-form.css']
})
export class ExaminationForm {

  examName = signal<string>('');
  loading = signal<boolean>(false);

  constructor(
    private examService: ExaminationService,
    private router: Router
  ) { }

  save() {
    if (!this.examName().trim()) return;

    this.loading.set(true);

    const payload: ExaminationCreateDto = {
      examName: this.examName()
    };

    this.examService.create(payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/examination']);
      },
      error: () => this.loading.set(false)
    });
  }
}
