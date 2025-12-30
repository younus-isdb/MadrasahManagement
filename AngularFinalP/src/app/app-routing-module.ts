import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExaminationList } from './components/examination-list/examination-list';
import { ExaminationForm } from './components/examination-form/examination-form';
import { ExaminationEdit } from './components/examination-edit/examination-edit';
import { ExamfeeList } from './components/examfee/examfee-list/examfee-list';
import { ExamfeeCreate } from './components/examfee/examfee-create/examfee-create'


const routes: Routes = [
  { path: 'examination', component: ExaminationList },
  { path: 'create', component: ExaminationForm },
  { path: 'edit/:id', component: ExaminationEdit },
  { path: 'examfee', component: ExamfeeList },
  {path:'feecreate',component:ExamfeeCreate}
 
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
