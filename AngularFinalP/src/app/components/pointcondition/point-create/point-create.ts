import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PointConditionCreateDto, PointConditionDetailCreateDto } from '../../../models/pointcondition';
import { PointService } from '../../../service/point-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';

@Component({
  selector: 'app-point-create',
  standalone: false,
  templateUrl: './point-create.html',
  styleUrls: ['./point-create.css']
})
export class PointCreate implements OnInit {

  // Master form
  pointCondition = signal<PointConditionCreateDto>({
    educationYear: '',
    classId: null!,
    examId: null!,
    subjectId: null!,
    passMarks: 0,
    highestMark: 0,
    details: []
  });

  // Dropdown data
  classes = signal<{ id: number; name: string }[]>([]);
  exams = signal<{ id: number; name: string }[]>([]);
  subjects = signal<{ id: number; name: string }[]>([]);

  loading = signal<boolean>(false);

  constructor(
    private pcService: PointService,
    private classService: ClassService,
    private examService: ExaminationService,
    private router: Router
  ) { }

  ngOnInit() {
    this.loadClasses();
    this.loadExams();
  }

  // Load dropdowns
  loadClasses() {
    this.classService.getAll().subscribe(res =>
      this.classes.set(res.map(c => ({ id: c.classId, name: c.className })))
    );
  }

  loadExams() {
    this.examService.getAll().subscribe(res =>
      this.exams.set(res.map(e => ({ id: e.examId, name: e.examName })))
    );
  }

  // Add new detail row
  addDetail() {
    const newDetail: PointConditionDetailCreateDto = {
      fromMark: 0,
      toMark: 0,
      division: '',
      isSilverColor: false
    };

    this.pointCondition.update(pc => ({
      ...pc,
      details: [...pc.details, newDetail]
    }));
  }

  // Remove detail row
  removeDetail(index: number) {
    this.pointCondition.update(pc => ({
      ...pc,
      details: pc.details.filter((_, i) => i !== index)
    }));
  }

  // Save master-detail
  save() {
    const pc = this.pointCondition();
    if (!pc.educationYear.trim() || !pc.classId || !pc.examId || !pc.subjectId) {
      alert('Please fill all required fields.');
      return;
    }

    this.loading.set(true);
    this.pcService.create(pc).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/point']);
      },
      error: err => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }
}
