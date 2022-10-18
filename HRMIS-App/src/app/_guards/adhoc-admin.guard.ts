import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class AdhocAdminGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let userName: string = this._authService.getUser().UserName.toLowerCase();
        let roleName: string = this._authService.getUser().RoleName.toLowerCase();
        if (userName && roleName &&
            (roleName == 'adhocscrutiny'
                || userName == 'pshd'
                || userName.startsWith('sdp')
                || roleName=='Senior Data Processor'
                || userName == 'dpd')) {
            return true;
        } else {
            this._authService.unauthorizedPage();
        }
        return false;
    }
}