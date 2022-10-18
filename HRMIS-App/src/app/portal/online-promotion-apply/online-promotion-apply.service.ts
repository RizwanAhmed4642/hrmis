import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class OnlinePromotionApplyService {
  constructor(private http: HttpClient) { }

  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUserAdhoc`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public getUserPromotion(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserPromotion')}/${UserName}`);
  }
  public getUserFull(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserFull')}/${UserName}`);
  }
  public changePreferenceOrder(id: number, order: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'ChangePreferenceOrder')}/${id}/${order}`);
  }
  public removeApplicantPreference(id: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'RemoveApplicantPreference')}/${id}`);
  }
  public getAdhocApplications(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplications')}/${applicantId}`);
  }
  public lockApplication(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'LockApplication')}/${applicationId}`);
  }
  public sendConfimationSMS(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'SendConfimationSMS')}/${applicationId}`);
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
  public getAdhocScrutiny(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocScrutiny")}/${applicationId}`
    );
  }
  public getAdhocInterviewApplications(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewApplications")}/${applicantId}`);
  }
  public getAdhocPresentApplications(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocPresentApplications")}/${applicantId}`);
  }
  public getAdhocMLVL(DistrictCode: string, DesignationId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocMLVL")}/${DistrictCode}/${DesignationId}`);
  }
  public getAdhocGrievance(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocGrievance")}/${applicationId}`
    );
  }
  public getAdhocGrievancesByApplicant(applicantId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocGrievancesByApplicant")}/${applicantId}`
    );
  }
  public removeGrievance(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "RemoveGrievience")}/${Id}`
    );
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
  public getAdhocApplicationByDesig(applicantId: number, designationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationByDesig')}/${applicantId}/${designationId}`);
  }
  public getApplicationPref(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationPref')}/${applicationId}`);
  }
  public getJobDocumentsRequired(jobId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetJobDocumentsRequired')}/${jobId}`);
  }
  public getApplicant(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetSeniorityApplicantProfile')}/${cnic}`);
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
  public getApplicantQualificationById(id: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicantQualificationById')}/${id}`);
  }
  public getAdhocGrievanceUploads(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocGrievanceUploads')}/${applicantId}`);
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
  public uploadApplicantPMDC(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantPMDC')}/${cnic}`, formData);
  }
  public reUploadApplicantPMDC(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ReUploadApplicantPMDC')}/${cnic}`, formData);
  }
  public reUploadApplicantDomicile(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ReUploadApplicantDomicile')}/${cnic}`, formData);
  }
  public reUploadApplicantHifz(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ReUploadApplicantHifz')}/${cnic}`, formData);
  }
  public uploadApplicantDomicile(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantDomicile')}/${cnic}`, formData);
  }
  public uploadApplicantCNIC(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantCNIC')}/${cnic}`, formData);
  }
  public uploadApplicantHifz(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantHifz')}/${cnic}`, formData);
  }
  public uploadPositionDoc(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadPositionDoc')}/${cnic}`, formData);
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
  public getApplicationSingle(applicantId, designationId) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationSingle')}/${applicantId}/${designationId}`);
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

  public saveSeniorityApplication(model: any, selectedFiles: any[]) {
    var formData = new FormData();
    debugger;
    if (model) {
      for (let i = 0; i < selectedFiles.length; i++) {
        if (selectedFiles[i].Id == 1)
          formData.append('CNIC', selectedFiles[i]);
        else if (selectedFiles[i].Id == 2)
          formData.append('OrderCopy', selectedFiles[i]);
        else if (selectedFiles[i].Id == 3)
          formData.append('ChargeAssumption', selectedFiles[i]);
        else if (selectedFiles[i].Id == 4)
          formData.append('PPSCMeritList', selectedFiles[i]);
        else if (selectedFiles[i].Id == 5)
          formData.append('PromotionOrder', selectedFiles[i]);
        else if (selectedFiles[i].Id == 6)
          formData.append('ContractOrderCopy', selectedFiles[i]);
        else if (selectedFiles[i].Id == 7)
          formData.append('ContractJoining', selectedFiles[i]);
        else if (selectedFiles[i].Id == 8)
          formData.append('Status', selectedFiles[i]);
      }
      formData.append('Model', JSON.stringify(model));
    }
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveSeniorityApplicationProfile')}`, formData);
  }

  public getSeniorityList(filters: any) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetSeniorityList")}`, filters
    );
  }
  public getSeniorityListFixed(filters: any) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetSeniorityListFixed")}`, filters
    );
  }
  public generateSeniorityList(filters: any) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GenerateSeniorityList")}`, filters
    );
  }
  public getSeniorityApplicant(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Profile", "GetSeniorityApplicant")}/${cnic}`);
  }

}
