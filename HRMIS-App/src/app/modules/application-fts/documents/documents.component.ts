import { Component, OnInit, OnDestroy } from '@angular/core';
import { KGridHelper } from '../../../_helpers/k-grid.class';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { Subject } from 'rxjs/Subject';
import { ApplicationFtsService } from '../application-fts.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-fts-documents',
  templateUrl: './documents.component.html',
  styles: []
})
export class DocumentsComponent implements OnInit {
  public kGrid: KGridHelper = new KGridHelper();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public inputChange: Subject<any>;
  public newDocDialogOpened: boolean = false;
  public addingDocument: boolean = false;
  public addingDocumentType: boolean = false;

  public applicationDocument: any = {};
  public applicationDocumentType: any = {};

  constructor(private _applicationFtsService: ApplicationFtsService,
    private _rootService: RootService,
    private _notoficationService: NotificationService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    window.scroll(0, 0);
    this.initializeProps();
    this.loadDropDownValues();
  }
  private initializeProps() {
    this.kGrid.loading = false;
  }
  private loadDropDownValues() {
    this.getApplicationTypes();
    this.getApplicationDocuments();
  }
  public addDocumentType() {
    this.addingDocumentType = true;
    this._applicationFtsService.submitApplicationDocumentType(this.applicationDocumentType).subscribe((res) => {
      if (res) {
        this.applicationDocumentType = {};
        this._notoficationService.notify('success', 'Document Type Saved!');
        this.addingDocumentType = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public addDocument() {
    this.addingDocument = true;
    this._applicationFtsService.submitApplicationDocument(this.applicationDocument).subscribe((res) => {
      if (res) {
        this.applicationDocument = {};
        this._notoficationService.notify('success', 'Document Saved!');
        this.getApplicationDocuments();
        this.addingDocument = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public switchValChange(dataItem) {
    console.log(dataItem);
    this.requireDocument(dataItem.Id, dataItem.IsRequired);
  }
  private requireDocument = (id: number, isRequired: boolean) => {
    this._rootService.setRequiredAppDocument(id, isRequired).subscribe((res: any) => {
      if (res) {
        this._notoficationService.notify('success', 'Document Updated!');
      }else {
        this._notoficationService.notify('danger', 'Update Failed!');
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getApplicationDocuments = () => {
    this.dropDowns.applicationDocuments = [];
    this._rootService.getApplicationDocs().subscribe((res: any) => {
      if (res) {
        this.dropDowns.applicationDocuments = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getApplicationTypes = () => {
    this.dropDowns.applicationTypes = [];
    this._rootService.getApplicationTypesActive().subscribe((res: any) => {
      if (res) {
        this.dropDowns.applicationTypes = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private handleError(err: any) {
    this.kGrid.loading = false;
    this.addingDocumentType = false;
    this.addingDocument = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
}
