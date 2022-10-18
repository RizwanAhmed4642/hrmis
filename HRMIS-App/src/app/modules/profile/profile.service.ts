import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ProfileService {
  constructor(private http: HttpClient) { }
  public saveProfile(profile: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveProfile')}`, profile);
  }
  public saveHealthWorker(hrHealthWorker: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveHealthWorker')}`, hrHealthWorker);
  }
  public saveFocalPerson(hrFocalPerson: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveFocalPerson')}`, hrFocalPerson);
  }
  public saveShortProfile(profile: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveShortProfile')}`, profile);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfile')}/${cnic}`);
  }
  public getSenority(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'GetSeniority')}`, obj);
  }
  public getHealthWorker(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetHealthWorker')}/${cnic}`);
  }

  public submitForReview(profileReview: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SubmitForReview')}`, profileReview);
  }
  public submitReview(hrReview: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SubmitReview')}`, hrReview);
  }
  public getProfileReview(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileReview')}/${profileId}`);
  }
  public getVaccinationCertificate(profileId: number) {
    return this.http.get(`${Config.getControllerUrl("Profile", "GetVaccinationCertificate")}/${profileId}`
    );
  }
  public getDuplication(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetDuplication')}/${cnic}`);
  }
  public removeDuplication(id: number, cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'DeleteDuplication')}/${id}/${cnic}`);
  }
  public removeFocalPerson(id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveFocalPerson')}/${id}`);
  }
  public confirmJoining(id: number, date: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'ConfirmJoining')}/${id}/${date}`);
  }
  public getHrReview(reviewSubmissionId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetHrReviews')}/${reviewSubmissionId}`);
  }
  public saveServiceHistory(serviceHistory: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveServiceHistory')}`, serviceHistory);
  }
  public getServiceHistory(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetServiceHistory')}/${profileId}`);
  }
  public lockServiceHistory(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'LockServiceHistory')}/${profileId}`);
  }
  public removeServiceHistory(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveServiceHistory')}/${Id}`);
  }
  public saveAttachedPerson(attachedPerson: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveAttachedPerson')}`, attachedPerson);
  }
  public getAttachedPerson(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetAttachedPerson')}/${profileId}`);
  }
  public removeAttachedPerson(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveAttachedPerson')}/${Id}`);
  }
  public getLeaveRecord(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetLeaveRecord')}/${profileId}`);
  }
  public removeLeaveRecord(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveLeaveRecord')}/${Id}`);
  }
  public saveProfileLeave(leaveRecord: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveLeaveRecord')}`, leaveRecord);
  }
  public getProfiles(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[], selectedStatuses?: any[], retirementInOneYear?: boolean, retirementAlerted?: boolean) {
    return this.http
      .post(
        `${Config.getControllerUrl('Profile', 'GetProfiles')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations, statuses: selectedStatuses, retirementInOneYear: retirementInOneYear, retirementAlerted: retirementAlerted }
      );
  }

  public sendSMSToEmployee(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SendSMSToEmployee')}`, obj);
  }
  public getProfileReviews(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[]) {
    return this.http
      .post(
        `${Config.getControllerUrl('Profile', 'GetProfileReviews')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations }
      );
  }
  public getProfilesInPool(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[]) {
    return this.http
      .post(
        `${Config.getControllerUrl('Profile', 'GetProfilesInPool')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations }
      );
  }
  public getProfilesInActive(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, selectedCadres: any[], selectedDesignations: any[]): any {
    return this.http
      .post(
        `${Config.getControllerUrl('Profile', 'GetProfilesInActive')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: selectedCadres, designations: selectedDesignations }
      );
  }
  public getServiceDetails(Id: number) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetProfileServiceDetail")}`,
      { Id: Id }
    );
  }
  public checkOrderRequest(profileId: number) {
    return this.http.get(
      `${Config.getControllerUrl("OrderNotification", "CheckOrderRequest")}/${profileId}`
    );
  }
  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileDetail')}/${cnic}/${type}`);
  }
  public getProfileInquiries(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileInquiries')}/${profileId}`);
  }
  public saveProfileInquiry(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveInquiry')}`, obj);
  }
  public changePassword(oldPass, newPass, confirmPass) {
    return this.http.post(`${Config.getControllerUrl('Account', 'ChangePassword')}`, { OldPassword: oldPass, NewPassword: newPass, ConfirmPassword: confirmPass });
  }
  public postProfileRemarks(profileRemarks: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'PostProfileRemarks')}`, profileRemarks);
  }
  public getProfileRemarks(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileRemarks')}/${profileId}`);
  }
  public getProfileLogs(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileLogs')}/${profileId}`);
  }
  public getFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetFileAttachments')}/${fileId}`);
  }
  public removeFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveFileAttachments')}/${fileId}`);
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
  public uploadProfileAttachement(files: any[], profile_Id: number, profileRemarks_Id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'UploadProfileAttachments')}/${profile_Id}/${profileRemarks_Id}`, formData);
  }
  public removeProfileRemarks(id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveProfileRemarks')}/${id}`);
  }
  public uploadPhoto(files: any[], HFId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'UploadProfilePhoto')}/${HFId}`, formData);
  }
  public uploadVaccinationCertificate(files: any[], profile_Id: number, certificateNumber: string, typeId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'UploadVaccinationCertificate')}/${profile_Id}/${certificateNumber}/${typeId}`, formData);
  }
  public uploadFileAttachment(files: any[], ddsId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'UploadFileAttachment')}/${ddsId}`, formData);
  }

  public SaveLogs(obj) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveLogs')}`, obj);
  }

 
    
  
}
