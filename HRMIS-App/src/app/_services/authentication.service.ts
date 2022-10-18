import { Injectable, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { CookieService } from './cookie.service';
import { LocalService } from './local.service';
import { UserNav, Nav } from '../_models/nav.class';
import { RootService } from './root.service';
import { User } from '../_models/user.class';
import { PandSOfficerView } from '../modules/user/user-claims.class';

@Injectable({
    providedIn: 'root'
})
export class AuthenticationService {
    public userName = '';
    public roleName = '';
    public hfmisCode = '';
    public userLevelNameEmitter = new EventEmitter<string>();
    public currentOfficerEmitter = new EventEmitter<PandSOfficerView>();
    public userLevelNameEmitted: boolean = false;
    public userIPEmitter = new EventEmitter<string>();
    constructor(private _rootService: RootService, private router: Router, private _localService: LocalService, private _cookieService: CookieService) { }
    public getUser() {
        return this._localService.get('ussr');
    }
    public setUser(_: any): boolean {
        let user = JSON.parse(_.user),
            nav = _.nav,
            token = _.access_token,
            remember = _.remember;
        if (token !== '') {
            this._cookieService.deleteAndSetCookie('ussr', token);
            user.HfmisCode = (user.HfmisCode ?
                user.HfmisCode : user.TehsilCode ?
                    user.TehsilCode : user.DistrictCode ?
                        user.DistrictCode : user.DivisionCode ?
                            user.DivisionCode : '0');
            if (remember == true) {
                this._cookieService.deleteAndSetCookie('ussrR', user.Id);
            }
            console.log(this._cookieService.getCookie('ussrR'));
            this.userName = user.UserName.toLowerCase();
            this.roleName = user.RoleName.toLowerCase();
            this.hfmisCode = user.HfmisCode;

            this._localService.set('ussr', user);
            this._localService.set('ussrNvd', nav);
            this.setCurrentOfficer(user.HfmisCode == '0');
            this.getSessionInfo();
            return true;
        }
        else {
            this.logout();
            return false;
        }
    }
    public getSessionInfo() {
        this._rootService.getSessionInfo().subscribe((x) => {
            if (x.ip) {
                this.saveIP(x.ip);
                this.userIPEmitter.emit(x.ip);
            } else {
                console.log('ip');
            }
        }, err => {
            console.log('ip');
        });
    }
    public saveIP(ip) {
        this._rootService.saveIP(ip).subscribe((res) => {
        });
    }
    // public saveIP(ip) {
    //     this._rootService.saveIP(ip).subscribe((res) => {
    //     });
    // }
    public setCurrentOfficer(proceed: boolean) {
        if (proceed) {
            this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
                if (res) {
                    this.currentOfficerEmitter.emit(res);
                    this.userLevelNameEmitter.emit(res.DesignationName);
                    this.userLevelNameEmitted = true;
                    this._localService.set('crpso', res);
                } else {
                    this.getUserLevelName();
                }
            });
        } else {
            this.getUserLevelName();
        }
    }
    public getCurrentOfficer() {
        return this._localService.get('crpso');
    }
    public getNav() {
        let realNav: Nav[] = [];
        let serverNav: any = this._localService.get('ussrNvd');
        let parsedNav: any[] = JSON.parse(serverNav);
        parsedNav.forEach(n => {
            let nav: Nav = new Nav();
            nav.name = n.Name;
            nav.url = '/' + n.Url;
            nav.icon = n.Icon;
            realNav.push(nav);
        });
        return realNav;
    }
    unauthorizedPage() {
        /* this._cookieService.deleteCookie('ussr');
        this._localService.set('ussr', null);
        this._localService.set('ussrNvd', null); */
        this.router.navigate(['/401']);
    }
    logout() {
        this._cookieService.deleteCookie('ussr');
        this._localService.set('ussr', null);
        this._localService.set('ussrNvd', null);
        this._localService.set('crpso', null);
        this.router.navigate(['/account/login']);
    }
    logoutApplicant() {
        this._cookieService.deleteCookie('ussr');
        this._localService.set('ussr', null);
        this._localService.set('ussrNvd', null);
        this._localService.set('crpso', null);
        this.router.navigate(['/retirement-application/account']);
    }
    public getUserHfmisCode() {
        let user = this.getUser();
        return user ? user.HfmisCode : '00000';
    }
    public getUserLevel() {
        let hfmisCode: string = this.getUserHfmisCode();

        return hfmisCode.length == 1 ? 'Province' : hfmisCode.length == 3 ? 'Division' :
            hfmisCode.length == 6
                ? 'District' : hfmisCode.length == 9 ? 'Tehsil' : '';
    }
    public getUserLevelName() {
        let user = this.getUser();
        if (!user) return;
        let hfmisCode: string = user.HfmisCode;
        switch (hfmisCode.length) {
            case 1:
                this.userLevelNameEmitter.emit('Punjab Province');
                break;
            case 3:
                this._rootService.getDivisions(hfmisCode).subscribe((data: any) => {
                    if (data && data.length > 0) {
                        this.userLevelNameEmitter.emit(data[0].Name + ' Division');
                    }
                }, (err) => console.log(err));
                break;
            case 6:
                this._rootService.getDistricts(hfmisCode).subscribe((data: any) => {
                    if (data && data.length > 0) {
                        this.userLevelNameEmitter.emit(data[0].Name + ' District');
                    }
                }, (err) => console.log(err));
                break;
            case 9:
                this._rootService.getTehsils(hfmisCode).subscribe((data: any) => {
                    if (data && data.length > 0) {
                        this.userLevelNameEmitter.emit(data[0].Name + ' Tehsil');
                    }
                }, (err) => console.log(err));
                break;
            case 19:
                this._rootService.getHealthFacilities(hfmisCode).subscribe((data: any) => {
                    if (data && data.length > 0) {
                        this.userLevelNameEmitter.emit(data[0].Name);
                    }
                }, (err) => console.log(err));
                break;
            default:
                break;
        }
    }
    public setUserDisplay(proceed: boolean) {
        this.setCurrentOfficer(proceed);
        if (proceed) {
            this._rootService.getSessionInfo().subscribe((x) => {
                if (x.ip) {
                    this.userIPEmitter.emit(x.ip);
                } else {
                    console.log('ip');
                }
            }, err => {
                console.log('ip');
            });
        }
    }
    public _ADMIN = () => {
        return this.userName == 'dpd' && this.hfmisCode.length == 1 && this.roleName == 'Administrator' ? true : false;
    }
    public _SDP = () => {
        return this.userName.startsWith('sdp') ? true : false;
    }
    public _COMPUTER_OPERATOR = () => {
        return this.userName.startsWith('sdp') && this.hfmisCode.length == 3 && this.roleName == 'Administrator' ? true : false;
    }
    public _CEO = () => {
        return this.userName.startsWith('sdp') && this.hfmisCode.length == 3 && this.roleName == 'Administrator' ? true : false;
    }
    public _FDO = () => {
        return this.userName.startsWith('sdp') && this.hfmisCode.length == 3 && this.roleName == 'Administrator' ? true : false;
    }
}