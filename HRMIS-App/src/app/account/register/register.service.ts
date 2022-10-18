import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';

@Injectable()
export class RegisterService {
  constructor(private http: HttpClient) { }

  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUser`, info);
  }
  public addEUser(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddEUser`, info);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetProfile')}/${cnic}`);
  }
  registerStepOne(user: any) {
    return this.http.post(`${Config.getControllerUrl('Account')}/signup`, user);
  }

  public getProfileByCNIC(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Public", "GetProfileByCNIC")}/${cnic}`
    );
  }
  public getRegularProfileByCNIC(cnic: string) {
    return this.http.get(
      `${Config.getControllerUrl("Public", "GetRegularProfileByCNIC")}/${cnic}`
    );
  }
  public getVerificationCode(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/GetVCode`, obj);
  }
  public verifySmsCode(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/VerifyVCode`, obj);
  }

  register(user: any) {
    return this.http.post(`${Config.getControllerUrl('Account')}/Register`, user);
  }
 
  changePassword(passwordInfo: any){
    return this.http.post(`${Config.getControllerUrl('Account')}/changepassword`, passwordInfo);

  }
}
