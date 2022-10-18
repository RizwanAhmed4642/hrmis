import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Config } from '../../_helpers/config.class';
import { ApplicationMaster, ApplicationAttachment, ApplicationLog } from '../application-fts/application-fts';

@Injectable()
export class OrderService {

  constructor(private http: HttpClient) { }
  public getOrders(query: any, district: string, transferTypeId: number, currentPage: number, itemsPerPage: number) {
    return this.http.post(`${Config.getControllerUrl('OrderNotification', 'GetESRReport')}/${district}/${transferTypeId}/${currentPage}/${itemsPerPage}`, query);
  }
  public getOrder(id: number, type: number) {
    return this.http.get(`${Config.getControllerUrl('OrderNotification', 'GetESR')}/${id}/${type}`);
  }
  public removeOrder(id: number, type: number) {
    return this.http.get(`${Config.getControllerUrl('OrderNotification', type == 5 ? 'RemoveLeaveorder' : 'RemoveESR')}/${id}`);
  }
  public saveProfile(profile: any) {
    return this.http.post(`${Config.getControllerUrl('OrderNotification', 'SaveOrderProfile')}`, profile);
  }
  public submitCombineOrder(obj) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitCombineOrder')}`, obj);
  }
  public getApplications(skip: number, pageSize: number, query: string, typeId: number, statusId: number) {
    return this.http.post(`${Config.getControllerUrl('Application', 'GetApplications')}`, { Skip: skip, PageSize: pageSize, Type_Id: typeId, Status_Id: statusId, Query: query });
  }
  public getOrderRequests(skip: number, pageSize: number) {
    return this.http
      .post(
        `${Config.getControllerUrl('OrderNotification', 'GetOrderRequests')}`,
        { skip: skip, pageSize: pageSize }
      );
  }
  public getOfficerStampId() {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetOfficerStampId')}`);
  }
  public approveOrderRequest(id: number) {
    return this.http.get(`${Config.getControllerUrl('OrderNotification', 'ApproveOrderRequest')}/${id}`);
  }
  public editOrderRequest(orderId: number, orderRequestId: number) {
    return this.http.get(`${Config.getControllerUrl('OrderNotification', 'EditOrderRequest')}/${orderId}/${orderRequestId}`);
  }
  public getProfileDetail(cnic: string, type: number) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetProfileDetail')}/${cnic}/${type}`);
  }
  public submitApplication(application: ApplicationMaster) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitOrderApplication')}`, application);
  }
  public submitMutualOrder(obj: any) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitMutualOrder')}`, obj);
  }
  UpdateOrderHTML(id: number, OrderHTML: string) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateOrderHTML')}`, { id: id, OrderHTML: OrderHTML });
  }
  UpdateLeaveOrderHTML(leaveOrder) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateLeaveOrderHTML')}`, leaveOrder);
  }
  public createApplicationLog(applicationLog: ApplicationLog) {
    return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
  }
  public uploadOrder(esrSigned: any) {
    return this.http.post(`${Config.getControllerUrl('OrderNotification', 'UploadSignedOrder')}`, esrSigned);
  }
  public generateQRManual(id: number, type: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GenerateQRManual')}/${id}/${type}`);
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
  public uploadSignedOrder(signedOrder: any, esrId: number, elrId: number, appId: number) {
    const formData = new FormData();
    for (let key in signedOrder.files) {
      if (signedOrder.files.hasOwnProperty(key)) {
        let element = signedOrder.files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Application', 'UploadSignedOrder')}/${appId}/${esrId}/${elrId}`, formData);
  }
  public getOrderDesignations(hf_Id: number, hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetOrderDesignations')}/${hf_Id}/${hfmisCode}`);
  }
  public changeSystemWithOrder(esrId: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'ChangeSystemWithOrder')}/${esrId}`);
  }
  public submitFileMovement(applications: ApplicationMaster[]) {
    return this.http.post(`${Config.getControllerUrl('Application', 'SubmitFileMovement')}`, applications);
  }
  public searchProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'SearchProfile')}/${cnic}`);
  }
  public getApplication(appId: number, trackingId: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GetApplication')}/${appId}/${trackingId}`);
  }
  public getProfileForOrder(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Profile', 'GetAnyProfile')}/${cnic}`);
  }
  public generateOrderBars(tracking: number) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GenerateOrderBars')}/${tracking}`);
  }
  public generateOrderQr(desc: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GenerateOrderQr')}/${desc}`);
  }
  public checkVacancy(hfid: number, hfmisCode: string, designationId: number) {
    if (!hfmisCode) return;
    return this.http.get(`${Config.getControllerUrl('Application', 'CheckVacancy')}/${hfid}/${hfmisCode}/${designationId}`);
  }

  public uploadReqeustedSignedCopy(files: any[], orderId: number, orderRequestId: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('OrderNotification', 'UploadReqeustedSignedCopy')}/${orderId}/${orderRequestId}`, formData);
  }
}
