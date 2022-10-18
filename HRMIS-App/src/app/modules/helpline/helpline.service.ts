import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class HelplineService {
  constructor(private http: HttpClient) { }

  public editUser(model: any) {
    return this.http.post(`${Config.getControllerUrl('StaffManage')}/EditUser`, model);
  }
  public editAdhocApplicant(model: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc')}/EditAdhocApplicant`, model);
  }
  public editUserPhoneEmail(model: any) {
    return this.http.post(`${Config.getControllerUrl('StaffManage')}/EditUserPhoneEmail`, model);
  }
  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUserAdhoc`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public getUser(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUser')}/${UserName}`);
  }
  public getUserFull(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserFull')}/${UserName}`);
  }
  public getAdhocuser(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetAdhocuser')}/${UserName}`);
  }
  public getAdhocApplications(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplications')}/${applicantId}`);
  }
  public lockApplication(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'LockApplication')}/${applicationId}`);
  }
  public getJobs() {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetJobs')}`);
  }
  public getAdhocs(categoryId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocs')}/${categoryId}`);
  }
  public getDegrees(designationCatId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetDegrees')}/${designationCatId}`);
  }
  public getAdhocDesignations() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetAdhocDesignations')}`);
  }
  public getJobByDesignation(designationId) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetJobByDesignation')}/${designationId}`);
  }
  public getAdhocApplication(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplication')}/${applicantId}`);
  }
  public getApplicationPref(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationPref')}/${applicationId}`);
  }
  public getJobDocumentsRequired(jobId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetJobDocumentsRequired')}/${jobId}`);
  }
  public getApplicant(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicant')}/${cnic}`);
  }
  public getAdhocVacants(type: string, desigId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocVacants')}/${type}/${desigId}/0`);
  }
  saveApplicationGrievance(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveApplicationGrievance')}`, obj);
  }
  public getJobPreferences(jobId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetJobPreferences')}/${jobId}`);
  }
  public getApplicationGrievances(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationGrievances')}/${applicantId}`);
  }
  public getMerits(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetMeritProfile')}/${cnic}`);
  }
  public meritOrder(merit_Id: number, html: string) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'MeritOrder')}/${merit_Id}`, { OrderHTML: html });
  }
  public lockMeritOrder(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockMeritOrder')}`);
  }

  public lockMeritOrderSingle(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockMeritOrderSingle')}/${merit_Id}`);
  }

  UpdateOrderHTML(id: number, OrderHTML: string) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateOrderHTML')}`, { id: id, OrderHTML: OrderHTML });
  }
  public getMeritById(id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritById')}/${id}`);
  }
  public getMeritProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetMeritProfile')}/${cnic}`);
  }
  public saveProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddMeritProfile`, merit);
  }
  public saveApplicant(applicant: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc')}/SaveApplicant`, applicant);
  }
  public saveAdhocPreference(preference: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc')}/SaveAdhocPreference`, preference);
  }
  public saveAdhocApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc')}/SaveAdhocApplication`, application);
  }
  public getJobApplications(applicantId: number, hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetJobApplications')}/${applicantId}/${hfmisCode}`);
  }
  public uploadApplicantPhoto(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantPhoto')}/${cnic}`, formData);
  }
  public uploadApplicantPMDC(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantPMDC')}/${id}`, formData);
  }
  public uploadApplicantDomicile(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantDomicile')}/${id}`, formData);
  }
  public uploadApplicantCNIC(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantCNIC')}/${id}`, formData);
  }
  public uploadApplicantHifz(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantHifz')}/${id}`, formData);
  }
  public uploadApplicantQualification(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantQualification')}/${id}`, formData);
  }

  public uploadJobApplicantAttachments(jobApplicantAttachments: any[], cnic: string) {
    const formData = new FormData();
    jobApplicantAttachments.forEach(attachment => {
      for (let key in attachment.files) {
        if (attachment.files.hasOwnProperty(key)) {
          let element = attachment.files[key];
          formData.append('file_' + attachment.TotalMarks + '_' + attachment.ObtainedMarks
            /* + '_' + attachment.Degree
              + '_' + attachment.PassingYear
             + '_' + attachment.Division
             + '_' + attachment.Grade */
            + '_' + attachment.JobDocument_Id, element);
        }
      }
    });
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadJobApplicantAttachments')}/${cnic}`, formData);
  }
  public getMeritActiveDesignation(id) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/GetMeritActiveDesignation/${id}`);
  }
  public getDownloadLink(type: string, cnic: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/${type == 'acceptance' ? 'GetDownloadAcceptanceLetterLink' : 'GetDownloadOfferLink'}/${cnic}`);
  }
  public getDownloadLinkById(Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/GetDownloadOfferLinkById/${Id}`);
  }
  public getPreferences(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesList')}/${MeritID}`);
  }
  public getPreferencesVacancyColor(HfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesVacancyColor')}/${HfmisCode}`);
  }
  public getMeritPosting(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritPosting')}/${MeritID}`);
  }
  public getPostedMerits(HfmisCode: string, desigId: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPostedMerits')}/${HfmisCode}/${desigId}`);
  }
  public getPreferencesAll(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritPreferences')}/${MeritID}`);
  }
  public getPreferenceVacancy(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesVacancy')}/${hfmisCode}`);
  }
  public addPreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'SavePreferences')}/${obj.MeritId}/${obj.hfmisCode}`);
  }
  public saveAllPreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'SavePreferencesFinal')}/${obj.MeritId}`);
  }/* 
  public removePreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'RemovePreference')}/${obj.MeritId}/${obj.hfmisCode}`);
  } */
  public removePreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'RemovePreference')}/${obj.HfmisCode}/${obj.MeritId}`);
  }
  public saveExperience(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'AdhocApplicantExperience')}`, obj);
  }
  public uploadExperienceCertificate(files: any[], experienceId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadExperienceCertificate')}/${experienceId}`, formData);
  }
  public getExperiences(applicantId) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetExperiences')}/${applicantId}`);
  }
  public removeExperience(experienceId) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'RemoveExperience')}/${experienceId}`);
  }
  public getApplicantDocuments(applicantId) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicantDocuments')}/${applicantId}`);
  }
  public getApplication(applicantId, designationId) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplication')}/${applicantId}/${designationId}`);
  }
  public getHFOpened(DesignationId: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetHFOpened')}/${DesignationId}`);
  }
  public getPostingPlan(obj) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'GetMerits')}`, obj);
  }
  public getMeritSummary(obj) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'GetMeritSummary')}`, obj);
  }



  public saveApplicantQualification(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'AdhocApplicantQualification')}`, obj);
  }
  public getApplicantQualification(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicantQualification')}/${applicantId}`);
  }
  public removeApplicantQualification(Id: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'RemoveApplicantQualification')}/${Id}`);
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
  public uploadSignedAcceptance(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('JobApp', 'UploadAcceptance')}/${id}`, formData);
  }
}
