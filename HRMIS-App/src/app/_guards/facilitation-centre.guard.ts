import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class FacilitationCentreGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let currentOfficer: any = this._authService.getCurrentOfficer();

        if (currentOfficer && (currentOfficer.Program == 'Facilitation Centre' || currentOfficer.Program == 'HISDU')) {
            return true;
        } else {
            return true;
            //this._authService.unauthorizedPage();
        }
    }
}