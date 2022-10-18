import { Injectable } from '@angular/core';
import { Modules } from './claims/claims.class';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';
import { UserClaims, User, PandSOfficerFilters } from './user-claims.class';

@Injectable()
export class UserService {

  constructor(private http: HttpClient) { }

  public getUserClaims(userId: string) {
    return this.http.get(`${Config.getControllerUrl('StaffManage', 'GetClaims')}/${userId}`);
  }
  public submitUserClaims(claims: UserClaims) {
    return this.http.post(`${Config.getControllerUrl('StaffManage', 'SaveClaims')}`, claims);
  }
  public getModules() {
    return this.http.get(`${Config.getControllerUrl('StaffManage')}/GetAllModules`);
  }
  public getAllComponents() {
    return this.http.get(`${Config.getControllerUrl('StaffManage')}/GetAllComponents`);
  }
  public getRoles() {
    return this.http.get(`${Config.getControllerUrl('StaffManage', 'GetRoles')}`);
  }
  public registerRole(roleName: string) {
    return this.http.get(`${Config.getControllerUrl('StaffManage', 'AddRole')}/${roleName}`);
  }
  public getUserById(userId: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserById')}/${userId}`);
  }
  public getUserWithById(userId: string) {
    return this.http.get(`${Config.getControllerUrl('Account', 'GetUserWithById')}/${userId}`);
  }
  public getUserRight() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetUserRight')}`);
  }
  public setUserRights(userRights: any) {
    return this.http.post(`${Config.getControllerUrl('Root', 'UserRight')}`, userRights);
  }
  public saveOfficer(officer: any) {
    return this.http.post(`${Config.getControllerUrl('Root', 'SaveOfficer')}`, officer);
  }
  public postFPsDataBeta(fpArray: any, officerId: number, iNumber: number) {
    return this.http.post(`${Config.getControllerUrl('Application', 'FPrintRegister')}/${officerId}/${iNumber}`, fpArray);
  }
  public getUsers(skip: number, pageSize: number, userName: string, roleName: string) {
    return this.http.post(`${Config.getControllerUrl('Account', 'Users')}`,
      { Skip: skip, PageSize: pageSize, UserName: userName, RoleName: roleName });
  }
  public saveOfficerData(pandSOfficerFilters: PandSOfficerFilters) {
    return this.http.post(`${Config.getControllerUrl('Root')}/SaveOfficerData`, pandSOfficerFilters);
  }
  public getUserModules(userId: string) {
    return this.http.get(`${Config.getControllerUrl('Erp')}/GetUserRights/${userId}`);
  }
  public saveUserModules(userModules: any) {
    return this.http.post(`${Config.getControllerUrl('Erp')}/SaveErpUserRights`, userModules);
  }
  public getModuleById(id: string) {
    return this.http.get(`${Config.getControllerUrl('StaffManage')}/GetModule/${id}`);
  }
  public saveModule(module: any) {
    return this.http.post(`${Config.getControllerUrl('Erp')}/SaveErpTables`, module);
  }
  public saveUser(model: User) {
    return this.http.post(`${Config.getControllerUrl('StaffManage')}/AddUser`, model);
  }
  public editUser(model: User) {
    return this.http.post(`${Config.getControllerUrl('StaffManage')}/EditUser`, model);
  }
}
