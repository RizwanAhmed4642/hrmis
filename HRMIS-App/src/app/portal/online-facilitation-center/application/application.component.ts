import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ApplicationAttachment, ApplicationDocument } from '../../../modules/application-fts/application-fts';
import { CookieService } from '../../../_services/cookie.service';
import { RootService } from '../../../_services/root.service';
import { OnlineFacilitationCenterService } from '../online-facilitation-center.service';


@Component({
  selector: 'app-application',
  templateUrl: './application.component.html',
})
export class ApplicationComponent implements OnInit {


  public today = new Date();
  public count = 3;
  public cnic: string = '3520052149083';
  
  public applicationTypes: any [] = [];
  public applications: any [] = [];
  public dialogClosed: boolean = true;
  public applicationDocuments: ApplicationDocument [] = [];
  public applicationAttachments: ApplicationAttachment [] = [];
  public resFound: boolean = false;
  public appsNotFound: boolean = false;

  constructor(
    private _rootService: RootService, 
    private _onlineFacilitationService: OnlineFacilitationCenterService,
    private router: Router,
    private route: ActivatedRoute,
    private _cookieService: CookieService
    ) { }

  ngOnInit() {
    // this.getApplications();
    this.fetchParams();
    this.getApplicationsTypes();
    this.getApplicationDocuments();
  }

  private fetchParams() {
    this.cnic = this._cookieService.getCookie('cnicussrpublic');
    if (this.cnic) {
      //this.getApplications();
    }
  }

  public getApplications()
  {
    this._onlineFacilitationService.getProfileDetail(this.cnic, 2).subscribe((data: any) => {
      if(data)
      {
        this.resFound = true;
        this.applications = data;
        if(this.applications.length == 0){
          this.appsNotFound = true;
        }
        console.log('Applications: ', this.applications);
      }
    }, err => {
      console.log(err);
    });
  }

  private getApplicationsTypes()
  {
      this._rootService.getApplicationTypesActive().subscribe((data: any) => {
        if(data)
        {
          this.applicationTypes = data;
        }
    }, err => {
      console.log(err);
    });
  }

  public openApplicationTypes(link)
  {
    this.dialogClosed = false;
    if(link != null)
    {
      this.router.navigate([link]);
    }

  }

  public closeWindow()
  {
    this.dialogClosed = true;
  }
  public openApplicationDetails(appId, appTrackingNumber)
  {
    if(appId != null)
    {
      this.router.navigate(['online-facilitation-center/application/details' + '/' + appId + '/' + appTrackingNumber]);
    }
  }

  
  private getApplicationDocuments = () => {
    // this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(1).subscribe((res: any[]) => {
      if (res) {

        let tempIds = [];
        this.applicationAttachments.forEach(x => {
          let doc = res.find(y => y.Id == x.Document_Id);
          if (doc) {
            tempIds.push(doc.Id);
          }
        });
        res.forEach(doc => {
          let id = tempIds.find(z => z == doc.Id);
          if (!id) {
            this.applicationDocuments.push(doc);
          }
        });
      
      }

      // this.loadingDocs = false;
      // this.uploadingAttachments = false;
      // this.applicationAttachmentsUpload = [];
      // console.log('App Docs: ', this.applicationDocuments);
      // console.log('App Atts: ', this.applicationAttachments);
      
    },
      err => { console.log(err);
       }
    );
  }

}
