import { Component, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { PointService } from '../../../service/point-service';
import { PointConditionReadDto } from '../../../models/pointcondition';

@Component({
  selector: 'app-point-list',
  standalone:false,
  templateUrl: './point-list.html',
  styleUrls: ['./point-list.css']
})
export class PointList implements OnInit {
  pointConditions = signal<PointConditionReadDto[]>([]);
  loading = signal<boolean>(false);

  constructor(private pcService: PointService, private router: Router) { }

  ngOnInit() {
    this.loadPointConditions();
  }

  loadPointConditions() {
    this.loading.set(true);
    this.pcService.getAll().subscribe({
      next: res => {
        this.pointConditions.set(res);  // direct assignment
        this.loading.set(false);
      },
      error: err => {
        console.error(err);
        this.loading.set(false);
      }
    });
  }

  pointedit(id: number) {
    this.router.navigate(['/pointedit', id]);
  }

  delete(id: number) {
    if (confirm('Are you sure you want to delete this record?')) {
      this.pcService.delete(id).subscribe(() => this.loadPointConditions());
    }
  }
}
