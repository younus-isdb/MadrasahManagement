import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';   // ✅ HERE

import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { ExaminationList } from './components/examination-list/examination-list';
import { ExaminationForm } from './components/examination-form/examination-form';
import { ExaminationEdit } from './components/examination-edit/examination-edit';
import { AppComponents } from './components/app-components/app-components';

@NgModule({
  declarations: [
    App,
    ExaminationList,
    ExaminationForm,
    ExaminationEdit,
    AppComponents
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
