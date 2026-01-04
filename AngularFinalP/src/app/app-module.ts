import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS, provideHttpClient, withInterceptors } from '@angular/common/http';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';

/* ---------------- Components ---------------- */
import { ExaminationList } from './components/examination-list/examination-list';
import { ExaminationForm } from './components/examination-form/examination-form';
import { ExaminationEdit } from './components/examination-edit/examination-edit';
import { AppComponents } from './components/app-components/app-components';
import { ExamfeeList } from './components/examfee/examfee-list/examfee-list';
import { ExamfeeCreate } from './components/examfee/examfee-create/examfee-create';
import { ExamfeeEdit } from './components/examfee/examfee-edit/examfee-edit';
import { PointList } from './components/pointcondition/point-list/point-list';
import { PointCreate } from './components/pointcondition/point-create/point-create';
import { PointEdit } from './components/pointcondition/point-edit/point-edit';
import { ExamFeesList } from './components/examfeecollection/examfees-list/examfees-list';
import { ExamFeeCollectionCreate } from './components/examfeecollection/examfeecollection-create/examfeecollection-create';
import { ExamfeecollectionEdit } from './components/examfeecollection/examfeecollection-edit/examfeecollection-edit';
import { ExamroutineList } from './components/examroutine/examroutine-list/examroutine-list';
import { Examroutinecreate } from './components/examroutine/examroutinecreate/examroutinecreate';
import { ExamroutineEdit } from './components/examroutine/examroutine-edit/examroutine-edit';
import { RegisterPage } from './components/register-page/register-page';
import { LoginPage } from './components/login-page/login-page';
import { AdminDashboardComponent } from './components/admindashboard-component/admindashboard-component';
import { ExamIncomeIndex } from './components/examincome/exam-income-index/exam-income-index';
import { ExamIncomeCreate } from './components/examincome/exam-income-create/exam-income-create';
import { ExamIncomeEdit } from './components/examincome/exam-income-edit/exam-income-edit';
import { TokenInterceptor, TokenInterceptorFn } from './service/token-interceptor';

@NgModule({
  declarations: [
    App,
    ExaminationList,
    ExaminationForm,
    ExaminationEdit,
    AppComponents,
    ExamfeeList,
    ExamfeeCreate,
    ExamfeeEdit,
    PointList,
    PointCreate,
    PointEdit,
    ExamFeesList,
    ExamFeeCollectionCreate,
    ExamfeecollectionEdit,
    ExamroutineList,
    Examroutinecreate,
    ExamroutineEdit,
    RegisterPage,
    LoginPage,
    AdminDashboardComponent,
    ExamIncomeIndex,
    ExamIncomeCreate,
    ExamIncomeEdit
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule       // ✅ MUST
  ],
  providers: [
    //{
    //  provide: HTTP_INTERCEPTORS,
    //  useClass: TokenInterceptor,  // ✅ ensures token is attached
    //  multi: true
    //}
    provideHttpClient(withInterceptors([TokenInterceptorFn])),
    provideBrowserGlobalErrorListeners()
  ],
  bootstrap: [App]
})
export class AppModule { }
