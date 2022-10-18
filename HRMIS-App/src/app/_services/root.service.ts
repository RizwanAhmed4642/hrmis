import { Injectable } from "@angular/core";
import { CookieService } from "./cookie.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Config } from "../_helpers/config.class";
import "rxjs/add/operator/map";
import { DDsDetail } from "../file-tracking-system/file-search.class";
@Injectable({
  providedIn: "root"
})
export class RootService {
  private checkb: any;
  constructor(
    private http: HttpClient,
    private _cookieService: CookieService
  ) { }
  public getCheque() {
    return this.checkb;
  }
  public setCheque(cb) {
    return (this.checkb = cb);
  }
  public search(query: string) {
    return this.http.post<any>(`${Config.getControllerUrl("Root", "Search")}`, {
      Query: query
    });
  }
  public getCurrentDate() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCurrentDate")}`);
  }
  public searchEmployees(obj: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SearchEmployees")}`
      , obj);
  }
  public searchHealthFacilities(query: string) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SearchHealthFacilities")}`,
      { Query: query }
    );
  }
  public getHealthFacilitiesAll() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetHealthFacilitiesAll")}`
    );
  }
  public getVaccinationCertificate(profileId: number) {
    return this.http.get(`${Config.getControllerUrl("Root", "GetVaccinationCertificate")}/${profileId}`
    );
  }
  public saveCovidFacility(obj: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SaveCovidFacility")}`,
      obj
    );
  }
  public saveCovidStaff(obj: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SaveCovidStaff")}`, obj
    );
  }
  public searchHealthFacilitiesAll(query: string) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SearchHealthFacilitiesAll")}`,
      { Query: query }
    );
  }
  public searchVacancy(query: string, designationId: number) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "SearchVacancy")}`,
      { Query: query, designationId: designationId }
    );
  }
  public removeCovidFacility(id: number) {
    return this.http.get(`${Config.getControllerUrl("Root", "RemoveCovidFacility")}/${id}`
    );
  }
  public getProfileById(Id: number) {
    return this.http.get(`${Config.getControllerUrl("Profile", "GetProfileById")}/${Id}`
    );
  }
  public removeCovidStaff(id: number) {
    return this.http.get(`${Config.getControllerUrl("Root", "RemoveCovidStaff")}/${id}`
    );
  }
  public getCovidStaff(covidFacilityId: number) {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCovidStaff")}/${covidFacilityId}`
    );
  }
  public getCovidFacilities(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCovidFacilities")}/${hfmisCode}`
    );
  }
  public getCovidFacilityTypes() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCovidFacilityTypes")}`
    );
  }
  public getHFCodMap(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetHFCodMap"
      )}/${hfmisCode}`
    );
  }
  public getHFVacs(hfmisCode: string, desgId) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetHFVacs"
      )}/${hfmisCode}/${desgId}`
    );
  }
  public getHrMarkings(desgnationId: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetHrMarkings"
      )}/${desgnationId}`
    );
  }
  public saveHrMarkings(hrMarking: any) {
    return this.http.post(
      `${Config.getControllerUrl(
        "Root",
        "SaveHrMarkings"
      )}`, hrMarking
    );
  }
  public generateOrderRequest(phfmcOrder: any) {
    return this.http.post(
      `${Config.getControllerUrl(
        "OrderNotification",
        "GenerateOrderRequest"
      )}`, phfmcOrder
    );
  }
  public getHealthFacilitiesAtDisposal() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetHealthFacilitiesAtDisposal")}`
    );
  }
  public getDivisions(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDivisions")}/${hfmisCode}`
    );
  }
  public getDistricts(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDistricts")}/${hfmisCode}`
    );
  }
  public getDistrictsVacancy(DesignationId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDistrictsVacancy")}/${DesignationId}`
    );
  }
  public getDistrictsLatLong(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "getDistrictsLatLong")}/${hfmisCode}`
    );
  }
  public getVacantPlaces(designationId: number, profileId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetVacantPosts")}/${designationId}/${profileId}`
    );
  }

  public postMerit(MeritID: number) {
    return this.http.get(
      `${Config.getControllerUrl("JobApp", "PostMerit")}/${MeritID}`
    );
  }
  public getApplicationPurposes() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetApplicationPurposes")}`
    );
  }
  public getPensionDocuments() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetPensionDocuments")}`
    );
  }
  public postMeritManually(MeritID: number, HF_Id: number, HFMISCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("JobApp", "PostMeritManually")}/${MeritID}/${HF_Id}/${HFMISCode}`
    );
  }
  public getHFOpened(query: string, DesignationId: number) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetHFOpened")}`,
      { Query: query, DesignationId: DesignationId }
    );
  }
  public getTehsils(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetTehsils")}/${hfmisCode}`
    );
  }

  public getCoordinates(name: string,Type:string) {
    return this.http.get(
      `${Config.getControllerUrl("DailyWagesProfile", "GetCorrdinates")}/${name}/${Type}`
    );
  }

  public GetDailyWagesMapCount(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[], selectedStatuses?: any[], retirementInOneYear?: boolean, retirementAlerted?: boolean, designationStr?: String) {
    debugger
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWagesMapCount')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations, statuses: selectedStatuses, retirementInOneYear: retirementInOneYear, retirementAlerted: retirementAlerted, Category:designationStr }
      );
  }
  
  public getHFUCs(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetHFUCs")}/${hfmisCode}`
    );
  }
  public GetHFUCsForDailyWages(hfmisCode: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetHFUCsForDailyWages")}/${hfmisCode}`
    );
  }
  public GetAllHFUCsForDailyWages() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetAllHFUCsForDailyWages")}`
    );
  }
  public GetBankForDailyWages() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetBankForDailyWages")}`);
  }
  
  public getWards() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetWards")}`
    );
  }
  public getWardsCustom(HF_Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetWardsCustom")}/${HF_Id}`
    );
  }
  public getHFTypes() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetTypes")}`);
  }
  public getHFCategories() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCategories")}`);
  }
  public getHFAC() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetHFAC")}`);
  }
  public getServices() {
    return this.http.get<any>(
      `${Config.getControllerUrl("Root", "GetServices")}`
    );
  }
  public getHFCategory(id) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetCategory")}/${id}`
    );
  }
  public getEmploymentModes() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetEmploymentModes`
    );
  }
  public getDepartments() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis")}/GetDepartments`
    );
  }
  public getDepartmentsHealth() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetDepartmentsHealth`
    );
  }
  public getDomiciles() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis")}/GetDomiciles`
    );
  }
  public getLanguages() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis")}/GetLanguages`
    );
  }
  public getSpecializations() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis")}/GetSpecializations`
    );
  }
  public getQualifications() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis")}/GetQualifications`
    );
  }
  public getProfileStatuses() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetProfileStatuses`
    );
  }
  public getCadres() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetCadres")}`);
  }
  public getPostTypes() {
    return this.http.get(`${Config.getControllerUrl("Root")}/GetPostTypes`);
  }
  public getApplicationTypes() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetApplicationTypes`
    );
  }
  public getApplicationDocs() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetApplicationDocs`
    );
  }
  public getApplicationTypesActive() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetApplicationTypesActive`
    );
  }
  public getFileRequestStatuses() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetFileRequestStatuses`
    );
  }
  public getApplicationStatus() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetApplicationStatus`
    );
  }
  public getApplicationSources() {
    return this.http.get(
      `${Config.getControllerUrl("Root")}/GetApplicationSources`
    );
  }
  public getOrderTypes() {
    return this.http.get(`${Config.getControllerUrl("Root")}/GetOrderTypes`);
  }
  public getDisposalOf() {
    return this.http.get(`${Config.getControllerUrl("Root")}/GetDisposalOf`);
  }
  public getLeaveTypes() {
    return this.http.get(`${Config.getControllerUrl("Root")}/GetLeaveTypes`);
  }
  public getDesignations() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetDesignations")}`
    );
  }
  public getDesignationCadreWise(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDesignationsCadreWise")}/${Id}`
    );
  }
  public getDisabilityTypes() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDisabilityTypes")}`
    );
  }
  public getInquiryStatus() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetInquiryStatus")}`
    );
  } public getPenaltyType() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetPenaltyType")}`
    );
  }
  public getDesignationsFiltered(filter: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetDesignationsFiltered")}`,
      filter
    );
  }
  public getConsultantDesignations() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetConsultantDesignations')}`);
  }
  public getMeritActiveDesignations() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetMeritActiveDesignations')}`);
  }
  public getHealthFacilitiesByType(hfmisCode: string, deptId?: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetHealthFacilitiesByType"
      )}/${hfmisCode}/${deptId}`
    );
  }
  public getHealthFacilities(hfmisCode: string, deptId?: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetHealthFacilities"
      )}/${hfmisCode}/${deptId}`
    );
  }
  public getHealthFacilitiesByTypeName(typeNames: any[]) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetHealthFacilitiesByTypeName")}`,
      typeNames
    );
  }
  public getHealthFacilitiesByTypeCode(typeCodes: any[]) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetHealthFacilitiesByTypeCode")}`,
      typeCodes
    );
  }
  public getEmployeesOnLeave(obj: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetEmployeesOnLeave")}`,
      obj
    );
  }
  public getPandSOfficers(type: string) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetPandSOfficers")}/${type}`
    );
  }
  public getPunjabOfficers() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetPunjabOfficers")}`
    );
  }
  public getAllPandSOfficers(filter: any) {
    return this.http.post(
      `${Config.getControllerUrl("Root", "GetAllPSOfficers")}`,
      filter
    );
  }
  public getOfficerData(userId: string, officerId: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "GetOfficerData"
      )}/${userId}/${officerId}`
    );
  }
  public getPandSOfficerFingerPrint(officerId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetPSOfficersFingers")}/${officerId}`
    );
  }
  public getAppTypePendancy(typeId: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetAppTypePendancy')}/${typeId}`);
  }
  public getQualificationType() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetQualificationType')}`);
  }
  public getDegrees(qualificationId: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetDegrees')}/${qualificationId}`);
  }
  public getAdhocCounts() {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocCounts')}`);
  }
  public getAdhocApplicantCounts(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicantCounts')}/${hfmisCode}`);
  }
  public getApplicationDocuments(typeId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetApplicationDocuments")}/${typeId}`
    );
  }
  public getOrderDocuments(typeId: number) {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetOrderDocuments")}/${typeId}`
    );
  }
  public setRequiredAppDocument(id: number, isRequired: boolean) {
    return this.http.get(`${Config.getControllerUrl('Root', 'SetRequiredAppDocument')}/${id}/${isRequired}`);
  }
  public calcDate(fromDate: string, toDate: string, totalDays: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "CalcDate"
      )}/${fromDate}/${toDate}/${totalDays}`
    );
  }
  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileDetail')}/${cnic}/${type}`);
  }
  public getVacancyReport(hfmisCode: string) {
    return this.http.get<any>(
      `${Config.getControllerUrl("Vacancy", "Report")}/${hfmisCode}`
    );
  }
  public getAnyProfile(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Profile", "GetAnyProfile")}/${cnic}`
    );
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Public", "GetProfileByCNIC")}/${cnic}`
    );
  }
  public UpdateOrderHTML(id: number, OrderHTML: string) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateOrderHTML')}`, { id: id, OrderHTML: OrderHTML });
  }
  public UpdateLeaveOrderHTML(leaveOrder) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateLeaveOrderHTML')}`, leaveOrder);
  }
  public getUserRightById(userRights: any) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetUserRightById')}`, userRights);
  }
  public saveEntityLog(entity_Log: any) {
    return this.http.post(`${Config.getControllerUrl('Root', 'SaveEntityLog')}`, entity_Log);
  }
  public getAdhocDesignations() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetAdhocDesignations')}`);
  }
  /*  public getFiles(query: string, skip: number, pageSize: number) {
         return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFiles')}`, { Skip: skip, PageSize: pageSize, Query: query });
     }
     public getFilesByCNIC(cnic: string) {
         return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFilesByCNIC')}`, { cnic: cnic });
     }
     public getFileByBarcode(barcode: string) {
         return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFileByCodeBar')}`, { Barcode: barcode });
     }
     public getFilesByFileNumber(fileNumber: string) {
         return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFilesByFileNumber')}`, { FileNumber: fileNumber });
     } */
  public getDDSFiles(query: string, skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSFiles")}`,
      { Skip: skip, PageSize: pageSize, Query: query }
    );
  }
  public getDDSDetails(ddsId: number, skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSDetails")}`,
      { Skip: skip, PageSize: pageSize, DDsId: ddsId }
    );
  }
  public saveDDSDetail(ddsDetail: DDsDetail) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "SaveDDSDetails")}`,
      ddsDetail
    );
  }
  public removeDDSDetail(ddsId: number) {
    return this.http.get(
      `${Config.getControllerUrl("FilesACRs", "RemoveDDSDetail")}/${ddsId}`
    );
  }
  public getDDsBarcode(id: number) {
    return this.http.get(
      `${Config.getControllerUrl("FilesACRs", "GetDDsBarcode")}/${id}`
    );
  }
  public getDDSFileById(id: number) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSFileById")}`,
      { DDsId: id }
    );
  }
  
  public getDDSFilesByCNIC(cnic: string) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSFilesByCNIC")}`,
      { cnic: cnic }
    );
  }
  public getDDSFileByBarcode(barcode: string) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSFileByCodeBar")}`,
      { Barcode: barcode }
    );
  }
  public getDDSFilesByFileNumber(fileNumber: string) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetDDSFilesByFileNumber")}`,
      { FileNumber: fileNumber }
    );
  }
  public getLawFilesByFileNumber(fileNumber: string) {
    return this.http.post(
      `${Config.getControllerUrl("FilesACRs", "GetLawFilesByFileNumber")}`,
      { FileNumber: fileNumber }
    );
  }
  public getPostNamesForVecancy() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetDesignationsVecancy")}`
    );
  }

  public getScales() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetScales")}`
    );
  }

  public getPostCadres() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetCadres")}`
    );
  }
  public generateBars(tracking: number) {
    return this.http.get<any>(
      `${Config.getControllerUrl("Application", "GenerateBars")}/${tracking}`
    );
  }
  public generateBarcodeRI(tracking: number) {
    return this.http.get<any>(
      `${Config.getControllerUrl("Application", "GenerateBarcodeRI")}/${tracking}`
    );
  }
  public getCurrentOfficer() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetCurrentOfficer")}`
    );
  }
  public getProfileAttachmentTypes() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetProfileAttachmentTypes")}`
    );
  }
  public getSessionInfo() {
    return this.http.get<any>(`https://ipapi.co/json`);
  }

  public saveIP(ip) {
    debugger;
    return this.http.post(`${Config.getControllerUrl("Root", "SaveSession")}`, {
      IP: ip
    });
  }

  // public saveAddress (){
    
  // }


  public getApplication(id: number, tracking: number) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Root",
        "SearchTracking"
      )}/${id}/${tracking}`
    );
  }
  public getApplicationData(id: number, type: string) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Application",
        "GetApplicationData"
      )}/${id}/${type}`
    );
  }
  public getApplicationLogs(id: number, lastId: number, orderAsc: boolean) {
    return this.http.get(
      `${Config.getControllerUrl(
        "Application",
        "GetApplicationLog"
      )}/${id}/${lastId}/${orderAsc}`
    );
  }

  public GetJobPostingMode() {
    return this.http.get(`${Config.getControllerUrl("Root", "GetHrPostingMode")}`
    );
  }

  public getFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetFileAttachments')}/${fileId}`);
  }
  public removeFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveFileAttachments')}/${fileId}`);
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
  public createApplicationLog(applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public submitFileMovement(applicationIds: number[]) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitFileMovement')}`, applicationIds);
  }
  public getMeritById(id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritById')}/${id}`);
  }
  public saveProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddMeritProfile`, merit);
  }
  public travelGuideTest(obj: any) {
    return this.http.post(`${Config.getControllerUrl('MWRoute')}/SaveRoute`, obj);
  }
  public getFTsToken(credentials: any) {
    const headers = new HttpHeaders({
      "Content-Type": "application/x-www-form-urlencoded"
    });
    return this.http.post(
      `https://fts.pshealthpunjab.gov.pk/Token`,
      "username=" +
      credentials.userName +
      "&password=" +
      credentials.password +
      "&grant_type=password"
    );
  }
  public getApplicationsFTs(trackingNumber: number, token: string) {
    const headers = new HttpHeaders({
      "Content-Type": "application/json",
      Authorization: "Bearer " + token
    });
    return this.http.get(
      `https://fts.pshealthpunjab.gov.pk/api/ApplicationsZ/SearchByTrackingAll/${trackingNumber}`,
      { headers: headers }
    );
  }
}
