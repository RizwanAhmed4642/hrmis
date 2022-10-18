import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { SortDescriptor, orderBy, aggregateBy } from '@progress/kendo-data-query';
import { GridDataResult, PageChangeEvent } from '@progress/kendo-angular-grid';
import { VpMProfileView, VPDetail, VpDProfileView } from '../../../../vacancy-position/vacancy-position.class';
import { Subject } from 'rxjs/Subject';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../../../../_services/root.service';
import { AuthenticationService } from '../../../../../_services/authentication.service';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { NotificationService } from '../../../../../_services/notification.service';
import { DropDownsHR } from '../../../../../_helpers/dropdowns.class';
import { KGridHelper } from '../../../../../_helpers/k-grid.class';
import { ProfileService } from '../../../profile.service';

@Component({
  selector: 'app-profile-inquiry',
  templateUrl: './inquiry.component.html',
  styles: []
})
export class InquiryComponent implements OnInit, OnDestroy {
  @Input() public profile: any;
  public kGrid: KGridHelper = new KGridHelper();
  public inquiry: any = {};
  public newPenalties: any[] = [];
  public penaltyTypes: any[] = [];
  public penalties: any[] = [];
  public majorPenalties: any[] = [];
  public minorPenalties: any[] = [];
  public retiredPenalties: any[] = [];
  public dropDowns: DropDownsHR = new DropDownsHR();
  public loading: boolean = false;
  public newInquiry: boolean = false;
  public savingInquiry: boolean = false;
  public showInquiryDetail: boolean = false;
  public years: any[] = [1, 2, 3, 4, 5];
  public currentUser: any;
  constructor(private _rootService: RootService, private _profileService: ProfileService, private _notificationService: NotificationService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    this.getInquiryStatus();
    this.getPenaltyType();
    if (this.profile && this.profile.Id) {
      this.getInquiries();
    }
  }
  public saveInquiry() {
    this.savingInquiry = true;
    this.setNewPenalties();
    this.inquiry.Profile_Id = this.profile.Id;
    this._profileService.saveProfileInquiry({ hrInquiry: this.inquiry, hrInquiryPenalties: this.newPenalties }).subscribe((res) => {
      if (res) {
        this.savingInquiry = false;
        this.inquiry = {};
        this.newPenalties = [];
        this.newInquiry = false;
        this._notificationService.notify('success', 'Inquiry Saved!');
      }
    }, err => {
      this.handleError(err);
    })
  }
  private getInquiryStatus() {
    this._rootService.getInquiryStatus().subscribe((res: any) => {
      if (res) {
        this.dropDowns.inquiryStatuses = res;
      }
    }, err => {
      this.handleError(err);
    });
  }
  private getPenaltyType() {
    this._rootService.getPenaltyType().subscribe((res: any) => {
      if (res) {
        this.penaltyTypes = res;
        this.setPenalties();
      }
    }, err => {
      this.handleError(err);
    });
  }
  private setPenalties() {
    this.majorPenalties = this.penaltyTypes.filter(x => x.Class_Id == 2);
    this.minorPenalties = this.penaltyTypes.filter(x => x.Class_Id == 1);
    this.retiredPenalties = this.penaltyTypes.filter(x => x.Class_Id == 3);
  }
  private setNewPenalties() {
    this.newPenalties = [];
    this.minorPenalties.forEach(element => {
      if (element.checked) {
        this.addPenalty('Minor', element);
      }
    });
    this.majorPenalties.forEach(element => {
      if (element.checked) {
        this.addPenalty('Major', element);
      }
    });
    this.retiredPenalties.forEach(element => {
      if (element.checked) {
        this.addPenalty('Retirement', element);
      }
    });
  }
  private addPenalty(penaltyTypeName: string, penaltyType: any) {
    let inquiryPenalty: any = {};
    inquiryPenalty.PenaltyType = penaltyTypeName;
    inquiryPenalty.PenaltyType_Id = penaltyType.Id;
    inquiryPenalty.DurationFrom = penaltyType.DurationFrom;
    inquiryPenalty.DurationTo = penaltyType.DurationTo;
    inquiryPenalty.Scale = penaltyType.Scale;
    inquiryPenalty.Amount = penaltyType.Amount;
    inquiryPenalty.OtherAmountSource = penaltyType.OtherAmountSource;
    inquiryPenalty.Remarks = penaltyType.Remarks;
    inquiryPenalty.NumberOfYears = penaltyType.NumberOfYears;
    this.newPenalties.push(inquiryPenalty);
  }
  private getInquiries() {
    this._profileService.getProfileInquiries(this.profile.Id).subscribe((res: any) => {
      if (res) {
        console.log(res);
        this.kGrid.data = res.inquiries;
        this.penalties = res.inquiryPenalties;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public getInquiryPenalties(id) {
    return this.penalties.filter(x => x.EmpInquiry_Id == id);
  }
  private handleError(err: any) {
    this.loading = false;
    this.savingInquiry = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy(): void {
  }
}
