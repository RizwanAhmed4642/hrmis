import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RootService } from '../../../_services/root.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { LocalService } from '../../../_services/local.service';
import { VacancyPositionService } from '../vacancy-position.service';

@Component({
  selector: 'vp-quota',
  templateUrl: './quota.component.html',
  styleUrls: ['./quota.component.scss']
})
export class VPQuotaComponent implements OnInit {

  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public report: any = {};
  public quota: any[] = [];
  public selectedReport: any[] = [];
  public selectedReport2: any[] = [];
  public showDesignation: boolean = false;
  public showDistrict: boolean = false;
  public showHF: boolean = false;

  constructor(private route: ActivatedRoute,
    private _rootService: RootService,
    private _cookieService: CookieService,
    private router: Router,
    private _localService: LocalService,
    private _authenticationService: AuthenticationService,
    private _vpService: VacancyPositionService
  ) { }

  ngOnInit() {
    this.loadAll();
  }

  public loadAll() {
    this.report = {};
    this.showDistrict = false;
    this.showDesignation = true;
    this._vpService.getVPQuota(this._authenticationService.getUserHfmisCode())
      .subscribe((x: any) => {
        if (x) {
          this.showDistrict = false;
          this.report.vpQuota = x.vpQuota;
          this.report.vpQuotaDesignation = x.vpQuotaDesignation;
          this.report.vpQuotaDistrict = x.vpQuotaDistrict;
          this.report.vpQuotaDistrictDesignation = x.vpQuotaDistrictDesignation;
          this.selectedReport = this.report.vpQuotaDesignation;
          this.selectedReport2 = this.report.vpQuotaDistrict;
          this.quota = [];
          this._vpService.getVacancyQuota()
            .subscribe((x: any) => {
              if (x) {
                this.quota = x;
                this.selectedReport.forEach(r => {
                  r.quota = this.quota.find(x => x.DesignationId == r.Designation_Id);
                });
                console.log(this.selectedReport);
              }
            });
        }
      });
  }
  public designationClicked(id: number) {
    this.selectedReport = [];
    this.selectedReport = this.report.vpQuotaDistrictDesignation.filter(x => x.Designation_Id == id);

    this.showDesignation = false;
    this.showHF = false;
    this.showDistrict = true;
  }
  public districtClicked(code: string, designationId: number) {
    this.selectedReport = [];
    this.selectedReport = this.report.vpQuota.filter(x => x.DistrictCode == code && x.Designation_Id == designationId);

    this.showDesignation = false;
    this.showDistrict = false;
    this.showHF = true;
  }
  public back() {
    this.selectedReport = this.report.vpQuotaDesignation;
    this.selectedReport2 = this.report.vpQuotaDistrict;

    this.showDesignation = true;
    this.showDistrict = false;
    this.showHF = false;
  }

}
