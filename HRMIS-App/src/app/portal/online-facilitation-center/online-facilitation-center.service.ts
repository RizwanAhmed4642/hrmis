import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ApplicationAttachment, ApplicationLog, ApplicationMaster } from '../../modules/application-fts/application-fts';
import { Config } from '../../_helpers/config.class';
import { CookieService } from '../../_services/cookie.service';
import { LocalService } from '../../_services/local.service';


@Injectable({
  providedIn: 'root'
})
export class OnlineFacilitationCenterService {

  constructor(
    private http: HttpClient,
    private _localService: LocalService, 
    private _cookieService: CookieService,
    private router: Router ) { }

  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUser`, info);
  }

  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }

  logout() {
        this._cookieService.deleteCookie('ussr');
        this._cookieService.deleteCookie('cnicussrpublic');
        this._cookieService.deleteCookie('ussrpublic');
        this.router.navigate(['/online-facilitation-center/account']);
  }

  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfile')}/${cnic}`);
  }

  public getProfileExist(id: number, cnic: string, mobileno: string, email: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileExist')}/${id}/${cnic}/${mobileno}/${email}`);
  }

  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileByCNIC')}/${cnic}`);
  }

  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileDetail')}/${cnic}/${type}`);
  }

  public getServiceHistory(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetServiceHistory')}/${profileId}`);
  }

  // Service function to get data from File Master..
  // public getFileMaster() {
  //   return this.http.get(`${Config.getControllerUrl('Profile', 'GetFileMaster')}`);
  // }

  
  public getLeaveRecord(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetLeaveRecord')}/${profileId}`);
  }

  public getApplications(skip: number, pageSize: number, query: string, typeId: number, statusId: number, officeId?: number, from?: Date, to?: Date, sourceId?: number) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetApplications')}`,
      {
        Skip: skip, PageSize: pageSize, Type_Id: typeId, Status_Id: statusId, Source_Id: sourceId, OfficeId: officeId, Query: query,
        From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
      });
  }
  public getApplication(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplication')}/${id}/${tracking}`);
  }
  public getApplicationData(id: number, type: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationData')}/${id}/${type}`);
  }
  public getHFApplications(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFApplications')}/${hfmisCode}`);
  }
  /* public getHFApplications(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFApplications')}/${hfmisCode}`);
  } */
  public getHFApps(hfId: number, desigId: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFApps')}/${hfId}/${desigId}`);
  }
  public getHFDesignationApps(hfId: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFDesignationApps')}/${hfId}`);
  }
  getSelectedApplications(selectedApplicationsIds: number[]): any {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetSelectedInboxApplications')}`, selectedApplicationsIds);
  }
  public removeApplication(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'RemoveApplication')}/${id}/${tracking}`);
  }
  public submitApplication(application: ApplicationMaster) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
  }
  public submitApplicationDocument(document: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplicationDocument')}`, document);
  }
  public submitApplicationDocumentType(appTypeDoc: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplicationDocumentType')}`, appTypeDoc);
  }
  public uploadApplicationAttachments(applicationAttachments: ApplicationAttachment[], appId: number) {
    const formData = new FormData();
    applicationAttachments.forEach(attachment => {
      for (let key in attachment.files) {
        if (attachment.files.hasOwnProperty(key)) {
          let element = attachment.files[key];
          formData.append('file_' + attachment.Document_Id, element);
        }
      }
    });
    return this.http.post(`${Config.getControllerUrl('Application', 'UploadApplicationAttachments')}/${appId}`, formData);
  }
  public uploadSignedApplication(signedApplication: ApplicationAttachment, appId: number) {
    const formData = new FormData();
    for (let key in signedApplication.files) {
      if (signedApplication.files.hasOwnProperty(key)) {
        let element = signedApplication.files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Application', 'UploadSignedApplication')}/${appId}`, formData);
  }
  public createApplicationLog(applicationLog: ApplicationLog) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public submitFileMovement(applicationIds: number[]) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitFileMovement')}`, applicationIds);
  }
  public submitFileMovement2(applications: ApplicationMaster[]) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitFileMovement2')}`, applications);
  }
  public submitFileMovementFDO(applications: ApplicationMaster[], officer_Id: number) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitFileMovementFDO')}/${officer_Id}`, applications);
  }
  public searchProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'SearchProfile')}/${cnic}`);
  }
  public getOfficesHisdu(officer_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetInboxOfficesHisdu')}/${officer_Id}`);
  }
  public searchHealthFacilities(query: string, hfmisCode: string) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SearchHealthFacilities')}/${hfmisCode}`, { Query: query });
  }
  public getApplicationsHisdu(skip: number,
    pageSize: number, query: string,
    officeId: number, officer_Id: number,
    typeId?: number, statusId?: number,
    field?: string, fieldValue?: number, from?: Date, to?: Date
  ) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetInboxApplicationsHisdu')}`, {
      Skip: skip,
      PageSize: pageSize,
      officeId: officeId,
      Query: query,
      officer_Id: officer_Id,
      Pending: true,
      Type_Id: typeId,
      Status_Id: statusId,
      Field: field,
      FieldValue: fieldValue,
      From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
    });
  }

  public getApplicationLogs(id: number, lastId: number, orderAsc: boolean) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationLog')}/${id}/${lastId}/${orderAsc}`);
  }

 }
