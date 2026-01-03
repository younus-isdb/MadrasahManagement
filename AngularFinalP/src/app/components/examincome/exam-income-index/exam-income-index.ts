import { Component, OnInit } from '@angular/core';
import { ExamIncomeReadDto } from '../../../models/ExamIncome';
import { ExamincomeService } from '../../../service/examincome-service';

@Component({
  selector: 'app-exam-income-index',
  standalone: false,
  templateUrl: './exam-income-index.html'
})
export class ExamIncomeIndex implements OnInit {

  list: ExamIncomeReadDto[] = [];

  constructor(private service: ExamincomeService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this.service.getAll().subscribe(res => {
      this.list = res;
    });
  }

  delete(id: number) {
    if (confirm('Are you sure?')) {
      this.service.delete(id).subscribe(() => {
        this.loadData();
      });
    }
  }
}
