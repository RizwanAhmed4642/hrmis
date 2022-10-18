import { Injectable } from "@angular/core";
import { Config } from "../../_helpers/config.class";
import { HttpClient } from "@angular/common/http";
import { Service } from "../database/services/services.class";
//import { VpMProfileView } from '../vacancy-position/vacancy-position.class';

@Injectable({
  providedIn: "root"
}) export class AdhocService {
  constructor(private http: HttpClient) { }

  public getAdhocApplicantsDash(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetJobApplicants')}`, obj);
  }
  public saveHFOpen(hFOpenedPosting: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveAdhocHFOpen")}`, hFOpenedPosting
    );
  }
  public saveAdhoc(jonDTO: any) {
    return this.http.post(
      `${Config.getControllerUrl("Adhoc", "SaveAdhocJob")}`, jonDTO
    );
  }
  public getAdhocs() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocs")}`
    );
  }
  public getOpenHF() {
    return this.http.get(
      `${Config.getControllerUrl("Database", "GetAdhocOpenHF")}`
    );
  }
  public getAdhocApplicantsSummary() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocApplicantsSummary")}`
    );
  }
  public getAdhocPendencySummary() {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocPendencySummary")}`
    );
  }
  public getAdhocApplicationStatus() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocApplicationStatus")}`
    );
  }
  public getAdhocJobs() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocJobs")}`
    );
  }
  public getAdhocScrutinyReasons() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocScrutinyReasons")}`
    );
  }
  public saveAdhocScrutiny(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocScrutiny')}`, obj);
  }
  public saveAdhocScrutinyGrievance(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocScrutinyGrievance')}`, obj);
  }
  public editAdhocScrutiny(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'EditAdhocScrutiny')}`, obj);
  }
  public getAdhocCounts() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocCounts")}`
    );
  }
  public changeAdhocInterviewStatus(id: number, statusId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "ChangeAdhocInterviewStatus")}/${id}/${statusId}`
    );
  }
  public getAdhocDashboardCounts() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocDashboardCounts")}`
    );
  }
  public getAdhocScrutiny(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocScrutiny")}/${applicationId}`
    );
  }
  public getAdhocApplicationMerit(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocApplicationMerit")}/${applicationId}`
    );
  }
  public getAdhocScrutinyByApplicant(applicantId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocScrutinyByApplicant")}/${applicantId}`
    );
  }
  public getAdhocApplicantPMC(applicantId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocApplicantPMC")}/${applicantId}`
    );
  }
  public getAdhocMerit(designationId: number, districtCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocMerit")}/${designationId}/${districtCode}`);
  }
  public getAdhocMeritLocked(designationId: number, districtCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocMeritLocked")}/${designationId}/${districtCode}`);
  }
  public saveAdhocMerit(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocMerit')}`, obj);
  }
  public getMeritVerification(batchApplicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetMeritVerification")}/${batchApplicationId}`
    );
  }
  public getMeritVerificationAll(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetMeritVerificationAll")}/${applicationId}`
    );
  }
  public activePosting() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "ActivePosting")}`
    );
  }
  public saveAdhocApplicantPMC(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocApplicantPMC')}`, obj);
  }
  public getAdhocApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetJobApplications')}`, obj);
  }
  public saveMeritVerification(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveMeritVerification')}`, obj);
  }
  public verifyPMC(pmcNo: string) {
    return this.http.post('https://www.pmc.gov.pk/api/DRC/GetData', {
      RegistrationNo: pmcNo,
      Name: null, FatherName: null
    });
  }
  public verifyPMCGetQualifications(pmcNo: string) {
    return this.http.post('https://www.pmc.gov.pk/api/DRC/GetQualifications', {
      RegistrationNo: pmcNo,
      Name: null, FatherName: null
    });
  }
  public getAdhocVacants(type: string, desigId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocVacants')}/${type}/${desigId}/0`);
  }
  public getAdhocVacantDesignations(type: string, desigId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocVacantDesignations')}/${type}/${desigId}/0`);
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
  public getAdhocApplicationApplicant(id: number, applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationApplicant')}/${id}/${applicationId}`);
  }
  public setAdhocMerit(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'SetAdhocMerit')}/${applicationId}`);
  }
  public getAdhocApplicationMarks(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationMarks')}/${applicationId}`);
  }
  public getAdhocJob(designationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocJob')}/${designationId}`);
  }
  public getMeritMarks() {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetMeritMarks')}`);
  }
  public undoAdhocScrutiny(id: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'UndoAdhocScrutiny')}/${id}`);
  }
  public getadhocApplicationMarks(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetadhocApplicationMarks')}/${applicationId}`);
  }
  public getAdhocDistrictMerit(designationId: number, districtCode: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocDistrictMerit')}/${designationId}/${districtCode}`);
  }
  public getAdhocApplicationScrutinyPrint(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationScrutinyPrint')}`, obj);
  }
  public getAdhocApplicationGrievanceScrutinyPrint(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationGrievanceScrutinyPrint')}`, obj);
  }
  public getAdhocAcceptedGrievancePrint(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocAcceptedGrievancePrint')}`, obj);
  }
  public getAdhocJobApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocJobApplications')}`, obj);
  }
  public getAdhocVerificationData(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocVerificationData')}`, obj);
  }
  public getAdhocJobInterviews(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocJobInterviews')}`, obj);
  }

  public getAdhocScrutinyCommittee(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocScrutinyCommittee")}/${hfmisCode}`
    );
  }
  public verifyHifzPosition(id: number, type: number, status: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "VerifyHifzPosition")}/${id}/${type}/${status}`
    );
  }
  public getAdhocInterviewBatchApplication(id: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewBatchApplication")}/${id}`
    );
  }
  public saveAdhocScrutinyCommittee(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocScrutinyCommittee')}`, obj);
  }
  public saveAdhocInterviewBatchApplication(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocInterviewBatchApplication')}`, obj);
  }
  public saveAdhocApplicationVerification(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocApplicationVerification')}`, obj);
  }
  public getAdhocInterviewVerifications(applicationId, batchApplicationId) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewVerifications")}/${applicationId}/${batchApplicationId}`);
  }
  public getScrutinyMinutes(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetScrutinyMinutes')}`, obj);
  }
  public saveScrutinyMinutes(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveScrutinyMinutes')}`, obj);
  }
  public searchAdhocInterviewBatchApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SearchAdhocInterviewBatchApplications')}`, obj);
  }
  public saveAdhocInterview(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocInterview')}`, obj);
  }
  public saveAdhocInterviewBatchCommittee(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocInterviewBatchCommittee')}`, obj);
  }
  public getAdhocInterviewBatchCommittee(interviewBatchId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewBatchCommittee")}/${interviewBatchId}`);
  }
  public sendAdhocInterviewSMS(interviewBatchId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "SendAdhocInterviewSMS")}/${interviewBatchId}`);
  }
  public checkAdhocInterviewSMS(interviewBatchId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "CheckAdhocInterviewSMS")}/${interviewBatchId}`);
  }
  public getAdhocInterview(districtCode: string, designationId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterview")}/${districtCode}/${designationId}`);
  }
  public getAdhocInterviews(districtCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviews")}/${districtCode}`);
  }
  public getAdhocInterviewBatches(interviewId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewBatches")}/${interviewId}`);
  }
  public getAdhocInterviewBatchApplications(interviewBatchId: number) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocInterviewBatchApplications")}/${interviewBatchId}`);
  }
  public uploadCommitteeNotification(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadCommitteeNotification')}/${id}`, formData);
  }
  public uploadScrutinyMinutes(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadScrutinyMinutes')}/${id}`, formData);
  }
  public uploadMeritList(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadMeritList')}/${id}`, formData);
  }
  public uploadGrievanceScrutinyMinutes(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadGrievanceScrutinyMinutes')}/${id}`, formData);
  }
  public uploadApplicantPositionDoc(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'UploadApplicantPositionDoc')}/${id}`, formData);
  }
  public getAdhocApplicationGrievances(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationGrievances')}`, obj);
  }
  public getAdhocApplicationScrutiny(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationScrutiny')}`, obj);
  }
  public changeAdhocApplicantQualification(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ChangeAdhocApplicantQualification')}`, obj);
  }
  public getApprovedJobApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetApprovedJobApplications')}`, obj);
  }
  public getAdhocApplication(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplication')}/${applicantId}`);
  }
  public getApplicationPref(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationPrefs')}/${applicationId}`);
  }
  public getApplicationPreference(applicationId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicationPref')}/${applicationId}`);
  }
  public getAdhocGrievance(applicationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetAdhocGrievance")}/${applicationId}`
    );
  }
  public changeApplicationStatus(applicantLog: any) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ChangeApplicationStatus')}`, applicantLog);
  }
  public getAdhocApplicants(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetJobApplicants')}`, obj);
  }
  public changeApplicationGrievanceStatus(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ChangeApplicationGrievanceStatus')}`, obj);
  }
  public changeApplicationGrievanceScrutinyStatus(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'ChangeApplicationGrievanceScrutinyStatus')}`, obj);
  }
  public acceptAdhocGrievanceScrutiny(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'AcceptAdhocGrievanceScrutiny')}`, obj);
  }
  public getApplicantDocuments(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetApplicantQualification')}/${applicantId}`);
  }
  public getExperiences(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetExperiences')}/${applicantId}`);
  }
  public getDocuments() {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "GetDocuments")}`
    );
  }
  public getAdhocApplicant(applicantId: number) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicant')}/${applicantId}`);
  }
  public downloadAdvertisement(jobId: number) {
    return this.http.get(
      `${Config.getControllerUrl("JobApp", "DownloadAdvertisement")}/${jobId}`
    );
  }
  public removeOpenHF(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "RemoveAdhocOpenHF")}/${Id}`
    );
  }
  public addVpProfileStatus(status_Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "AddVpProfileStatus")}/${status_Id}`
    );
  }
  public getVpProfileStatus(type: string) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "GetVpProfileStatus")}/${type}`
    );
  }
  public removeVpProfileStatus(status_Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "RemoveVpProfileStatus")}/${status_Id}`
    );
  }

  public getServices(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetServiceList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public editService(Service: any, Id: number) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
    );
    // return this.http.post(`${Config.getControllerUrl('Database', 'SaveService')}/${Id}`, Service);
  }
  public addService(Service: any) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
    );
  }

  //add hftype
  public addHfType(hftype: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHfType")}`,
      hftype
    );
  }

  public editHfType(hftype: any, Id: number) {
    debugger;
    console.log(hftype);
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHfType")}`,
      hftype
    );
  }

  public adhocApplicantContinueExp(experienceId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Adhoc", "AdhocApplicantContinueExp")}/${experienceId}`
    );
  }
  public getHfTypes(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetHfTypeList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public getDesignations(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetDesignationList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public getDesigSearch(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetDesigSearchList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public addDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveDesignation")}`,
      desig
    );
  }

  public editDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveDesignation")}`,
      desig
    );
  }

  public removeDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "RemoveDesig")}`,
      desig
    );
  }

  public getHfCategories(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetHfCategoryList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }


  public makePreferenceDistrictWise(preferences: any[], districts: any[]) {
    let finalDistricts: any[] = [];
    districts.forEach(district => {
      district.prefs = preferences.filter(x => x.HFMISCode.startsWith(district.Code));
    });
  }

}
