import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExaminationList } from './components/examination-list/examination-list';
import { ExaminationForm } from './components/examination-form/examination-form';
import { ExaminationEdit } from './components/examination-edit/examination-edit';
import { ExamfeeList } from './components/examfee/examfee-list/examfee-list';
import { ExamfeeCreate } from './components/examfee/examfee-create/examfee-create';
import { PointList } from './components/pointcondition/point-list/point-list';
import { PointCreate } from './components/pointcondition/point-create/point-create';
import { PointEdit } from './components/pointcondition/point-edit/point-edit';
import { ExamFeesList } from './components/examfeecollection/examfees-list/examfees-list';
import { ExamFeeCollectionCreate } from './components/examfeecollection/examfeecollection-create/examfeecollection-create';
import { ExamroutineList } from './components/examroutine/examroutine-list/examroutine-list';
import { LoginPage } from './components/login-page/login-page';
import { RegisterPage } from './components/register-page/register-page';
import { AppGuard } from '../app-guard';
import { AdminDashboardComponent } from './components/admindashboard-component/admindashboard-component';



const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginPage },
  { path: 'register', component: RegisterPage },
  {
    path: 'admin-dashboard',
    component: AdminDashboardComponent ,
    canActivate: [AppGuard],
    data: { roles: ['Admin'] }
  },

  { path: 'examination', component: ExaminationList, canActivate: [AppGuard] },
  { path: 'create', component: ExaminationForm },
  { path: 'edit/:id', component: ExaminationEdit },
  { path: 'examfee', component: ExamfeeList },
  { path: 'feecreate', component: ExamfeeCreate },
  { path: 'point', component: PointList },
  { path: 'pointcreate', component: PointCreate },
  { path: 'pointedit/:id', component: PointEdit },
  { path: 'examfeecollection', component: ExamFeesList },
  { path: 'examfeecollectioncreate', component: ExamFeeCollectionCreate },
  { path: 'examroutine', component: ExamroutineList}

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
