import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ApplicationMaster, ApplicationLog } from '../application-fts/application-fts';
import { Config } from '../../_helpers/config.class';
@Injectable()
export class HrPostingService {
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
    public removeApplication(id: number, tracking: number) {
        return this.http.get(`${Config.getControllerUrl('Application', 'RemoveApplication')}/${id}/${tracking}`);
    }
    public submitApplicationAttachments(applicationAttachments: any[]) {
        return this.http.post(`${Config.getControllerUrl('Application', 'SubmitApplicationAttachments')}`, applicationAttachments);
    }
    public submit(hrPosting: any) {
        return this.http.post(`${Config.getControllerUrl('Root', 'SaveHrProsting')}`, hrPosting);
    }
    public getHrProsting(filter: any) {
        return this.http.post(`${Config.getControllerUrl('Root', 'GetHrProsting')}`, filter);
    }
    public removeHrProsting(id: number) {
        return this.http.get(`${Config.getControllerUrl('Root', 'RemoveHrProsting')}/${id}`);
    }
    public createApplicationLog(applicationLog: ApplicationLog) {
        return this.http.post(`${Config.getControllerUrl('Application', 'CreateApplicationLog')}`, applicationLog);
    }
}