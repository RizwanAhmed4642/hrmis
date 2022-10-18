import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class PublicPortalMOService {
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
  public saveConsultantProfile(consultantProfile: any) {
    return this.http.post(`${Config.getControllerUrl('Main', 'SavePromotedCandidate')}`, consultantProfile);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetProfileByCNIC')}/${cnic}`);
  }
  public savePromotionApplication(fd: any) {
    return this.http.post(`${Config.getControllerUrl('promotionapplication', 'Create')}`, fd);
  }
  public getBindingData() {
    return this.http.get(`${Config.getControllerUrl('promotionapplication', 'BindingData')}`);
  }
  public getDetailList(code: string) {
    return this.http.get(`${Config.getControllerUrl('Common', 'DetailList')}/${code}`);
  }
  public getPromoApp(id: any) {
    return this.http.get(`${Config.getControllerUrl('promotionapplication', 'GetPromoApp')}/${id}`);
  }
  public getPromoApps(skip: any, searchPhrase: any, dateStart: any, endDate: any) {
    return this.http.get(`${Config.getControllerUrl('promotionapplication', 'GetPromoApps')}?skip=${skip}&SearchPhrase=${encodeURI(searchPhrase)}&dateStart=${dateStart}&endDate=${endDate}`);
  }
  public checkProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('promotionapplication', 'CheckProfile')}/${cnic}`);
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getHFList(designationId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetHFList')}/${designationId}`);
  }
  public submitApplicationMO(profileId: number, mobileNumber: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'SubmitApplicationMO')}/${profileId}/1/${mobileNumber}`);
  }
  public getApplicationMO(profileId: number) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetApplicationMO')}/${profileId}`);
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
  public getPromotedCandidate(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Main", "GetPromotedCandidate")}/${cnic}`
    );
  }
  public getVacancy(designationId: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetVacancy')}/${designationId}`);
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
}
