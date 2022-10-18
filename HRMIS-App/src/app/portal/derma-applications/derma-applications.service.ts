import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Config } from '../../_helpers/config.class';

@Injectable({
  providedIn: 'root'
})
export class DermaApplicationsService {

  constructor(private http: HttpClient) { }

  public saveDermaApplication(dermaApp :any) {
    return this.http.post(`${Config.getControllerUrl('Root', 'SaveDermaApplication')}`, dermaApp);
  }
}
