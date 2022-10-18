import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class PublicPortalService {
  constructor(private http: HttpClient) { }
  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUser`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public saveProfile(profile: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveProfile')}`, profile);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getHFList(designationId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetHFList')}/${designationId}`);
  }
  public getPreferences(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetPreferences')}/${cnic}`);
  }
  public addPreferences(obj) {
    return this.http.post(`${Config.getControllerUrl('Public', 'AddPreference')}`, obj);
  }
  public removePreferences(id: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'RemovePreference')}/${id}`);
  }
  public saveServiceTemp(ServiceTemp: any) {
    return this.http.post(
      `${Config.getControllerUrl("Main", "SaveServiceTemp")}`, ServiceTemp
    );
  }
  public getServiceTemps(profileId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Main", "GetServiceTemp")}/${profileId}`
    );
  }
  public removeServiceTemp(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Main", "RemoveServiceTemp")}/${Id}`
    );
  }
  public getJobApplicants(job: any) {
    return this.http.post(`${Config.getControllerUrl('OnlineJob', 'GetJobApplicants')}`, job);
  }
  public getHrApplications(job: any) {
    return this.http.post(`${Config.getControllerUrl('OnlineJob', 'GetHrApplications')}`, job);
  }
  public getAdhocApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetJobApplications')}`, obj);
  }
  public getApprovedAdhocApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetApprovedJobApplications')}`, obj);
  }
  public getHrApplication(id: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetHrApplication')}/${id}`);
  }
  public getJobBatches(designationId: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetJobBatches')}/${designationId}`);
  }
  public getJobApplicant(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetJobApplicant')}/${applicantId}`);
  }
  public getJobApplicantQualifications(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetJobApplicantQualifications')}/${applicantId}`);
  }
  public getJobApplicantExperiences(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetJobApplicantExperiences')}/${applicantId}`);
  }
  public getJobApplicantPreferences(applicantId: number, jobId: number) {
    return this.http.get(`${Config.getControllerUrl('OnlineJob', 'GetJobApplicantPreferences')}/${applicantId}/${jobId}`);
  }
  public getAdhocApplication(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplication')}/${applicantId}`);
  }
  public getApplicantDocuments(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicantQualification')}/${applicantId}`);
  }
  public getExperiences(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetExperiences')}/${applicantId}`);
  }
  public addInterviewMarks(applicationId: number, marks: number) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'AddInterviewMarks')}`, {Id: applicationId, InterviewViewMarks: marks});
  }
  public getAdhocApplicant(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicant')}/${applicantId}`);
  }
}
