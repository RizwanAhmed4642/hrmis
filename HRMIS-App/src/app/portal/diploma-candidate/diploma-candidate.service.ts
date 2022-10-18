import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class DiplomaCandidateService {

  constructor(private http: HttpClient) { }

  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUserAdhoc`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public saveDiplomaCandidate(diplomaCandidateDTO :any) {
    return this.http.post(`${Config.getControllerUrl('MeritDiploma', 'SaveMeritDiplomaCandidate')}`, diplomaCandidateDTO);
  }
  public getUserFull(UserName: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserFull')}/${UserName}`);
  }
  public getMerit(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('MeritDiploma', 'GetMerit')}/${cnic}`);
  }
  public getHFLists() {
    return this.http.get(`${Config.getControllerUrl('MeritDiploma', 'GetHFLists')}`);
  }
  public GetHFListsPreference() {
    return this.http.get(`${Config.getControllerUrl('MeritDiploma', 'GetHFListsPreference')}`);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('MeritDiploma', 'GetProfile')}/${cnic}`);
  }
  public getDesignations() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetDesignations")}`
    );
  }
  public uploadMeritProfilePhoto(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('MeritDiploma', 'UploadDocumentPhoto')}/${cnic}`, formData);
  }
  public getDistrict() {
    return this.http.get(
      `${Config.getControllerUrl("MeritDiploma", "GetDistrict")}`
    );
  }
}
