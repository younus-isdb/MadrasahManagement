import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';   // ✅ HERE

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ExaminationList } from './components/examination-list/examination-list';
import { ExaminationForm } from './components/examination-form/examination-form';
import { ExaminationEdit } from './components/examination-edit/examination-edit';
import { AppComponents } from './components/app-components/app-components';
import { ExamfeeList } from './components/examfee/examfee-list/examfee-list';
import { ExamfeeCreate } from './components/examfee/examfee-create/examfee-create';
import { PointList } from './components/pointcondition/point-list/point-list';
import { PointCreate } from './components/pointcondition/point-create/point-create';
import { ExamfeeEdit } from './components/examfee/examfee-edit/examfee-edit';
import { PointEdit } from './components/pointcondition/point-edit/point-edit';

@NgModule({
  declarations: [
    App,
    ExaminationList,
    ExaminationForm,
    ExaminationEdit,
    AppComponents,
    ExamfeeList,
    ExamfeeCreate,
    PointList,
    PointCreate,
    ExamfeeEdit,
    PointEdit
    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule    // ✅ MUST
  ],
  providers: [
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
