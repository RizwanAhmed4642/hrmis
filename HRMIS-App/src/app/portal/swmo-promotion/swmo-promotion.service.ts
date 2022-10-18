import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';
import { HttpClient } from '@angular/common/http';


@Injectable({
  providedIn: 'root'
})
export class SwmoPromotionService {

  constructor(private http: HttpClient) { }

  public saveSwmoPromotion(swmoPromotionDTO :any) {
    return this.http.post(`${Config.getControllerUrl('SwmoPromotion', 'SaveSwmoPromotionCandidate')}`, swmoPromotionDTO);
  }
  public getMerit(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('SwmoPromotion', 'GetMerit')}/${cnic}`);
  }
  public getHFLists() {
    return this.http.get(`${Config.getControllerUrl('SwmoPromotion', 'GetHFLists')}`);
  }
  public GetHFListsPreference() {
    return this.http.get(`${Config.getControllerUrl('SwmoPromotion', 'GetHFListsPreference')}`);
  }

  public getProfile(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('SwmoPromotion', 'GetProfile')}/${cnic}`);
  }
  
  public getPreferedHFL(cnic: string) {
    return this.http.get(`${Config.getControllerUrl('SwmoPromotion', 'PreferedHFList')}/${cnic}`);
  }
  
  public getDesignations() {
    return this.http.get(
      `${Config.getControllerUrl("BaseDataHrmis", "GetDesignations")}`
    );
  }
  public uploadMeritProfilePhoto(files: any[], cnic: string) {
    const formData = new FormData();
    for (let key in files) {
      if (files.hasOwnProperty(key)) {
        let element = files[key];
        formData.append('file', element);
      }
    }
    return this.http.post(`${Config.getControllerUrl('MeritDiploma', 'UploadDocumentPhoto')}/${cnic}`, formData);
  }
  public getDistrict() {
    return this.http.get(
      `${Config.getControllerUrl("MeritDiploma", "GetDistrict")}`
    );
  }
}
