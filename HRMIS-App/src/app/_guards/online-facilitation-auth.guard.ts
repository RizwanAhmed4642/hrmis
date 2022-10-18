import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CookieService } from '../_services/cookie.service';

@Injectable({
    providedIn: 'root'
})
export class OnlineFacilitationAuthGuard implements CanActivate {

    constructor(private router: Router, private _cookieService: CookieService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let cookie: any = this._cookieService.getCookie('ussr');
    
        if (cookie) {
            // logged in so return true
        return true;
        }
        // not logged in so redirect to login page with the return url
        let url = state.url;
        // if (state.url == '/dashboard') {
        //     this.router.navigate(['/account/login']);
        // } else {
        {
            alert('In OnlineFacilitationAuthGuard..!!');
            this.router.navigate(['online-facilitation-center/account'], { queryParams: { returnUrl: state.url } });
        }
        return false;
    }
}