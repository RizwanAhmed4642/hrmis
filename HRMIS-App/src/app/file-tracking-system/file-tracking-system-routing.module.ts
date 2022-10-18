import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { FileTrackingSystemComponent } from './file-tracking-system.component';
import { AuthGuard } from '../_guards/auth.guard';
import { InboxComponent } from './inbox/inbox.component';
import { MyApplicationsComponent } from './my-applications/my-applications.component';
import { ImportantComponent } from './important/important.component';
import { SentComponent } from './sent/sent.component';
import { ApplicationComponent } from './application/application.component';
import { RecordRoomComponent } from './record-room/record-room.component';
import { LetterComponent } from './letter/letter.component';
import { FileRequisitionComponent } from './file-requisition/file-requisition.component';
import { NewComponent } from './file-requisition/new/new.component';
import { AddEditComponent } from './scanned-files/add-edit/add-edit.component';
import { DispatchFilesComponent } from './file-requisition/dispatch-files/dispatch-files.component';
import { ScannedFilesComponent } from './scanned-files/scanned-files.component';
import { DiaryComponent } from './diary/diary.component';
import { DetailsComponent } from './application/details/details.component';
import { SearchTrackingComponent } from './search-tracking/search-tracking.component';
import { ScannedLawFilesComponent } from './scanned-law-files/scanned-law-files.component';
import { AddEditFileComponent } from './scanned-law-files/add-edit/add-edit-file.component';
import { CallsListComponent } from './scanned-law-files/calls-list/calls-list.component';
import { CauseListComponent } from './cause-list/cause-list.component';
import { AddEditCauseComponent } from './cause-list/add-edit/add-edit-cause.component';
import { InquiryFilesComponent } from './inquiry-files/inquiry-files.component';
import { EmployeeOnLeaveComponent } from './employee-on-leave/employee-on-leave.component';
import { ShowCauseLetterComponent } from './show-cause-letter/show-cause-letter.component';
import { SummariesComponent } from './summaries/summaries.component';
import { SummaryComponent } from './summary/summary.component';
import { ScannedDocumentsComponent } from './scanned-documents/scanned-documents.component';

const routes: Routes = [
  { path: '', component: FileTrackingSystemComponent, canActivate: [AuthGuard] },
  { path: 'diary', component: DiaryComponent, canActivate: [AuthGuard], data: { title: 'Diary' } },
  { path: 'inbox', component: InboxComponent, canActivate: [AuthGuard] },
  { path: 'my-applications', component: MyApplicationsComponent, canActivate: [AuthGuard] },
  { path: 'my-applications/:type/:statusId', component: MyApplicationsComponent, canActivate: [AuthGuard] },
  { path: 'my-applications/:type/:statusId/:from/:to', component: MyApplicationsComponent, canActivate: [AuthGuard] },
  { path: 'application/:id/:tracking', component: ApplicationComponent, canActivate: [AuthGuard] },
  { path: 'important', component: ImportantComponent, canActivate: [AuthGuard] },
  {
    path: 'file-requisition', component: FileRequisitionComponent, canActivate: [AuthGuard],
  },
  { path: 'file-requisition-new', component: NewComponent, canActivate: [AuthGuard] },
  { path: 'search-tracking', component: SearchTrackingComponent, canActivate: [AuthGuard], data: { title: 'Search Tracking' } },
  { path: 'scanned-files', component: ScannedFilesComponent, canActivate: [AuthGuard] },
  { path: 'awaiting', component: EmployeeOnLeaveComponent, canActivate: [AuthGuard] },
  { path: 'inquiry-files', component: InquiryFilesComponent, canActivate: [AuthGuard] },
  { path: 'lawwing-files', component: ScannedLawFilesComponent, canActivate: [AuthGuard] },
  { path: 'scanned-files-new', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: 'daily-calls-list', component: CauseListComponent, canActivate: [AuthGuard] },
  { path: 'daily-calls-new', component: AddEditCauseComponent, canActivate: [AuthGuard] },
  { path: 'lawwing-files-new', component: AddEditFileComponent, canActivate: [AuthGuard] },
  { path: 'lawwing-files-new/:id', component: AddEditFileComponent, canActivate: [AuthGuard] },
  { path: 'lawwing-files-new/:id/edit', component: AddEditFileComponent, canActivate: [AuthGuard] },
  { path: 'scanned-files-new/:filetype', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: 'scanned-files-new/:filetype/:id/edit', component: AddEditComponent, canActivate: [AuthGuard] },
  { path: 'file-dispatch', component: DispatchFilesComponent, canActivate: [AuthGuard] },
  { path: 'letter/:id/:tracking/:lettertype', component: LetterComponent, canActivate: [AuthGuard] },
  { path: 'show-cause-letter/:id', component: ShowCauseLetterComponent, canActivate: [AuthGuard] },
  { path: 'crr', component: RecordRoomComponent, canActivate: [AuthGuard] },
  { path: 'sent', component: SentComponent, canActivate: [AuthGuard] },
  { path: 'summaries', component: SummariesComponent, canActivate: [AuthGuard] },
  { path: 'scanned-documents', component: ScannedDocumentsComponent, canActivate: [AuthGuard] },
  { path: 'summary/:id/:tracking', component: SummaryComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class FileTrackingSystemRoutingModule { }
