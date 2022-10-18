import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class PSOfficerGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let currentOfficer = this._authService.getCurrentOfficer();
        if (currentOfficer) {
            return true;
        } else {
            return true;
            //this._authService.unauthorizedPage();
        }
        return false;
    }
}