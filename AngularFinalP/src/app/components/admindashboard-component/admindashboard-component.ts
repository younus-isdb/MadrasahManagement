import { Component, OnInit, AfterViewInit } from '@angular/core';
import { AuthService } from '../../service/authservice';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AppUserListItem } from '../../models/user';
import Chart from 'chart.js/auto';

interface Card { title: string; value: number | string; color: string; }

@Component({
  selector: 'app-admin-dashboard',
  standalone: false,
  templateUrl: './admindashboard-component.html',
  styleUrls: ['./admindashboard-component.css']
})
export class AdminDashboardComponent implements OnInit, AfterViewInit {
  apiBase = 'https://localhost:7113/api';
  loading = false;

  // Dashboard metrics
  totalStudents = 0;
  totalTeachers = 0;
  totalUsers = 0;
  todaysAttendance = 0;
  pendingFees = 0;
  totalExpenses = 0;
  totalFeeCollected = 0;
  totalActiveCourses = 0;

  cards: Card[] = [];

  // Tables
  users: AppUserListItem[] = [];
  assignRoleMap: Record<string, string> = {};
  roles: string[] = ['SuperAdmin', 'Admin', 'Teacher', 'Student'];

  todaysPayments: any[] = [];
  todaysAttendanceList: any[] = [];
  todayExpenses: any[] = [];

  // Charts
  chartMonths: string[] = [];
  feeCollectionTotals: number[] = [];
  expenseTotals: number[] = [];
  attendancePresent = 0;
  attendanceAbsent = 0;

  constructor(public auth: AuthService, private http: HttpClient, private router: Router) { }

  ngOnInit(): void {
    this.loadDashboard();
    this.loadUsers();
  }

  ngAfterViewInit(): void {
    setTimeout(() => this.renderCharts(), 500); // wait for data
  }

  loadDashboard() {
    // replace with real API call
    this.http.get<any>(`${this.apiBase}/AdminDashboard`).subscribe(res => {
      this.totalStudents = res.totalStudents;
      this.totalTeachers = res.totalTeachers;
      this.totalUsers = res.totalUsers;
      this.todaysAttendance = res.todaysAttendance;
      this.pendingFees = res.pendingFees;
      this.totalExpenses = res.totalExpenses;
      this.totalFeeCollected = res.totalFeeCollected;
      this.totalActiveCourses = res.totalActiveCourses;

      this.cards = [
        { title: 'Total Students', value: this.totalStudents, color: '#0d6efd' },
        { title: 'Total Teachers', value: this.totalTeachers, color: '#198754' },
        { title: 'Total Users', value: this.totalUsers, color: '#6f42c1' },
        { title: "Today's Attendance", value: this.todaysAttendance, color: '#fd7e14' },
        { title: 'Pending Fees', value: this.pendingFees, color: '#dc3545' },
        { title: 'Total Expenses', value: this.totalExpenses, color: '#20c997' },
        { title: 'Total Fee Collected', value: this.totalFeeCollected, color: '#0dcaf0' },
        { title: 'Active Courses', value: this.totalActiveCourses, color: '#ffc107' }
      ];

      this.todaysPayments = res.todayPayments;
      this.todaysAttendanceList = res.todaysAttendanceList;
      this.todayExpenses = res.todayExpenses;
      this.chartMonths = res.chartMonths;
      this.feeCollectionTotals = res.feeCollectionTotals;
      this.expenseTotals = res.expenseTotals;
      this.attendancePresent = res.attendancePresent;
      this.attendanceAbsent = res.attendanceAbsent;
    }, err => {
      console.error(err);
    });
  }

  loadUsers() {
    this.http.get<AppUserListItem[]>(`${this.apiBase}/Admin/Users`).subscribe(res => {
      this.users = res;
      this.users.forEach(u => this.assignRoleMap[u.id] = '');
    });
  }

  onAssignRole(user: AppUserListItem) {
    const newRole = this.assignRoleMap[user.id];
    if (!newRole) return alert('Select a role first');
    this.http.post(`${this.apiBase}/Admin/AssignRole`, { userId: user.id, role: newRole }).subscribe(() => {
      user.roles = [newRole];
      this.assignRoleMap[user.id] = '';
    });
  }

  onRemoveRole(user: AppUserListItem, role: string) {
    if (!confirm(`Remove role ${role} from ${user.email}?`)) return;
    this.http.post(`${this.apiBase}/Admin/RemoveRole`, { userId: user.id, role }).subscribe(() => {
      user.roles = user.roles.filter(r => r !== role);
    });
  }

  onDeleteUser(user: AppUserListItem) {
    if (!confirm(`Delete user ${user.email}?`)) return;
    this.http.delete(`${this.apiBase}/Admin/DeleteUser/${user.id}`).subscribe(() => {
      this.users = this.users.filter(u => u.id !== user.id);
    });
  }

  goCreateUser() { this.router.navigate(['/admin/users/create']); }
  goAddStudent() { this.router.navigate(['/students/create']); }
  goAddTeacher() { this.router.navigate(['/teachers/create']); }
  goCollectFee() { this.router.navigate(['/examfeecollection']); }
  goAddExpense() { this.router.navigate(['/expenses/create']); }
  goMarkAttendance() { this.router.navigate(['/attendance/mark']); }

  renderCharts() {
    new Chart('feeChart', {
      type: 'line',
      data: { labels: this.chartMonths, datasets: [{ label: 'Fee Collected', data: this.feeCollectionTotals, borderColor: 'rgb(75,192,192)', fill: false }] }
    });

    new Chart('expenseChart', {
      type: 'bar',
      data: { labels: this.chartMonths, datasets: [{ label: 'Expenses', data: this.expenseTotals, backgroundColor: 'rgb(255,99,132)' }] }
    });

    new Chart('attendancePie', {
      type: 'pie',
      data: { labels: ['Present', 'Absent'], datasets: [{ data: [this.attendancePresent, this.attendanceAbsent], backgroundColor: ['#198754', '#dc3545'] }] }
    });

    new Chart('paymentDonut', {
      type: 'doughnut',
      data: { labels: ['Collected', 'Pending'], datasets: [{ data: [this.totalFeeCollected, this.pendingFees], backgroundColor: ['#0dcaf0', '#dc3545'] }] }
    });
  }
}
