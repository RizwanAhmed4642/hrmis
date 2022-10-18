import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class OnlinePostApplyService {
  constructor(private http: HttpClient) { }



  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUser`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfile')}/${cnic}`);
  }

  public getProfileExistDesignation(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileExistDesignation')}/${cnic}`);
  }
  public getPromotionApplyData(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetPromotionApplyData')}/${cnic}`);
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getProfileApplicant(cnic: string, id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileApplicant')}/${cnic}/${id}`);
  }
  public saveProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddMeritProfile`, merit);
  }
  public updateSMOSatus(promotionApply: any) {
    return this.http.post(`${Config.getControllerUrl('JobApp')}/UpdateSMOSts`, promotionApply);
  }
  public submitSMOApplication(promotionApply: any) {
    return this.http.post(`${Config.getControllerUrl('JobApp')}/SubmitSMOApplication`, promotionApply);
  }
  public verifyMeritProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/VerifyMeritProfile`, merit);
  }
  public uploadMeritProfilePhoto(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Public', 'UploadProfilePhoto')}/${cnic}`, formData);
  }
  public getMeritActiveDesignation(id) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/GetMeritActiveDesignation/${id}`);
  }
  public getDownloadLink(type: string, profile_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/${type == 'acceptance' ? 'DownloadAcceptanceLetterSMO' : 'GetDownloadOfferLink'}/${profile_Id}`);
  }
  public getPreferences(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesList')}/${MeritID}`);
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
  public removePreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'RemovePreference')}/${obj.MeritId}/${obj.hfmisCode}`);
  }

  public getPromotionApply(obj) {
    return this.http.post(`${Config.getControllerUrl('Main', 'GetPromotionApply')}`, obj);
  }
  public uploadSignedAcceptance(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('JobApp', 'UploadAcceptanceSMO')}/${id}`, formData);
  }
}
