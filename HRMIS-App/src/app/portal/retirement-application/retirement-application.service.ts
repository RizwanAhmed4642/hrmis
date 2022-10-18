import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class RetirementApplicationService {

  constructor(private http: HttpClient) { }


  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/RegisterOnlineApplicant`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }

  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfile')}/${cnic}`);
  }

  public getProfileExist(id: number, cnic: string, mobileno: string, email: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileExist')}/${id}/${cnic}/${mobileno}/${email}`);
  }

  public getLeaveRecord(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetLeaveRecord')}/${profileId}`);
  }
  public getServiceHistory(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetServiceHistory')}/${profileId}`);
  }
  public getMutualProfile(profileId: number, cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetMutualProfile')}/${profileId}/${cnic}`);
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getProfileApplicant(cnic: string, id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileApplicant')}/${cnic}/${id}`);
  }
  public submitApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
  }
  public getApplication(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplication')}/${id}/${tracking}`);
  }
  public getApplicationLogs(id: number, lastId: number, orderAsc: boolean) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationLog')}/${id}/${lastId}/${orderAsc}`);
  }
  public getApplicationData(id: number, type: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationData')}/${id}/${type}`);
  }
  public uploadApplicationAttachments(applicationAttachments: any[], appId: number) {
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
  public verifyProfileForTransfer(Id: number, reason_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'VerifyProfileForTransfer')}/${Id}/${reason_Id}`);
  }
  public createApplicationLog(applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public getOrderDesignations(hf_Id: number, hfmisCode: string, designation_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetOrderDesignations')}/${hf_Id}/${hfmisCode}/${designation_Id}`);
  }
}
