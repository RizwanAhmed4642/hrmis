import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';
import { VpMProfileView } from '../vacancy-position/vacancy-position.class';

@Injectable()
export class HealthFacilityService {
  constructor(private http: HttpClient) { }
  public saveHF(hf: any) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'SaveHF')}`, hf);
  }
  public getHealthFacility(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHF')}/${hfmisCode}`);
  }
  public rmvHealthFacility(HF_Id: number, userhfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'RmvHealthFacility')}/${HF_Id}/${userhfmisCode}`);
  }
  public getHealthFacilities(skip: number, pageSize: number, hfmisCode: string, hfTypeCode: any, hfCategoryCodes: any, hfACs: any, hfStatus: string) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'GetHFList')}`, { Skip: skip, PageSize: pageSize, HFMISCode: hfmisCode, HFTypes: hfTypeCode, HFCategories: hfCategoryCodes, HFACs: hfACs, HFStatus: hfStatus, });
  }
  public getHealthFacilitiyDashboard(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFDashboardInfo')}/${hfmisCode}`);
  }
  public getHFVacancy(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFVacancy')}/${hfmisCode}`);
  }

  public getSuggestedProfile(vpMaster_Id: number, hFId: number, scale: number) {
    return this.http.get(`${Config.getControllerUrl('Vacancy', 'GetSuggestedProfile')}/${vpMaster_Id}/${hFId}/${scale}`);
  }
  public getVpProfiles(vpMaster_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Vacancy', 'GetVpProfiles')}/${vpMaster_Id}`);
  }
  public shiftProfileToPSH(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'ShiftProfileToPSH')}/${profileId}`);
  }
  public getHFHOD(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFHOD')}/${hfmisCode}`);
  }
  public getHFProfileStatuses(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileStatuses')}/${hfmisCode}`);
  }
  public getHFProfileDesignations(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileDesignations')}/${hfmisCode}`);
  }
  public getPPSCDesignations(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetPPSCDesignations')}/${hfmisCode}`);
  }
  public getPPSCCandidates(hfmisCode: string, desingationId: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetPPSCCandidates')}/${hfmisCode}/${desingationId}`);
  }
  public getHFProfileEmpModes(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfileEmpModes')}/${hfmisCode}`);
  }
  public getHFProfiles(hfmisCode: string, type: number, id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFProfiles')}/${hfmisCode}/${type}/${id}`);
  }
  public getProfilePJHistory(ids: any[]) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'GetProfilePJHistory')}`, ids);
  }
  public confirmJoining(id: number, date: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'ConfirmJoining')}/${id}/${date}`);
  }
  public notJoined(id: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'NotJoined')}/${id}`);
  }
  public getHFPhoto(hf_Id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFPhoto')}/${hf_Id}`);
  }
  public getNewHFMISCode(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacilityD')}/GetNewHFMISCode/${hfmisCode}`);
  }
  public getProfilesAgainstVacancy(hf_Id: number, desigantion_Id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetProfilesAgainstVacancy')}/${hf_Id}/${desigantion_Id}`);
  }
  public getHFWards(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFWards')}/${hfmisCode}`);
  }
  public addHfWard(hfWard: any) {
    return this.http.post(`${Config.getControllerUrl('HealthFacilityD', 'AddHFWards')}`, hfWard);
  }
  public saveHFUC(hfUc: any) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'SaveHFUC')}`, hfUc);
  }
  public getHFMode(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFMode')}/${hfmisCode}`);
  }
  public getHFUCInfo(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFUCInfo')}/${hfmisCode}`);
  }
  public addHfMode(hfMode: any) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'SaveHFMode')}`, hfMode);
  }
  public editHfWard(hfWard: any, Id: number) {
    return this.http.post(`${Config.getControllerUrl('HealthFacilityD', 'EditHFWard')}/${Id}`, hfWard);
  }
  public updateHfWards(hfWards: any[]) {
    return this.http.post(`${Config.getControllerUrl('HealthFacilityD', 'UpdateHFWards')}`, hfWards);
  }
  public getVpDProfileViews(mId: number) {
    return this.http.get(`${Config.getControllerUrl('Vacancy')}/GetVpDProfileViews/${mId}`);
  }
  public saveVacancy(vpMaster: VpMProfileView) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'SaveVacancy')}`, vpMaster);
  }
  public rmvHFWard(id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'RmvHFWard')}/${id}`);
  }
  public getHFServices(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFServices')}/${hfmisCode}`);
  }
  public addHFService(serviceId: number, hf_Id: number, hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'AddHFServices')}/${serviceId}/${hf_Id}/${hfmisCode}`);
  }
  public addHFWardBeds(hfWardBed: any) {
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'AddHFWardBed')}`, hfWardBed);
  }
  public getHFWardsBeds(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'GetHFWardsBeds')}/${hfmisCode}`);
  }
  public rmvHFWardsBed(id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'RmvHFWardBeds')}/${id}`);
  }
  public rmvHFService(hfId: number, serviceId: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'RmvHFService')}/${hfId}/${serviceId}`);
  }
  public rmvHFVacancy(id: number) {
    return this.http.get(`${Config.getControllerUrl('HealthFacility', 'RmvHFVacancy')}/${id}`);
  }
  public uploadPhoto(files: any[], HFId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('HealthFacility', 'UploadHFPhoto')}/${HFId}`, formData);
  }
}
