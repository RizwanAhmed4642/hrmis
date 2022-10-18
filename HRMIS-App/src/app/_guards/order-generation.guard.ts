import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';

@Injectable({
    providedIn: 'root'
})
export class OrderGenerationGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let userName: string = this._authService.getUser().UserName.toLowerCase();
        let roleName: string = this._authService.getUser().RoleName.toLowerCase();
        if (userName && roleName &&
            (roleName == 'hisdu order team'
                || roleName == 'chief executive officer'
                || roleName == 'Districts'
                || roleName == 'phfmc'
                || roleName == 'dg health'
                || roleName == 'phfmc admin'
                || userName.startsWith('ceo.')
                || userName == 'ordercell'
                || userName == 'south.ordercell'
                || userName == 'og1'
                || userName == 'sc1'
                || userName == 'a.system'
                || userName.startsWith('sdp')
                || userName == 'dpd')) {
            return true;
        } else {
            this._authService.unauthorizedPage();
        }
        return false;
    }
}