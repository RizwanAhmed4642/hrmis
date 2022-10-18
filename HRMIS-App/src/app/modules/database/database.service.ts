import { Injectable } from "@angular/core";
import { Config } from "../../_helpers/config.class";
import { HttpClient } from "@angular/common/http";
import { Service } from "../database/services/services.class";
//import { VpMProfileView } from '../vacancy-position/vacancy-position.class';

@Injectable()
export class DatabaseService {
  constructor(private http: HttpClient) { }


  public saveHFOpen(hFOpenedPosting: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHFOpen")}`, hFOpenedPosting
    );
  }
  public getOpenHF() {
    return this.http.get(
      `${Config.getControllerUrl("Database", "GetOpenHF")}`
    );
  }
  public removeOpenHF(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "RemoveOpenHF")}/${Id}`
    );
  }
  public addVpProfileStatus(status_Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "AddVpProfileStatus")}/${status_Id}`
    );
  }
  public getVpProfileStatus(type: string) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "GetVpProfileStatus")}/${type}`
    );
  }
  public removeVpProfileStatus(status_Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("Database", "RemoveVpProfileStatus")}/${status_Id}`
    );
  }

  public getServices(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetServiceList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public editService(Service: any, Id: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
    );
    // return this.http.post(`${Config.getControllerUrl('Database', 'SaveService')}/${Id}`, Service);
  }
  public addService(Service: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
    );
  }

  public getCords(Address: any){
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetCords")}`,
      {Address}
    );
  }


  //add hftype
  public saveMertiActiveDesignation(maDesignation: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveMeritActiveDesignation")}`,
      maDesignation
    );
  }

  //add hftype
  public addHfType(hftype: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHfType")}`,
      hftype
    );
  }

  public editHfType(hftype: any, Id: number) {
    console.log(hftype);
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHfType")}`,
      hftype
    );
  }

  public getHfTypes(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetHfTypeList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public getDesignations(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetDesignationList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public getDesignationsforDDl() {
    return this.http.get(`${Config.getControllerUrl("Database", "GetDesignationsDDL")}`);
  }

  public getDesigSearch(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetDesigSearchList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

  public addDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveDesignation")}`,
      desig
    );
  }

  public editDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveDesignation")}`,
      desig
    );
  }

  public removeDesignation(desig: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "RemoveDesig")}`,
      desig
    );
  }

  public getHfCategories(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "GetHfCategoryList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }
}
