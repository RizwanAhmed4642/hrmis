import { Injectable, EventEmitter } from '@angular/core';
import { Notification } from '../_models/notification.class';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { ApplicationMaster } from '../modules/application-fts/application-fts';
@Injectable({
    providedIn: 'root'
})
export class FirebaseHisduService {
  /*   private appsRealtime: any[] = [];
    private appsChanged: Subject<any[]>;
    constructor(private firestore: AngularFirestore) {
        this.subscribeToFirebase();
    }
    update(html) {
        localStorage.setItem('liveP', html);
    }
    updateTokenNumber(tokenNo) {
        localStorage.setItem('tokenNo', tokenNo);
    }
    getLivePreview() {
        return localStorage.getItem('liveP');
    }
    getTokenNumber() {
        return localStorage.getItem('tokenNo');
    }
    getAppData() {
        return this.firestore.collection('hisauth').snapshotChanges();
    }
    createAppData(appData: any) {
        return this.firestore.collection('hisauth').add(appData);
    }
    updateAppData(appData: any) {
        this.firestore.doc('hisauth/' + appData.id).update(appData);
    }
    deleteAppData(appDataId: string) {
        this.firestore.doc('hisauth/' + appDataId).delete();
    }
    getApplicationPreview({ uid }: any) {
        if (uid) {
            return this.firestore.collection('FacilitationCentre').doc(uid).collection('application').snapshotChanges();
        }
    }
    createApplicationPreview(application: any, metaInfo: any) {
        application.metaInfo = metaInfo;
        application.id = application.metaInfo.uid;
        return this.firestore.doc('FacilitationCentre/' + application.id + '/application/' + application.id).update(application);
    }
    updateApplicationPreview(application: any, metaInfo: any) {
        application.metaInfo = metaInfo;
        if (application.metaInfo.uid) {
            return this.firestore.doc('FacilitationCentre/' + application.metaInfo.uid + '/application/' + application.metaInfo.uid).update(application);
        }
    }
    createApplication(trackingNo: any, timeLast: any) {
        return this.firestore.collection('ApplicationMaster').add({ trackingNo, timeLast });
    }
    private subscribeToFirebase() { */
       /*  this.appsChanged = new Subject<any[]>();
        this.trackApplictionsRealtime().subscribe(data => {
            this.appsRealtime = data.map(e => {
                return {
                    id: e.payload.doc.id,
                    ...e.payload.doc.data()
                };
            });
            this.appsChanged.next(this.appsRealtime);
        }); */
  /*   }
    updateApplication(id: string, trackingNo: any, timeLast: any) {
        return this.firestore.doc('ApplicationMaster/' + id).update({ trackingNo, timeLast });
    }
    public updateApplicationFirebase(trackingNo: number) {
        let realtime = this.appsRealtime.find(x => x.trackingNo == trackingNo);
        if (realtime) {
            this.updateApplication(realtime.id, trackingNo, new Date());
        }
    }
    trackApplictionsRealtime() {
        return this.firestore.collection('ApplicationMaster').snapshotChanges();
    }
    public getAppsChanged(): Observable<any> {
        return this.appsChanged.asObservable();
    } */
}
