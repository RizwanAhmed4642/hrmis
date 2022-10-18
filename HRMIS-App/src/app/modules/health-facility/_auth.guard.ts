import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

@Injectable()
export class HealthFacilityAuthGuard implements CanActivate {

    constructor(private router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let url = state.url;
        // logged in so return true
        return true;
        // not logged in so redirect to login page with the return url
        /*  let url = state.url;
         if (state.url == '/dashboard') {
             this.router.navigate(['/account/login']);
         } else {
             this.router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
         } */
        return false;
    }
}