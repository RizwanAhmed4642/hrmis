import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  constructor(private http: HttpClient) {
  }

  getUsers() {
    return this.http.get(`http://localhost:8913/api/DataTransfer/Users`);
  }
  public getHrmisDashboard() {
    return this.http.get(`${Config.getControllerUrl('DashboardHrmis', 'GetDashboardHrmis')}`);
  }
  public getUserActivity(skip: number, pageSize: number, query: string) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetUserActivity')}`, { Skip: skip, PageSize: pageSize, Query: query });
  }
  public getUserVpProfileActivity(skip: number, pageSize: number, query: string) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetUserVpProfileActivity')}`, { Skip: skip, PageSize: pageSize, Query: query });
  }
  public getApplicationChart(start: any, end: any, skip: number, pageSize: number) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetApplicationChart')}`, { from: start, to: end });
  }
  public getSectionDashboard() {
    return this.http.get(`${Config.getControllerUrl('SectionDashboard', 'GetDashboardHrmis')}`);
  }
  public submitHrApplication(obj) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitHrApplication')}`, obj);
  }
  public addToPool(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'AddToPool')}/${cnic}`);
  }
  public removeFromPool(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'RemoveFromPool')}/${cnic}`);
  }
  public getDashboardFcAppFwdCounts(start: any, end: any, program: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'DashboardFcAppFwdCount')}`, { from: start, to: end, program: program });
  }
  public getDashboardPendency3(start: any, end: any, program: string, type?: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'DashboardPendency3')}`, { from: start, to: end, program: program, type: type ? type : '' });
  }
  public dashboardPendencyCount(start: any, end: any, program: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'DashboardPendencyCount')}`, { from: start, to: end, program: program });
  }
  public pucDashboard(obj: any) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'PUCDashboard')}`, obj);
  }
  public dashboardPendencyCountTypes(start: any, end: any, program: string, typeId: number) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'DashboardPendencyCount')}`, { from: start, to: end, program: program, TypeId: typeId });
  }
  public getPostingReport() {
    return this.http.get(`${Config.getControllerUrl('DashboardHrmis', 'GetPostingReport')}`);
  }
  public fetchCovidData() {
    return this.http.get(`${Config.getControllerUrl('Public', 'FetchCovidData')}`);
  }
  public checkApplicationMutualCode(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Root', 'CheckApplicationMutualCode')}/${cnic}`);
  }
  public verifySecondMutualCode(mutualId: number, secondCode: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'VerifySecondMutualCode')}/${mutualId}/${secondCode}`);
  }
  public submitApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
  }
  public createApplicationLog(applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public checkApplications(HFId: number, TOHFId: number, DesgId) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetHFApplications')}/${HFId}/${TOHFId}/${DesgId}`);
  }
  public getCovidDBData() {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetCovidDBData')}`);
  }
  public saveServiceHistory(serviceHistory: any) {
    return this.http.post(`${Config.getControllerUrl('Profile', 'SaveServiceHistory')}`, serviceHistory);
  }
  public saveCovidData(counts: any) {
    return this.http.post(`${Config.getControllerUrl('Public', 'SaveCovidData')}`, counts);
  }
  public getSectionReportNew(start: any, end: any) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetSectionReportNew')}`, { from: start, to: end });
  }
  public getSectionReportNew22(start: any, end: any, sourceId: number) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetSectionReportNew22')}`, { from: start, to: end, SourceId: sourceId });
  }
  public getMySectionReportNew(start: any, end: any, officerId: number) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetMySectionReportNew')}`, { from: start, to: end, OfficerId: officerId });
  }
  public getPensionCasesReport(start: any, end: any, program: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetPensionCasesReport')}`, { from: start, to: end, Program: program });
  }
  public getCRRReport() {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetCRRReport')}`, { OfficerId: 0 });
  }
  public getFDOReportTypeWiseDate(start: any, end: any, userId: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetFDOReportTypeWiseDate')}`, { from: start, to: end, UserId: userId });
  }
  public getRiBranchReportTypeWiseDate(start: any, end: any, userId: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetRiBranchReportOfficeWiseDate')}`, { from: start, to: end, UserId: userId });
  }
  public getDiaryOfficeWiseDate(start: any, end: any, userId: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetDiaryOfficeWiseDate')}`, { from: start, to: end, UserId: userId });
  }
  public getLawwingReportOfficeWiseDate(start: any, end: any, userId: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetLawwingReportOfficeWiseDate')}`, { from: start, to: end, UserId: userId });
  }
  public getLawWingReport() {
    return this.http.get(`${Config.getControllerUrl('DashboardHrmis', 'GetLawWingReport')}`);
  }
  public getCitizenPortalReportOfficeWiseDate(start: any, end: any, userId: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetCitizenPortalReportOfficeWiseDate')}`, { from: start, to: end, UserId: userId });
  }
  public getApplication(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplication')}/${id}/${tracking}`);
  }
  public getApplications(skip: number, pageSize: number, query: string, typeId: number, statusId: number, officeId?: number, from?: Date, to?: Date, program?: string) {
    return this.http.post(`${Config.getControllerUrl('DashboardHrmis', 'GetApplications')}`,
      {
        Skip: skip, PageSize: pageSize, Type_Id: typeId, Program: program, Status_Id: statusId, OfficeId: officeId, Query: query,
        From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
      });
  }
  public getApplicationData(id: number, type: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationData')}/${id}/${type}`);
  }
  public getApplicationLogs(id: number, lastId: number, orderAsc: boolean) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationLog')}/${id}/${lastId}/${orderAsc}`);
  }
  public getAdhocCounts(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocApplicantCounts")}/${hfmisCode}`
    );
  }
  public getAdhocDashboardDistrict(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocDashboardDistrict")}/${hfmisCode}`
    );
  }
  public getAdhocApplicationScrutiny(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationScrutiny')}`, obj);
  }

  public gGetAdhocApplicationScrutinyPrint(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetAdhocApplicationScrutinyPrint')}`, obj);
  }

  public getAdhocApplicantsSummary() {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocApplicantsSummary")}`
    );
  }
  public getOfficerFilesCount() {
    return this.http.get(`${Config.getControllerUrl("Application", "GetOfficerFilesCount")}`
    );
  }
  public getOfficerFilesFiles(type: number) {
    return this.http.get(`${Config.getControllerUrl("Application", "GetOfficerFilesFiles")}/${type}`
    );
  }
  public getAdhocScrutinyCommittee(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl("Adhoc", "GetAdhocScrutinyCommittee")}/${hfmisCode}`
    );
  }
  public getAdhocApplicantsDash(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'GetJobApplicants')}`, obj);
  }
  public saveAdhocScrutinyCommittee(obj) {
    return this.http.post(`${Config.getControllerUrl('Adhoc', 'SaveAdhocScrutinyCommittee')}`, obj);
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
  public getVacancyMapHrmis(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetVacancyMapHrmis")}`, obj
    );
  }
  public getVacancyChartHrmis(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetVacancyChartHrmis")}`, obj
    );
  }
  public getVPChartCount(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetVPChartCount")}`, obj
    );
  }
  public saveVPChartCount(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveVPChartCount")}`, obj
    );
  }
  public getAttachedPersons(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetAttachedPersons")}`, obj
    );
  }
  public getEmployeePersons(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetEmployeePersons")}`, obj
    );
  }
  public getAnesthesiaPersons(obj) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetAnesthesiaPersons")}`, obj
    );
  }
  public getGeoJson() {
    return this.http.get(
      `${Config.getControllerUrl("Root", "GetDistrictGeoJson")}`);
  }
  public getSurgeries(code, filter) {
    return this.http.get(
      `${Config.getControllerUrl("Profile", "GetSurgeries")}/${code}/${filter}`);
    // return this.http.get('http://125.209.111.70:88/dhis/api/get_surgeries.php?id=' + code + '&type=' + filter);
  }
}
