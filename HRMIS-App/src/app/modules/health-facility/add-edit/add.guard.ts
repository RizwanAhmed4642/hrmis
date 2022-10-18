import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../../../_services/authentication.service';

@Injectable()
export class HealthFacilityAddAuthGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
            if (this._authService._SDP()) {
            return true;
        } else {
            this._authService.logout();
            return false;
        }

    }
}