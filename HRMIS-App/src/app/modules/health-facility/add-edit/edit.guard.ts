import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthenticationService } from '../../../_services/authentication.service';

@Injectable()
export class HealthFacilityEditAuthGuard implements CanActivate {

    constructor(private router: Router, private _authService: AuthenticationService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        let hfmisCode: string = state.url.split('/')[2];
        if (hfmisCode && hfmisCode.length == 19) {
            let hfTypeCode: string = hfmisCode[12] + hfmisCode[13] + hfmisCode[14];
            if (this._authService.getUser().UserName.toLowerCase().startsWith('ceo.') && (hfTypeCode == '011' || hfTypeCode == '012')) {
                this._authService.logout();
                return false;
            }
        } else {
            this._authService.logout();
        }
        return true;
    }
}