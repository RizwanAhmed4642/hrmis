import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApplicationMaster, ApplicationLog } from '../application-fts/application-fts';
import { Config } from '../../_helpers/config.class';
@Injectable()
export class CEOApplicationService {
    constructor(private http: HttpClient) { }
    public search(query: string) {
        return this.http.post<any>(`${Config.getControllerUrl('Root', 'Search')}`, { Query: query });
    }
    public searchProfile(cnic: string) {
        return this.http.get(`${Config.getControllerUrl('Application', 'SearchProfile')}/${cnic}`);
    }
    public getApplications(skip: number,
        pageSize: number, query: string, sourceId: number,
        officeId: number, pending: boolean,
        typeId?: number, statusId?: number, from?: Date, to?: Date, forwardingOfficer_Id?: number) {
        return this.http.post(`${Config.getControllerUrl('Application', 'GetApplications')}`, {
            Skip: skip,
            PageSize: pageSize,
            officeId: officeId,
            Source_Id: sourceId,
            Query: query,
            Pending: pending,
            Type_Id: typeId,
            Status_Id: statusId,
            From: from ? from.toDateString() : null, To: to ? to.toDateString() : null,
            ForwardingOfficer_Id: forwardingOfficer_Id
        });
    }
    public removeApplication(id: number, tracking: number, applicationLog: any) {
      return this.http.post(`${Config.getControllerUrl('Application', 'RemoveApplication')}/${id}/${tracking}`, applicationLog);
    }
    public submitApplication(application: any) {
        return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplication')}`, application);
    }
    public createApplicationLog(applicationLog: ApplicationLog) {
        return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
    }
    public uploadApplicationAttachments(applicationAttachments: any[], appId: number) {
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
}