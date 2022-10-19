import { Injectable } from "@angular/core";
import { CookieService } from "./cookie.service";
import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Config } from "../_helpers/config.class";
import "rxjs/add/operator/map";
import { DDsDetail } from "../file-tracking-system/file-search.class";
import { dailyWagesProfile } from "../_models/_db/daily-wages.model";

@Injectable({
  providedIn: 'root'
})
export class DailyWagerService {

  constructor(
    private http: HttpClient,
  ) { }

  public AddDailyWages(WagesData: dailyWagesProfile) {
    return this.http.post(`${Config.getControllerUrl('DailyWagesProfile', 'SaveProfile')}`, WagesData);
  }

  public GetDailyWagerbyId(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("DailyWagesProfile", "GetDailyWagerbyId")}/${Id}`
    );
  }

  public AddContractFileById(Id: number,imageFile : any) {
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'AddContractFileById')}`,
        { Id: Id, imageFile: imageFile}
      );
  }

  public GetDailyWagesInPool(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[]) {
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWagesInPool')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations }
      );
  }

  public getProfiles(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[], selectedStatuses?: any[], retirementInOneYear?: boolean, retirementAlerted?: boolean, designationStr?: String) {
    debugger
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWages')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations, statuses: selectedStatuses, retirementInOneYear: retirementInOneYear, retirementAlerted: retirementAlerted, Designation:designationStr }
      );
  }

  public getProfilesForMap(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[], selectedStatuses?: any[], retirementInOneYear?: boolean, retirementAlerted?: boolean, designationStr?: String, value?: any) {
    debugger
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWages')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations, statuses: selectedStatuses, retirementInOneYear: retirementInOneYear, retirementAlerted: retirementAlerted, Designation:designationStr, value:value }
      );
  }



  public GetDailyWagesCount(skip: number, pageSize: number, hfmisCode: string,divisionCode:string, designations:any,districtCode:string,tehsilCode:string ) {
    console.log('aaaaaaaaaaaaaa');
    debugger
    
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWagesCount')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, divisionCode:divisionCode, designations: designations, districtCode:districtCode, tehsilCode:tehsilCode }
      );
  }

  
  public GetDailyDesignationbyName(name: string) {
    return this.http.get(
      `${Config.getControllerUrl("DailyWagesProfile", "GetDailyDesignationbyName")}/${name}`
    );
  }


  public GetDailyWagesCountProfile(skip: number, pageSize: number, hfmisCode: string, searchTerm: string, cadres: any[], designations: any[], selectedStatuses?: any[], retirementInOneYear?: boolean, retirementAlerted?: boolean, designationStr?:string) {
    console.log('aaaaaaaaaaaaaa');
    debugger
    return this.http
      .post(
        `${Config.getControllerUrl('DailyWagesProfile', 'GetDailyWagesCount')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm, cadres: cadres, designations: designations, statuses: selectedStatuses, retirementInOneYear: retirementInOneYear, retirementAlerted: retirementAlerted, Designation:designationStr }
      );
  }
  public GetProfileById(Id: number) {
    return this.http.get(
      `${Config.getControllerUrl("DailyWagesProfile", "GetProfileById")}/${Id}`
    );
  }
}
