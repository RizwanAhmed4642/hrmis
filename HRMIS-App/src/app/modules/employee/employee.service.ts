import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  constructor(private http: HttpClient) { }

  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfile')}/${cnic}`);
  }
  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileDetail')}/${cnic}/${type}`);
  }
  public getHealthFacilitiyDashboard(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFDashboardInfo')}/${hfmisCode}`);
  }
  public getHFProfileStatuses(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileStatuses')}/${hfmisCode}`);
  }
  public getHFProfiles(hfmisCode: string, type: number, id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfiles')}/${hfmisCode}/${type}/${id}`);
  }
  public getHFProfileDesignations(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileDesignations')}/${hfmisCode}`);
  }
  public getHFProfileEmpModes(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileEmpModes')}/${hfmisCode}`);
  }
  public submitApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
  }
  public checkApplications(HFId: number, TOHFId: number, DesgId) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetHFApplications')}/${HFId}/${TOHFId}/${DesgId}`);
  }
  public getApplicationMutualCode(FirstProfile_Id: number, SecondProfile_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetApplicationMutualCode')}/${FirstProfile_Id}/${SecondProfile_Id}`);
  }
  public verifyMutualCode(FirstProfile_Id: number, SecondProfile_Id: number, MutualCodeOne: number, MutualCodeTwo: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'VerifyMutualCode')}/${FirstProfile_Id}/${SecondProfile_Id}/${MutualCodeOne}/${MutualCodeTwo}`);
  }
  public createApplicationLog(applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public saveHrComplain(hrComplain: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveHrComplain')}`, hrComplain);
  }
  public getHrComplain() {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetHrComplain')}`);
  }
  public verifyProfileForTransfer(Id: number, reason_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'VerifyProfileForTransfer')}/${Id}/${reason_Id}`);
  }
  public subscribeAlert(profileId: number, vpMasterId: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'SubscribeAlert')}/${profileId}/${vpMasterId}`);
  }
  public searchProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'SearchProfile')}/${cnic}`);
  }
  public getServiceHistory(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetServiceHistory')}/${profileId}`);
  }
  public saveServiceHistory(serviceHistory: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveServiceHistory')}`, serviceHistory);
  }
  public getLeaveRecord(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetLeaveRecord')}/${profileId}`);
  }
  public saveProfileLeave(leaveRecord: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveLeaveRecord')}`, leaveRecord);
  }
  public removeServiceHistory(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveServiceHistory')}/${Id}`);
  }
  public uploadServiceAttachement(files: any[], serviceHistory_Id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'UploadServiceAttachement')}/${serviceHistory_Id}`, formData);
  }
  public uploaLeaveAttachement(files: any[], leaveRecord_Id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'UploaLeaveAttachement')}/${leaveRecord_Id}`, formData);
  }
  public removeLeaveRecord(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveLeaveRecord')}/${Id}`);
  }
  public saveHrQualification(obj) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveHrQualification')}`, obj);
  }
  public getHrQualification(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetHrQualification')}/${profileId}`);
  }
  public removeHrQualification(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveHrQualification')}/${Id}`);
  }
}
