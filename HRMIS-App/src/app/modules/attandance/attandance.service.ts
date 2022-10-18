import { Injectable } from "@angular/core";
import { Config } from "../../_helpers/config.class";
import { HttpClient } from "@angular/common/http";

/* @Injectable({
  providedIn: 'root'
}) */

@Injectable()
export class AttandanceService {

  constructor(private http: HttpClient) {}

  public getAttandanceLog(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetAttandanceLog")}`,
      { Skip: skip, PageSize: pageSize }
    );
  }

 /*  public getLeaveList(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetLeaveList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  } */

/*   public getLeaveList(skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetLeaveList")}`,
      { Skip: skip, PageSize: pageSize }
    );
  } */

  public getEmpYear() {
    return this.http.get(`${Config.getControllerUrl("Attandance", "GetYears")}`);
  }

  //new angular project
  
  public getLeaveList(skip: number, pageSize: number, LeaveFrom: string, LeaveTo: string, EmployeeName: string) {
    debugger;
    return this.http
      .post(
        `${Config.getControllerUrl('Attandance', 'getLeaveList')}`,
        { skip: skip, pageSize: pageSize, LeaveFrom: LeaveFrom, LeaveTo: LeaveTo, EmployeeName:EmployeeName}
      );
  }

  public getAttendanceList(skip: number, pageSize: number, From: string, To: string, EmployeeName: string) {
   // debugger;
    return this.http
      .post(
        `${Config.getControllerUrl('Attandance', 'getAttendanceList')}`,
        { skip: skip, pageSize: pageSize, From: From, To: To, EmployeeName:EmployeeName}
      );
  }

  public getAttendanceListRpt(From: string, To: string, EmployeeName: string) {
    // debugger;
     return this.http
       .post(
         `${Config.getControllerUrl('Attandance', 'getAttendanceListRpt')}`,
         {From: From, To: To, EmployeeName:EmployeeName}
       );
   }

   public getAttendanceRec(HrId: number, Year: string, Month: string, IsLate: string, parm: string) {
     debugger;
     return this.http
       .post(
         `${Config.getControllerUrl('Attandance', 'getAttendanceRec')}`,
         {HrId: HrId, Year: Year, Month:Month, IsLate: IsLate, parm: parm }
       );
   }


//
  public getLeaveReport(skip: number, pageSize: number, EmployeeName: string, Year: number, Month: number) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetLeaveReport")}`,
      { Skip: skip, PageSize: pageSize, EmployeeName:EmployeeName, Year:Year, Month:Month }
    );
  }

  public getLeaveRpt(EmployeeName: string, Year: number, Month: number) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetLeaveReport")}`,
      {EmployeeName:EmployeeName, Year:Year, Month:Month }
    );
  }

  public getMonths(Year: number) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetMonths")}`,
      { Year:Year }
    );
  }

  public loadLeavesRec(HrId: number, Year: string, Month: string, parm: string) {
    debugger;
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "GetLeaveRec")}`,
      { HrId: HrId, Year: Year, Month:Month, parm:parm }
    );
  }
  
  public getEmpStatus() {
    return this.http.get(`${Config.getControllerUrl("Attandance", "GetEmpStatus")}`);
  }

  
  public editEmpStatus(at: any, Id: number) {
    debugger;
    console.log(at);
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "EditEmpStatus")}`,
      at
    );
  }


  //add leave
  public addLeave(leave: any) {
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "SaveLeave")}`,
      leave
    );
  }

  public editLeave(leave: any, Id: number) {
    debugger;
    //console.log(leave);
    return this.http.post(
      `${Config.getControllerUrl("Attandance", "SaveLeave")}`,
      leave
    );
  }

  /* public getTotalLeaves(ProfileId: number) {
    return this.http.get(`${Config.getControllerUrl('Attandance', 'GetTotalLeaves')}/${ProfileId}`);
  }
 */
public searchHealthFacilities(query: string) {
  return this.http.post(
    `${Config.getControllerUrl("Attandance", "SearchHealthFacilities")}`,
    { Query: query }
  );
}
  public getTotalLeaves(Cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Attandance', 'GetTotalLeaves')}/${Cnic}`);
  }

  public getTotalLeaves1(PId: number) {
    return this.http.get(`${Config.getControllerUrl('Attandance', 'GetTotalLeaves1')}/${PId}`);
  }
  
  public getProfilesInActive(skip: number, pageSize: number, hfmisCode: string, searchTerm: string): any {
    return this.http
      .post(
        `${Config.getControllerUrl('Attandance', 'GetProfilesInActive')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm}
      );
  }
  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Attandance', 'GetProfileDetail')}/${cnic}/${type}`);
  }
  public getProfiles(skip: number, pageSize: number, hfmisCode: string, searchTerm: string) {
    return this.http
      .post(
        `${Config.getControllerUrl('Attandance', 'GetProfiles')}`,
        { skip: skip, pageSize: pageSize, hfmisCode: hfmisCode, searchTerm: searchTerm}
      );
  }
  public saveProfile(profile: any) {
    return this.http.post(`${Config.getControllerUrl('Attandance', 'SaveProfile')}`, profile);
  }
  public saveEmpAttendance(empAttandance: any) {
    return this.http.post(`${Config.getControllerUrl('Attandance', 'saveEmpAttendance')}`, empAttandance);
  }
  public uploadPhoto(files: any[], HFId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Attandance', 'UploadProfilePhoto')}/${HFId}`, formData);
  }
  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Attandance', 'GetProfile')}/${cnic}`);
  }
  public getDepartments() {
    return this.http.get(
      `${Config.getControllerUrl("Attandance")}/GetDepartments`
    );
  }
  public getSubDepartments() {
    return this.http.get(
      `${Config.getControllerUrl("Attandance")}/GetSubDepartments`
    );
  }
}
