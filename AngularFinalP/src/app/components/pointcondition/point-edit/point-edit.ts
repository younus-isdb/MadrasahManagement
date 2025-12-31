import { Component, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import {
  PointConditionReadDto,
  PointConditionUpdateDto,
  PointConditionDetailCreateDto
} from '../../../models/pointcondition';
import { PointService } from '../../../service/point-service';
import { ClassService } from '../../../service/class-service';
import { ExaminationService } from '../../../service/examinationService';
import { SubjectService } from '../../../service/subject-service';

@Component({
  selector: 'app-point-edit',
  standalone:false,
  templateUrl: './point-edit.html',
  styleUrls: ['./point-edit.css']
})
export class PointEdit implements OnInit {

  pointCondition = signal<PointConditionUpdateDto>({
    pointConditionId: 0,
    educationYear: '',
    classId: 0,
    examId: 0,
    subjectId: 0,
    passMarks: 0,
    highestMark: 0,
    details: []
  });

  classes = signal<{ id: number; name: string }[]>([]);
  exams = signal<{ id: number; name: string }[]>([]);
  subjects = signal<{ id: number; name: string }[]>([]);
  loading = signal(false);

  constructor(
    private route: ActivatedRoute,
    private pcService: PointService,
    private classService: ClassService,
    private examService: ExaminationService,
    private subjectService: SubjectService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadDropdowns();
    this.loadPointCondition();
  }

  loadDropdowns() {
    this.classService.getAll().subscribe(res =>
      this.classes.set(res.map(c => ({ id: c.classId, name: c.className })))
    );

    this.examService.getAll().subscribe(res =>
      this.exams.set(res.map(e => ({ id: e.examId, name: e.examName })))
    );

    this.subjectService.getAll().subscribe(res =>
      this.subjects.set(res.map(s => ({ id: s.subjectId, name: s.subjectName })))
    );
  }

  loadPointCondition() {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (!id) return;

    this.pcService.getById(id).subscribe((res: PointConditionReadDto) => {
      this.pointCondition.set({
        pointConditionId: res.pointConditionId,
        educationYear: res.educationYear,
        classId: res.classId,
        examId: res.examId,
        subjectId: res.subjectId,
        passMarks: res.passMarks,
        highestMark: res.highestMark,
        details: res.details.map(d => ({
          fromMark: d.fromMark,
          toMark: d.toMark,
          division: d.division,
          isSilverColor: d.isSilverColor
        }))
      });
    });
  }

  addDetail() {
    const detail: PointConditionDetailCreateDto = {
      fromMark: 0,
      toMark: 0,
      division: '',
      isSilverColor: false
    };

    this.pointCondition.update(pc => ({
      ...pc,
      details: [...pc.details, detail]
    }));
  }

  removeDetail(index: number) {
    this.pointCondition.update(pc => ({
      ...pc,
      details: pc.details.filter((_, i) => i !== index)
    }));
  }

  update() {
    this.loading.set(true);
    const model = this.pointCondition();

    this.pcService.update(model.pointConditionId, model).subscribe({
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
