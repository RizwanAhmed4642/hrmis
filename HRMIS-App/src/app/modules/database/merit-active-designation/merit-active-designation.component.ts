import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DatabaseService } from '../database.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/internal/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { NotificationService } from '../../../_services/notification.service';

@Component({
  selector: 'app-meritactivedesignation',
  templateUrl: './merit-active-designation.component.html',
  styleUrls: ['./merit-active-designation.component.scss']
})
export class MeritactivedesignationComponent implements OnInit {

  private subscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public merit: any = {};
  public meritdesigform : any = {};
  public districts: Array<{ text: string, value: string }>;
  public districtsData: Array<{ text: string, value: string }>;
    //selected object for drop downs
    public selectedFiltersModel: any = {
      division: { Name: 'Select Division', Code: '0' },
      district: { Name: 'Select District', Code: '0' },
      tehsil: { Name: 'Select Tehsil', Code: '0' }
    };

  constructor(private route: ActivatedRoute, private _dbService: DatabaseService,
    private _cookieService: CookieService,
    private router: Router,
    private _rootService: RootService,
    public _notificationService: NotificationService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.getdesignations();
    this.getDistricts();
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          this.merit.Id = +params['id'];
          // this.fetchMeritInfo();
        } else {
          // this.getDesignations();
        }
      }
    )}



    private getdesignations = () => {
      this.dropDowns.designations = [];
      this.dropDowns.designationsData = [];
      this._dbService.getDesignationsforDDl().subscribe((res: any) => {
        this.dropDowns.designations = res;
        this.dropDowns.designationsData = this.dropDowns.designations.slice();
      },
        err => { this.handleError(err); }
      );
    }

    private getDistricts = () => {
      this.districts = [];
      this.districtsData = [];
      this._rootService.getDistricts('0').subscribe((res: any) => {
        this.districts = res;
        this.districtsData = this.districts.slice();
        if (this.districts.length == 1) {
          this.selectedFiltersModel.district = this.districtsData[0];
        }
      },
        err => { this.handleError(err); }
      );
    }

    private handleError(err: any) {
      if (err.status == 403) {
        this._authenticationService.logout();
      }
    }

    public saveMeritActiveDesignation() {
      this._dbService.saveMertiActiveDesignation(this.meritdesigform).subscribe((data) => {
        if (data) {
          this._notificationService.notify(
            "success",
            "Saved Successfuly!"
          );
        }
      });
    }
}
