import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: "root"
})
export class OnlinePostingService {
  constructor(private http: HttpClient) { }

  public getPassword(info: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddUser`, info);
  }
  public login(user) {
    return this.http.post(`${Config.getServerUrl()}/Token`, 'username=' + user.username + '&password=' + user.password + '&grant_type=password');
  }
  public getMerits(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetMeritProfile')}/${cnic}`);
  }
  public meritOrder(merit_Id: number, html: string) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'MeritOrder')}/${merit_Id}`, { OrderHTML: html });
  }
  public lockMeritOrder(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockMeritOrder')}`);
  }
  public isPGInit(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'isPGInit')}/${merit_Id}`);
  }
  public lockMeritOrderSingle(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockMeritOrderSingle')}/${merit_Id}`);
  }
  public postCandidate(CNIC: string, HF_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'PostCandidate')}/${CNIC}/${HF_Id}`);
  }
  public postCandidateHF(CNIC: string, HF_Id: number, HFMISCode: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'PostCandidate')}/${CNIC}/${HF_Id}/${HFMISCode}`);
  }
  public lockAwaitingPostingOrder(merit_Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockAwaitingPostingOrder')}/${merit_Id}`);
  }
  public generateOrderQr(desc: string) {
    return this.http.get(`${Config.getControllerUrl('Application', 'GenerateOrderQr')}/${desc}`);
  }
  public lockMerits() {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'LockMerits')}`);
  }
  public isPGT(merit_Id: number, isPGT: boolean, fromDate: any, hfId: number, sepcId: number, hfName, fd: any) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'isPG')}?meritId=${merit_Id}&isOnPGT=${isPGT}&date=${fromDate}&hfId=${hfId}&hfName=${hfName}&sepcsId=${sepcId}`, fd);
  }
  UpdateOrderHTML(id: number, OrderHTML: string) {
    return this.http.post(`${Config.getControllerUrl('ESR', 'UpdateOrderHTML')}`, { id: id, OrderHTML: OrderHTML });
  }
  public getMeritById(id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritById')}/${id}`);
  }
  public saveDiplomaStatus(meritId: number, isAccepted: boolean) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'SaveDiplomaStatus')}/${meritId}/${isAccepted}`);
  }
  public getMeritProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('Public', 'GetMeritProfile')}/${cnic}`);
  }
  public saveProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/AddMeritProfile`, merit);
  }

  public verifyMeritProfile(merit: any) {
    return this.http.post(`${Config.getControllerUrl('Public')}/VerifyMeritProfile`, merit);
  }
  public uploadMeritProfilePhoto(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('Public', 'UploadProfilePhoto')}/${cnic}`, formData);
  }
  public getMeritActiveDesignation(id) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/GetMeritActiveDesignation/${id}`);
  }
  public getDownloadLink(type: string, cnic: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/${type == 'acceptance' ? 'GetDownloadAcceptanceLetterLink' : 'GetDownloadOfferLink'}/${cnic}`);
  }
  public getDownloadLinkById(Id: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp')}/GetDownloadOfferLinkById/${Id}`);
  }
  public getPreferences(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesList')}/${MeritID}`);
  }
  public getPreferencesList(CNIC: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetPreferencesList')}/${CNIC}`);
  }
  public getConsultantPreferences(CNIC: string) {
    return this.http.get(`${Config.getControllerUrl('Main', 'GetPreferencesList')}/${CNIC}`);
  }
  public getPgPreferences(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPgPreferences')}/${MeritID}`);
  }
  public getPreferencesVacancyColor(HfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesVacancyColor')}/${HfmisCode}`);
  }
  public getMeritPosting(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritPosting')}/${MeritID}`);
  }
  public getAwaitingPosting(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetAwaitingPosting')}/${MeritID}`);
  }
  public getHrPosting(CNIC: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetHrPosting')}/${CNIC}`);
  }
  public getPostedMerits(HfmisCode: string, desigId: number, meritsActiveDesignationId: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPostedMerits')}/${HfmisCode}/${desigId}/${meritsActiveDesignationId}`);
  }
  public getPreferencesAll(MeritID: number) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetMeritPreferences')}/${MeritID}`);
  }
  public getPreferenceVacancy(hfmisCode: string) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'GetPreferencesVacancy')}/${hfmisCode}`);
  }
  public addPreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'SavePreferences')}/${obj.MeritId}/${obj.hfmisCode}/${obj.hfId}`);
  }
  public saveAllPreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'SavePreferencesFinal')}/${obj.MeritId}`);
  }/* 
  public removePreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'RemovePreference')}/${obj.MeritId}/${obj.hfmisCode}`);
  } */
  public removePreferences(obj) {
    return this.http.get(`${Config.getControllerUrl('JobApp', 'RemovePreference')}/${obj.HfmisCode}/${obj.MeritId}`);
  }
  public getHFOpened(DesignationId: number) {
    return this.http.get(`${Config.getControllerUrl('Root', 'GetHFOpened')}/${DesignationId}`);
  }
  public getPostingPlan(obj) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'GetMerits')}`, obj);
  }
  public getMeritsByCNIC(obj) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'GetMeritsByCNIC')}`, obj);
  }
  public getPromotedCandidates(obj) {
    return this.http.post(`${Config.getControllerUrl('Main', 'GetPromotedCandidates')}`, obj);
  }
  public getMeritSummary(obj) {
    return this.http.post(`${Config.getControllerUrl('JobApp', 'GetMeritSummary')}`, obj);
  }
  public uploadSignedAcceptance(files: any[], id: number) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('JobApp', 'UploadAcceptance')}/${id}`, formData);
  }
}
