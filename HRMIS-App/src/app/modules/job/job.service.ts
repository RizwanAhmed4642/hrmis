import { Injectable } from "@angular/core";
import { Config } from "../../_helpers/config.class";
import { HttpClient } from "@angular/common/http";
import { Service } from "../database/services/services.class";
//import { VpMProfileView } from '../vacancy-position/vacancy-position.class';

@Injectable()
export class JobService {
  constructor(private http: HttpClient) { }


  public saveHFOpen(hFOpenedPosting: any) {
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveHFOpen")}`, hFOpenedPosting
    );
  }
  public saveJob(jonDTO: any) {
    return this.http.post(
      `${Config.getControllerUrl("Job", "SaveJob")}`, jonDTO
    );
  }
  public getJobs() {
    return this.http.get(
      `${Config.getControllerUrl("Job", "GetJobs")}`
    );
  }
  public getAdhocs() {
    return this.http.get(
      `${Config.getControllerUrl("Job", "GetAdhocs")}`
    );
  }
  public getJobApplications(obj) {
    return this.http.post(`${Config.getControllerUrl('Job', 'GetJobApplications')}`, obj);
  }
  public getJobApplicants(obj) {
    return this.http.post(`${Config.getControllerUrl('Job', 'GetJobApplicants')}`, obj);
  }
  public getPHFMCVacants() {
    return this.http.get(`${Config.getControllerUrl('Job', 'GetPHFMCVacants')}/designations/0`);
  }
  public getDocuments() {
    return this.http.get(
      `${Config.getControllerUrl("Job", "GetDocuments")}`
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
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
    );
    // return this.http.post(`${Config.getControllerUrl('Database', 'SaveService')}/${Id}`, Service);
  }
  public addService(Service: any) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Database", "SaveService")}`,
      Service
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
    debugger;
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
