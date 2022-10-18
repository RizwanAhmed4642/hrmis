import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { CookieService } from '../_services/cookie.service';

@Injectable({
    providedIn: 'root'
})
export class AuthPHFMCGuard implements CanActivate {

    constructor(private router: Router, private _cookieService: CookieService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let cookie: any = this._cookieService.getCookie('ussrphfmc');

        if (cookie) {
            // logged in so return true
            return true;
        }
        // not logged in so redirect to login page with the return url
        this.router.navigate(['/job/account']);
        return false;
    }
}