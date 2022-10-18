import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../_helpers/config.class';
import { ApplicationLog, FileMoveMaster, ApplicationAttachment } from '../modules/application-fts/application-fts';
import { VPHolder } from '../modules/vacancy-position/vacancy-position.class';

@Injectable()
export class FileTrackingSystemService {

  constructor(private http: HttpClient) { }

  // Service function to get file master..

  public getFileMaster(query: string, skip: number, pageSize: number) {
    return this.http.post(
      `${Config.getControllerUrl("Profile", "GetFileMaster")}`,
      { Skip: skip, PageSize: pageSize, Query: query }
    );
  }

  public getApplication(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplication')}/${id}/${tracking}`);
  }
  public getApplicationLogs(id: number, lastId: number, orderAsc: boolean) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationLog')}/${id}/${lastId}/${orderAsc}`);
  }
  public getApplicationData(id: number, type: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationData')}/${id}/${type}`);
  }
  public getApplications(skip: number,
    pageSize: number, query: string,
    officeId: number, pending: boolean,
    typeId?: number, statusId?: number,
    field?: string, fieldValue?: number, from?: Date, to?: Date
  ) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetInboxApplications')}`, {
      Skip: skip,
      PageSize: pageSize,
      officeId: officeId,
      Query: query,
      Pending: pending,
      Type_Id: typeId,
      Status_Id: statusId,
      Field: field,
      FieldValue: fieldValue,
      From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
    });
  }
  public saveVacancyHolder(vpHolder: VPHolder) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'HoldVacancy')}`, vpHolder);
  }
  public getSentApplications(skip: number, pageSize: number, query: string, officeId: number, typeId?: number, statusId?: number, from?: Date, to?: Date) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetSentApplications')}`, {
      Skip: skip, PageSize: pageSize, officeId: officeId, Query: query, Type_Id: typeId, Status_Id: statusId,
      From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
    });
  }
  public getSummaries(skip: number, pageSize: number, query: string, officeId: number, typeId?: number, statusId?: number, from?: Date, to?: Date) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetSummaries')}`, {
      Skip: skip, PageSize: pageSize, officeId: officeId, Query: query, Type_Id: typeId, Status_Id: statusId,
      From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
    });
  }
  public getScannedDocuments(skip: number, pageSize: number, query: string, officeId: number, typeId?: number, statusId?: number, from?: Date, to?: Date) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetScannedDocuments')}`, {
      Skip: skip, PageSize: pageSize, officeId: officeId, Query: query, Type_Id: typeId, Status_Id: statusId,
      From: from ? from.toDateString() : null, To: to ? to.toDateString() : null
    });
  }
  public getPetitioners(query: string) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetPetitioners')}`, {
      Query: query
    });
  }
  public addLawFile(obj: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'AddLawFile')}`, obj);
  }
  public submitCallsList(obj: any) {
    return this.http.post(`${Config.getControllerUrl('LawWing', 'SubmitCallsList')}`, obj);
  }
  public getCallsList() {
    return this.http.get(`${Config.getControllerUrl('LawWing', 'GetCallsList')}`);
  }
  public getCauseList(obj: any) {
    return this.http.post(`${Config.getControllerUrl('LawWing', 'GetCauseList')}`, obj);
  }
  public removeCauseList(id: number) {
    return this.http.get(`${Config.getControllerUrl('LawWing', 'RemoveCauseList')}/${id}`);
  }
  public getLawFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetLawFileAttachments')}/${fileId}`);
  }
  public getLawFilePetitioners(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetLawFilePetitioners')}/${fileId}`);
  }
  public getLawFileRespondants(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetLawFileRespondants')}/${fileId}`);
  }
  public getLawFileOfficers(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetLawFileOfficers')}/${fileId}`);
  }
  public getFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetFileAttachments')}/${fileId}`);
  }
  public removeFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveFileAttachments')}/${fileId}`);
  }
  public removeLawFileAttachments(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveLawFileAttachments')}/${fileId}`);
  }
  public removeLawFile(fileId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveLawFile')}/${fileId}`);
  }
  public getRepresentatives(query: string) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetRepresentatives')}`, {
      Query: query
    });
  }
  public getJudges(query: string) {
    return this.http.post(`${Config.getControllerUrl('Root', 'GetJudges')}`, {
      Query: query
    });
  }
  public getOffices(isPending: boolean) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetInboxOffices')}/${isPending}`);
  }
  getVpMaster(obj) {
    return this.http.post(`${Config.getControllerUrl('Vacancy', 'GetVacancyMaster')}`, obj);
  }
  public createApplicationLog(applicationLog: ApplicationLog) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public changeFileNumberOfApplication(application: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'ChangeFileNumberOfApplication')}`, application);
  }
  public submitFileMovement(fileMoveMaster: FileMoveMaster) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'SubmitFileMovement')}`, fileMoveMaster);
  }
  public generateFileRequest(applicationLog: ApplicationLog, file_Id: number) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GenerateFileRequest')}/${file_Id}`, applicationLog);
  }
  public getFileRequest() {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetFileRequestsStatus')}`);
  }
  public getRequisitions(skip: number, pageSize: number, query: string, statusId: number) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFileRequests')}`, { Skip: skip, PageSize: pageSize, StatusId: statusId, Query: query });
  }
  public getMyRequisitions(skip: number, pageSize: number, query: string, statusId: number) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetMyRequisitions')}`, { Skip: skip, PageSize: pageSize, StatusId: statusId, Query: query });
  }
  public removeFileRequest(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'RemoveFileRequest')}/${Id}`);
  }
  public fileAlreadyIssued(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'FileAlreadyIssued')}/${Id}`);
  }
  public issueReturnFile(reqIds: number[], officer_Id: number, toStatusId: number) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'IssueReturnFile')}/${officer_Id}/${toStatusId}`, reqIds);
  }
  public saveApplicationFileReqLog(obj: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'SaveApplicationFileReqLog')}`, obj);
  }
  public getApplicationFileReqLogs(afrId: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetApplicationFileReqLogs')}/${afrId}`);
  }
  public getOfficersForLetter(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetOfficersForLetter')}/${id}/${tracking}`);
  }
  public getApplicationForLetter(id: number, tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplicationForLetter')}/${id}/${tracking}`);
  }
  public searchProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'SearchProfile')}/${cnic}`);
  }
  public fingerPrintCompare(metaData) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'FPrintCompare')}`, metaData);
  }
  public addFileACR(filesUpdated: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'AddFile')}`, filesUpdated);
  }
  public getDDsBarcode(id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetDDsBarcode')}/${id}`);
  }
  public checkVacancy(hfid: number, hfmisCode: string, designationId: number) {
    if (!hfmisCode) return;
    return this.http.get(`${Config.getControllerUrl('Application', 'CheckVacancy')}/${hfid}/${hfmisCode}/${designationId}`);
  }
  public getFileMovements(obj: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetFileMovements')}`, obj);
  }
  public getDiaries(start: any, end: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetDiaries')}`, { from: start, to: end });
  }
  public getDiary(start: any, end: any) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetDiary')}`, { from: start, to: end });
  }
  public getLawFile(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetLawFile')}/${Id}`);
  }
  public ddsSouth(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'DDSSouth')}/${Id}`);
  }
  public ddsPSHD(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'DDSPSHD')}/${Id}`);
  }
  public hideDuplicationFile(Id: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'HideDuplicationFile')}/${Id}`);
  }
  public getLawFiles(obj) {
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'GetLawFiles')}`, obj);
  }
  public getFileMoveMaster(mid: number) {
    return this.http.get(`${Config.getControllerUrl('FilesACRs', 'GetFileMoveMaster')}/${mid}`);
  }
  public removeApplication(id: number, tracking: number, applicationLog: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'RemoveApplication')}/${id}/${tracking}`, applicationLog);
  }
  public removeApplicationAttachment(id: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'RemoveApplicationAttachment')}/${id}`);
  }
  public uploadSignedApplication(signedApplication: ApplicationAttachment, appId: number) {
    const formData = new FormData();
    for (let key in signedApplication.files) {
      if (signedApplication.files.hasOwnProperty(key)) {
        let element = signedApplication.files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Application', 'UploadSignedApplication')}/${appId}`, formData);
  }
  public uploadApplicationAttachments(applicationAttachments: ApplicationAttachment[], appId: number) {
    const formData = new FormData();
    applicationAttachments.forEach(attachment => {
      for (let key in attachment.files) {
        if (attachment.files.hasOwnProperty(key)) {
          let element = attachment.files[key];
          formData.append('file_' + attachment.Document_Id, element);
        }
      }
    });
    return this.http.post(`${Config.getControllerUrl('Application', 'UploadApplicationAttachments')}/${appId}`, formData);
  }
  public uploadFileAttachment(files: any[], ddsId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'UploadFileAttachment')}/${ddsId}`, formData);
  }
  public uploadLawFile(files: any[], fileId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('FilesACRs', 'UploadLawFile')}/${fileId}`, formData);
  }
}
