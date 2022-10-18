import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';
import { VpMProfileView, VPHolder } from './vacancy-position.class';

@Injectable()
export class VacancyPositionService {

  constructor(private http: HttpClient) { }
  public saveVacancy(vpMaster: VpMProfileView) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'SaveVacancy')}`, vpMaster);
  }
  public saveVacancyHolder(vpHolder: VPHolder) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'HoldVacancy')}`, vpHolder);
  }
  public getVacancyPosition(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('VacancyPosition', 'GetVacancyPosition')}/${hfmisCode}`);
  }
  getVacancyData(dem: any[], mea: any[], filters: any) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'GetVacancyData')}`, { Dimensions: dem, Measures: mea, Filters: filters });
  }
  getVpMaster(obj) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'GetVacancyMaster')}`, obj);
  }
  public getVacancyReportDetail(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'ReportDetail')}`, obj);
  }
  public getProfilesAgainstVacancy(hf_Id: number, desigantion_Id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetProfilesAgainstVacancy')}/${hf_Id}/${desigantion_Id}`);
  }
  getVpHolder(obj) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'GetVacancyHolder')}`, obj);
  }
  getVpHolders(obj) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'GetVacancyHolders')}`, obj);
  }
  getVPQuota(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetVPQuota')}/${hfmisCode}`);
  }
  getVacancyQuota() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetVacancyQuota')}`);
  }
  downloadVacancyData(dem: any[], mea: any[], filters: any) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'DownloadVacancyData')}`, { Dimensions: dem, Measures: mea, Filters: filters }, { observe: 'response', responseType: 'blob' });
  }
}
