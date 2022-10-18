import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../_services/authentication.service';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class RiBranchGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let userName: string = this._authService.getUser().UserName.toLowerCase();
        let roleName: string = this._authService.getUser().RoleName;
        if (userName && (roleName == 'RI Branch' || userName == 'dpd' || roleName == 'Administrative Office' || roleName == 'Deputy Secretary' || roleName == 'Law wing' || userName == 'pshd')) {
            return true;
        } else {
            this._authService.unauthorizedPage();
        }
        return false;
    }
}