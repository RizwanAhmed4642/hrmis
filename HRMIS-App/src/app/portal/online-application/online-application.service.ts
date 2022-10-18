import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class OnlineApplicationService {

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

  public getMutualProfile(profileId: number, cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetMutualProfile')}/${profileId}/${cnic}`);
  }
  public getProfileByCNIC(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileByCNIC')}/${cnic}`);
  }
  public getProfileApplicant(cnic: string, id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfileApplicant')}/${cnic}/${id}`);
  }
  public submitApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
  }
  public verifyProfileForTransfer(Id: number, reason_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'VerifyProfileForTransfer')}/${Id}/${reason_Id}`);
  }
  public createApplicationLog(applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public getOrderDesignations(hf_Id: number, hfmisCode: string, designation_Id: number) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetOrderDesignations')}/${hf_Id}/${hfmisCode}/${designation_Id}`);
  }
}
