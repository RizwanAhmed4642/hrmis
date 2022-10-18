import { environment } from '../../environments/environment';

export class Config {
    public static getControllerUrl(controllerName: string, actionName?: string) {
        return this.getApiUrl() + '/' + controllerName + (actionName ? '/' + actionName : '');
    }
    public static getApiUrl() {
        return this.getServerUrl() + '/api';
    }
    public static getServerUrl() {
        return environment.production ? '' : 'http://localhost:8913';
    }
    public static getFirebaseConfig() {
        return {
            // apiKey: "AIzaSyCnfQz6KDgzDeztSiSV3Y7k13D5h6tu9Ts",
            apiKey: "AIzaSyDhK39-ZBuptKm4OabOMehNNRYL7DXalgI",
            authDomain: "hrmis-38c28.firebaseapp.com",
            databaseURL: "https://hrmis-38c28.firebaseio.com",
            projectId: "hrmis-38c28",
            storageBucket: "",
            messagingSenderId: "356200191641"
        }
    }
    public static dashifyCNIC(cnic: string) {
        if(!cnic) return;
        return cnic[0] +
            cnic[1] +
            cnic[2] +
            cnic[3] +
            cnic[4] +
            '-' +
            cnic[5] +
            cnic[6] +
            cnic[7] +
            cnic[8] +
            cnic[9] +
            cnic[10] +
            cnic[11] +
            '-' +
            cnic[12];
    }
}