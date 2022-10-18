import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class UserModuleGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let userName: string = this._authService.getUser().UserName.toLowerCase();
        let roleName: string = this._authService.getUser().RoleName.toLowerCase();
        if (userName && (userName.startsWith('sdp') || userName == 'dpd' || userName == 'phfmcadmin')) {
            return true;
        } else {
            this._authService.unauthorizedPage();
        }
        return false;
    }
}