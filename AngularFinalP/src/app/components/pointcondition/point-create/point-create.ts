import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PointConditionCreateDto, PointConditionDetailCreateDto } from '../../../models/pointcondition';
import { PointService } from '../../../service/point-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

@Component({
  selector: 'app-point-create',
  standalone:false,
  templateUrl: './point-create.html',
  styleUrls: ['./point-create.css']
})
export class PointCreate implements OnInit {

  educationYear = signal<string>('');
  classId = signal<number | null>(null);
  examId = signal<number | null>(null);
  subjectId = signal<number | null>(null);
  passMarks = signal<number>(0);
  highestMark = signal<number>(0);
  details = signal<PointConditionDetailCreateDto[]>([]);

  classes = signal<{ id: number; name: string }[]>([]);
  exams = signal<{ id: number; name: string }[]>([]);
  subjects = signal<{ id: number; name: string }[]>([]);

  loading = signal<boolean>(false);

  constructor(
    private pointService: PointService,
    private classService: ClassService,
    private examService: ExaminationService,
    private subjectService: SubjectService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadClasses();
    this.loadExams();
    this.loadSubjects();
  }

  loadClasses() {
    this.classService.getAll().subscribe(res => this.classes.set(res.map(c => ({ id: c.classId, name: c.className }))));
  }

  loadExams() {
    this.examService.getAll().subscribe(res => this.exams.set(res.map(e => ({ id: e.examId, name: e.examName }))));
  }

  loadSubjects() {
    this.subjectService.getAll().subscribe(res => this.subjects.set(res.map(s => ({ id: s.subjectId, name: s.subjectName }))));
  }

  addDetail() {
    this.details.update(d => [...d, { fromMark: 0, toMark: 0, division: '', isSilverColor: false }]);
  }

  removeDetail(index: number) {
    this.details.update(d => d.filter((_, i) => i !== index));
  }

  save() {
    if (!this.educationYear().trim() || !this.classId() || !this.examId() || !this.subjectId()) return;

    const payload: PointConditionCreateDto = {
      educationYear: this.educationYear(),
      classId: this.classId()!,
      examId: this.examId()!,
      subjectId: this.subjectId()!,
      passMarks: this.passMarks(),
      highestMark: this.highestMark(),
      details: this.details()
    };

    this.loading.set(true);
    this.pointService.create(payload).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/point']);
      },
      error: () => this.loading.set(false)
    });
  }
}
