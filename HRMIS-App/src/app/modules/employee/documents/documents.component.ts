import { Component, OnInit } from '@angular/core';
import { RootService } from '../../../_services/root.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { EmployeeService } from '../employee.service';

@Component({
  selector: 'app-documents',
  templateUrl: './documents.component.html',
  styles: []
})
export class DocumentsComponent implements OnInit {
  public currentUser: any;
  public profile: any;
  public hfPhoto = new Image();
  public hfmisCode: string = '0';
  public selectedTab: string = 'General Information';
  public healthFacility: any;
  public hfPhotoLoaded: boolean = false;
  public documents: any[] = [
    {Name: 'Matric'},
    {Name: 'Intermediate'},
    {Name: 'MBBS'},
    {Name: 'Diploma'},
  ];
  constructor(private _rootService: RootService,
    private _authenticationService: AuthenticationService,
    private _employeeService: EmployeeService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (!this.currentUser) {
      this._authenticationService.logout();
    }
    this.fetchData(this.currentUser.cnic);
  }
  private fetchData(cnic) {
    this.profile = null;
    this._employeeService.getProfile(cnic).subscribe(
      res => {
        this.profile = res as any;
        this.healthFacility = null;
      }, err => {
        console.log(err);
      }
    );
  }
  public selectFile(event, document: any): void {
  /*   let inputValue = event.target;
    let applicationAttachment: ApplicationAttachment = new ApplicationAttachment();
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
    this.formEvent.next(true); */
  }
  public onTabSelect(e) {
    if (!e.heading) {
      return;
    }
    this.selectedTab = e.heading;
  }

}
