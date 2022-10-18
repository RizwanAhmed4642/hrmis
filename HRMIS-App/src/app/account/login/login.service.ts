import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';

@Injectable()
export class LoginService {
  constructor(private http: HttpClient) { }
  login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }

  sendSMSCode(username, phoneNumber, email) {
    // return this.http.post(`${Config.getServerUrl()}/SendSMS`, user, phoneNumber);
    return this.http.post(
      `${Config.getControllerUrl("Root", "SendSMS")}`,
      {Username: username, PhoneNumber: phoneNumber, Email: email}
    );
  }
}
