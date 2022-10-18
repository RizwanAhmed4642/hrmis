import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../../../_services/root.service';
import { AuthenticationService } from '../../../../_services/authentication.service';
import { ApplicationFtsService } from '../../application-fts.service';
import { KGridHelper } from '../../../../_helpers/k-grid.class';
import { SortDescriptor, orderBy } from '@progress/kendo-data-query';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs/Subscription';

@Component({
  selector: 'app-hf-online-transfer',
  templateUrl: './online-transfer.component.html',
  styles: [`
  td{
color: black !important;
  }
  `]
})
export class OnlineTransferComponent implements OnInit, OnDestroy {
  @Input() public hfmisCode: string = '';
  public toHfmisCode: string = '0';
  public hfId: number = 0;
  public fontSize: number = 12;
  public hfName: string = '';
  public designationName: string = '';
  public designationId: number = 0;
  public viewStep: number = 1;
  public scoreType: number = 1;
  public applicationFacilities: any[] = [];
  public recentApplications: any[] = [];
  public finalApplications: any[] = [];
  public rejectedApplications: any[] = [];
  public profiles: any[] = [];
  public leaveOrders: any[] = [];
  public distances: any[] = [];
  public vacancyPercentages: any[] = [];
  public selectedApplication: any;
  public vpMaster: any = {};
  public showOrder: boolean = false;
  public loading: boolean = true;
  public viewOrderWindow: boolean = false;
  public viewWindow: boolean = false;
  public savingMarking: boolean = false;
  public isBHU: boolean = false;
  public isRHC: boolean = false;
  public dateNow: string = '';
  public isTHQ: boolean = false;
  public generatingOrder: boolean = false;
  public currentUser: any;
  public kGrid: KGridHelper = new KGridHelper();
  public profile: any;
  public hfTenure: any = {
    percentage: 79
  };
  public hrMarks: any;
  public markings: any;
  /* public markings: any = {
    service: 0.3,
    serviceDetail: {
      posting: 0,
      totalService: 1
    },
    distance: 0.7,
    distanceDetail: {
      road: 0,
      district: 0.65,
      city: 0.25,
      dhq: 0.5,
      thq: 0.5,
      lhr: 0
    },
    vacancy: 0,
    vacancyDetail: {
      from: 1,
      to: 0
    },
    genderMale: 0,
    genderFemale: 0,
    bhuVacant: 1,
    disability: 2000
  }; */
  @Input() public step: number = 1;

  private subscription: Subscription;
  constructor(private sanitized: DomSanitizer, private _rootService: RootService,
    private _authenticationService: AuthenticationService, private route: ActivatedRoute,
    private _applicationFtsService: ApplicationFtsService) { }

  ngOnInit() {
    this.currentUser = this._authenticationService.getUser();
    if (!this.currentUser) {
      this._authenticationService.logout();
    }
    this.fetchParams();
    let today = new Date();
    let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();

    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('hfId') && params.hasOwnProperty('dsgId')) {

          if (+params['hfId'] && +params['dsgId']) {
            this.hfId = +params['hfId'];
            this.designationId = +params['dsgId'];
            this.fetchApplications(this.designationId, '');
          }
        } else {
          this.getAllApplications();
        }
      }
    );
  }
  public getHrMarkings() {
    this._rootService.getHrMarkings(this.designationId).subscribe((data: any) => {
      if (data) {
        this.hrMarks = data;
        this.markings = {
          service: this.hrMarks.Service,
          serviceDetail: {
            posting: this.hrMarks.ServiceCurrent,
            totalService: this.hrMarks.ServiceTotal
          },
          distance: this.hrMarks.Distance,
          distanceDetail: {
            road: this.hrMarks.P2P,
            district: this.hrMarks.PreferredDistrict,
            city: this.hrMarks.MainCity,
            dhq: this.hrMarks.NearDHQ,
            thq: this.hrMarks.NearTHQ,
            lhr: this.hrMarks.FromLahoreCity
          },
          vacancy: this.hrMarks.Vacancy,
          vacancyDetail: {
            from: this.hrMarks.VacancyFrom,
            to: this.hrMarks.VacancyTo
          },
          genderMale: this.hrMarks.Male,
          genderFemale: this.hrMarks.Female,
          bhuVacant: this.hrMarks.BhuMoreVacant,
          disability: this.hrMarks.Disability
        };
      }
    },
      err => {
        console.log(err);
      });
  }
  public saveHrMarkings() {
    this.savingMarking = true;
    this._rootService.saveHrMarkings(this.hrMarks).subscribe((data: any) => {
      this.savingMarking = false;
      if (data) {
        this.hrMarks = data;
        this.fetchApplications(this.designationId, this.designationName);
      }
    },
      err => {
        console.log(err);
      });
  }
  public getAllApplications() {
    this._applicationFtsService.getHFApplications(this.toHfmisCode).subscribe((data: any) => {
      if (data) {
        this.applicationFacilities = data;

        this.loading = false;
        /*  this.profiles = data.ProfileDetailsViews;
         this.distances = data.FacilityDistances;
         this.vacancyPercentages = data.FacilityVacancyPercentage; */
      }
    },
      err => {
        console.log(err);
      });
  }
  public fetchDesignations(hfId: number, hfName: string) {
    /* this.profile = null; */
    this.loading = true;
    this.isBHU = false;
    this.isRHC = false;
    this.isTHQ = false;
    this.hfId = hfId;
    this.hfName = hfName;
    if (hfName.includes('Basic Health')) {
      this.isBHU = true;
      this.viewStep = 3;
      this.fetchApplications(2404, '');
      return;
    }
    if (hfName.includes('Rural Health')) {
      this.isRHC = true;
    }
    if (hfName.includes('Tehsil Head')) {
      this.isTHQ = true;
    }
    this._applicationFtsService.getHFDesignationApps(this.hfId).subscribe(
      (res: any) => {
        if (res) {
          this.applicationFacilities = res.designations as any[];
          this.loading = false;
          this.viewStep = 2;
        }
      }, err => {
        console.log(err);
      }
    );
  }
  public calculateVacancy() {
    this.vpMaster.Vacant = (+this.vpMaster.TotalSanctioned - +this.vpMaster.TotalWorking) as number;
    this.calculateFinalScore();
  }
  public calculateFinalScore() {
    this.loading = true;
    this.recentApplications.forEach(item => {
      if (item.profile.CNIC == '3520136609741') {
        item.profile.disability = 'Vision (one eye)';
      }
      item.compassionate = item.application.Reason == 'Disability' ? 10 : item.application.Reason == 'Wedlock' ? 5
        : item.application.Reason == 'Divorce' ? 15 : item.application.Reason == 'Widow' ? 10 : 0;
      item.totalService = (this.calculateDays(item.profile.DateOfFirstAppointment) * 0.00142) * this.markings.serviceDetail.totalService;
      item.posting = this.calculateDays(item.profile.PresentPostingDate) * this.markings.serviceDetail.posting;
      item.serviceScore = ((item.totalService + item.posting) as number).toFixed();
      item.service = ((item.totalService + item.posting) * this.markings.service as number).toFixed();
      let totoalDays = 0;
      item.facilityDistances.forEach(distanceObj => {
        if (distanceObj.HFName != 'Total' && distanceObj.HFName != 'Score') {
          totoalDays = totoalDays + distanceObj.TotalDays;
        }
      });
      /*   item.facilityDistances.forEach(distanceObj => {
          distanceObj.road = (distanceObj.ByRoadValue / 1000);
          distanceObj.dhq = (distanceObj.FromDHQValue / 1000) * this.markings.distanceDetail.dhq;
          distanceObj.thq = (distanceObj.FromTHQValue / 1000) * this.markings.distanceDetail.thq;
          distanceObj.lhr = (distanceObj.FromLHRValue / 1000) * this.markings.distanceDetail.lhr;
          distanceObj.city = (distanceObj.FromCityValue / 1000) * this.markings.distanceDetail.city;
        }); */
      let totoalServiceDays = 1095;
      let lhrDistance = 0;
      let roadDistance = 0;
      let preferredDistrict = 0;
      let dhqDistance = 0;
      let thqDistance = 0;
      let mainCityDistance = 0;
      let totalDistances: number[] = [];
      // for health facilities
      for (let index = 0; index < item.facilityDistances.length; index++) {
        /*  totoalServiceDays += item.facilityDistances[index].TotalDays; */
        lhrDistance += (item.facilityDistances[index].FromLHRValue / 1000);
        preferredDistrict += (item.facilityDistances[index].PreferredDistrictValue / 1000);
        roadDistance += (item.facilityDistances[index].ByRoadValue / 1000);
        dhqDistance += (item.facilityDistances[index].FromDHQValue / 1000);
        thqDistance += (item.facilityDistances[index].FromTHQValue / 1000);
        mainCityDistance += (item.facilityDistances[index].FromCityValue / 1000);
      }
      for (let index = 0; index < item.facilityDistances.length; index++) {
        item.facilityDistances[index].totalDistance = ((((((item.facilityDistances[index].FromLHRValue / 1000)) * this.markings.distanceDetail.lhr) +
          (((item.facilityDistances[index].ByRoadValue / 1000)) * this.markings.distanceDetail.road) +
          (((item.facilityDistances[index].FromDHQValue / 1000)) * this.markings.distanceDetail.dhq) +
          (((item.facilityDistances[index].FromTHQValue / 1000)) * this.markings.distanceDetail.thq) +
          (((item.facilityDistances[index].PreferredDistrictValue / 1000)) * this.markings.distanceDetail.district) +
          (((item.facilityDistances[index].FromCityValue / 1000)) * this.markings.distanceDetail.city))) as number).toFixed();
        totalDistances.push(+item.facilityDistances[index].totalDistance);
      }
      /*  + (roadDistance / totoalServiceDays) */
      debugger;
      item.subScoreSum = ((roadDistance * this.markings.distanceDetail.road) + (dhqDistance * this.markings.distanceDetail.dhq)
        + (thqDistance * this.markings.distanceDetail.thq) + (mainCityDistance * this.markings.distanceDetail.city) as number).toFixed();
      item.subScore = +(item.subScoreSum * 0.0278);
      /* ByRoad: (((roadDistance / totoalServiceDays) * this.markings.distanceDetail.road) as number).toFixed(), */
      /*    item.facilityDistances.push({
         HFName: 'Total',
         TotalDays: totoalDays,
         ByRoad: (+((roadDistance) * this.markings.distanceDetail.lhr) as number).toFixed(),
         FromCity: (+((mainCityDistance) * this.markings.distanceDetail.city) as number).toFixed(),
         FromDHQ: (+((dhqDistance) * this.markings.distanceDetail.dhq) as number).toFixed(),
         FromTHQ: (+((thqDistance) * this.markings.distanceDetail.thq) as number).toFixed()
       }); */
      /* item.facilityDistances.push({
        HFName: 'Sub Total',
        lhrDistance: +lhrDistance,
        dhqDistance: +dhqDistance,
        thqDistance: +thqDistance,
        mainCityDistance: +mainCityDistance,
        totoalServiceDays: +totoalServiceDays,
        TotalDays: +item.subScore
      }); */
      item.distance = (+item.subScore * +this.markings.distance as number).toFixed();
      item.facilityDistances.push({
        HFName: 'Total',
        roadDistance: +roadDistance,
        dhqDistance: +dhqDistance,
        thqDistance: +thqDistance,
        mainCityDistance: +mainCityDistance,
        totalDistances: totalDistances,
        TotalDays: +item.subScore
      });
      item.facilityDistances.push({
        HFName: 'Distance Marks',
        TotalDays: +item.subScore
      });
      item.fromVacancy = item.facilityVacancyPercentage.PercentageFrom * this.markings.vacancyDetail.from;
      item.toVacancy = item.facilityVacancyPercentage.PercentageTo * this.markings.vacancyDetail.to;
      item.vacancyScore = (((item.fromVacancy + item.toVacancy)) as number).toFixed();
      item.vacancy = (((item.fromVacancy + item.toVacancy) * this.markings.vacancy) as number).toFixed();

      item.finalScore = (+item.service + +item.distance + +item.vacancy) as number;
      if (item.profile.disability) {
        item.finalScore = (+item.finalScore + +this.markings.disability) as number;
      }

      if (this.isBHU) {
        if (item.profile.Gender == 'Female') {
          item.bhuVacant = 93;
          item.finalScore = +item.finalScore + +this.markings.genderFemale as number;
        } else if (item.profile.Gender == 'Male') {
          item.bhuVacant = 23;
          item.finalScore = +item.finalScore + +this.markings.genderMale as number;
        }
        if (item.bhuVacant > this.hfTenure.percentage) {
          item.finalScore = (+item.finalScore * this.markings.bhuVacant) as number;
        }
        item.finalScore = item.finalScore ? +item.finalScore as number : 0;
        item.finalScore = +item.finalScore.toFixed();
      }
      item.finalScore = +item.finalScore as number + item.compassionate as number;
      item.finalScore = +item.finalScore as number;
    });
    this.kGrid.data = [];
    this.recentApplications.sort(this.compare);
    this.kGrid.data = this.recentApplications;
    this.kGrid.data.sort(this.compare);
    this.kGrid.totalRecords = this.recentApplications.length;
    this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
    this.setOrderApplicants();
    this.loading = false;

  }
  public generateOrder() {
    this.generatingOrder = true;
    this.finalApplications.forEach(obj => {
      let applicationLog: any = {};
      applicationLog.Application_Id = obj.application.Id;
      applicationLog.FromOfficer_Id = 165;
      applicationLog.ToStatus_Id = 2;
      applicationLog.ToStatus = 'Approved';
      applicationLog.Remarks = 'Marks: ' + (obj.finalScore ? obj.finalScore : 0) + '';
      this._applicationFtsService.createApplicationLog(applicationLog).subscribe((res: any) => { });
    });
    this.rejectedApplications.forEach(obj => {
      let applicationLog: any = {};
      applicationLog.Application_Id = obj.application.Id;
      applicationLog.FromOfficer_Id = 165;
      applicationLog.ToStatus_Id = 3;
      applicationLog.ToStatus = 'Rejected';
      applicationLog.Remarks = 'Marks: ' + (obj.finalScore ? obj.finalScore : 0) + '';
      this._applicationFtsService.createApplicationLog(applicationLog).subscribe((res: any) => { });
    });
    this.showOrder = true;
    this.generatingOrder = false;
  }
  private compare(a, b) {
    if (a.finalScore > b.finalScore) { return -1; }
    if (a.finalScore < b.finalScore) { return 1; }
    return 0;
  }

  public fetchApplications(desigId: number, designationName) {
    /* this.profile = null; */
    this.viewStep = 3;
    this.loading = true;
    this.designationId = desigId;
    this.designationName = designationName ? designationName : '';
    this.getHrMarkings();
    this._applicationFtsService.getHFApps(this.hfId, this.designationId).subscribe(
      res => {
        if (res) {
          console.log(res);
          this.recentApplications = res as any[];
          this.vpMaster = this.recentApplications[0] ? this.recentApplications[0].vpMaster ? this.recentApplications[0].vpMaster : {} : {};
          this.kGrid.data = this.recentApplications;
          this.kGrid.totalRecords = this.recentApplications.length;
          this.kGrid.gridView = { data: this.kGrid.data, total: this.kGrid.totalRecords };
          console.log(this.kGrid.gridView);
          
          this.calculateFinalScore();
          this.setOrderApplicants();
        }
        this.loading = false;
      }, err => {
        this.loading = false;
        console.log(err);
      }
    );
  }
  public setOrderApplicants() {
    this.recentApplications.sort(this.compare);
    this.finalApplications = [];
    this.rejectedApplications = [];
    /*  let vacantPercentage = ((this.vpMaster.TotalWorking / this.vpMaster.TotalSanctioned) * 100) as number;
     vacantPercentage = Math.round(vacantPercentage);
     console.log(vacantPercentage);
     let toBeFilled: number = Math.round((90 * this.vpMaster.TotalSanctioned) / 100);
     console.log(+toBeFilled); */
    let count: number = 0;
    if (this.recentApplications.length <= this.vpMaster.Vacant) {
      this.finalApplications = this.recentApplications;
    } else if (this.recentApplications.length > this.vpMaster.Vacant) {
      for (let i = 0; i < this.vpMaster.Vacant; i++) {
        if (this.recentApplications[i]) {
          count = i;
          this.finalApplications.push(this.recentApplications[i]);
        }
      }
    }
    for (let i = this.vpMaster.Vacant; i < this.recentApplications.length; i++) {
      this.rejectedApplications.push(this.recentApplications[i]);
    }
    console.log(this.recentApplications);
    console.log(this.finalApplications);
    console.log(this.rejectedApplications);
  }
  public viewOrder(order) {
    this.selectedApplication = order;
    if (this.selectedApplication) {
      console.log(this.selectedApplication);

      this.openViewOrderWindow();
    }
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedApplication = null;
  }

  public viewApplication(dataItem, type: number) {
    this.scoreType = type;
    this.selectedApplication = dataItem;
    if (this.selectedApplication) {
      this.openViewWindow();
    }
  }
  public openViewWindow() {
    this.viewWindow = true;
  }
  public closeViewWindow() {
    this.viewWindow = false;
    this.selectedApplication = null;
  }
  public sortChange(sort: SortDescriptor[]): void {
    if (sort[0].field == 'asd') { return; }
    this.kGrid.sort = sort;
    this.sortData();
  }
  private sortData() {
    this.kGrid.gridView = {
      data: orderBy(this.kGrid.data, this.kGrid.sort),
      total: this.kGrid.totalRecords
    };
  }
  public calculateDays(date: any) {
    let dateNow = new Date();
    let date2 = new Date(date);
    let Difference_In_Time = dateNow.getTime() - date2.getTime();
    let Difference_In_Days: number = Difference_In_Time / (1000 * 3600 * 24);
    return +Difference_In_Days.toFixed();
  }
  public getDistance(id: number) {
    let dist = this.distances.find(x => x.HFId == id);
    return dist ? dist.Distance.toFixed() : 0;
  }
  public getAppTime(id: number) {
    let dateTime = this.recentApplications.find(x => x.Profile_Id == id);
    return dateTime ? new Date(dateTime.Created_Date) : 0;
  }
  public getVacancyPercentage(id: number) {
    let vacancyPercentage = this.vacancyPercentages.find(x => x.HFId == id);
    return vacancyPercentage ? vacancyPercentage.Percentage.toFixed() : 0;
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  public dashifyCNIC(cnic: string) {
    return cnic[0] +
      cnic[1] +
      cnic[2] +
      cnic[3] +
      cnic[4] +
      '-' +
      cnic[5] +
      cnic[6] +
      cnic[7] +
      cnic[8] +
      cnic[9] +
      cnic[10] +
      cnic[11] +
      '-' +
      cnic[12];
  }
  makeOrderDate(day: number) {
    return day == 1 || day == 21 || day == 31 ? day + '<sup>st</sup>'
      : day == 2 || day == 22 ? day + '<sup>nd</sup>'
        : day == 3 || day == 23 ? day + '<sup>rd</sup>'
          : (day => 4 && day <= 20) || (day => 24 && day <= 30) ? day + '<sup>th</sup>' : '';
  }
  makeOrderMonth(month: number) {
    return month == 1 ? 'January'
      : month == 2 ? 'February'
        : month == 3 ? 'March'
          : month == 4 ? 'April'
            : month == 5 ? 'May'
              : month == 6 ? 'June'
                : month == 7 ? 'July'
                  : month == 8 ? 'August'
                    : month == 9 ? 'September'
                      : month == 10 ? 'October'
                        : month == 11 ? 'November'
                          : month == 12 ? 'December' : '';
  }

  printApplication() {
    let html = document.getElementById('applicationPrintOld').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
          <style>
            
            body {
           
              font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
              font-size: 13px;
              font-style: normal;
              font-variant: normal;
              font-weight: 400;
              line-height: 1.6;
              color: #383e4b;
              background-color: #fff;
          }
          p{
              word-wrap: break-word;
              font-family: sans-serif, Arial, Verdana, "Trebuchet MS";
              font-size: 13px;
              font-style: normal;
              font-variant: normal;
              font-weight: 400;
              line-height: 1.6;
          }
          .mt-2 {
            margin-top: 0.5rem !important;
          }.mb-0 {
            margin-bottom: 0 !important;
          }
          .ml-1 {
            margin-left: 0.25rem !important;
          }
          .mb-2 {
            margin-bottom: 0.5rem !important;
          }
       
          button.print {
            padding: 10px 40px;
            font-size: 21px;
            position: absolute;
            margin-left: 40%;
            background: #46a23f;
            background-image: linear-gradient(rgba(63, 162, 79, 0), rgba(63, 162, 79, 0.2));
            cursor: pointer;
            border: none;
            color: #ffffff;
            z-index: 9999;
          }
        
  
          .w-20 { width: 20% !important; }
          .w-30 { width: 30% !important; }
          .w-50 { width: 50% !important; }
          .w-70 { width: 70% !important; }
          .w-80 { width: 80% !important; }
  
          .mt-10 { margin-top: 10px !important; }
          .mt-30 { margin-top: 30px  !important; }
          @media print {
            button.print {
              display: none;
            }
          }
                </style>
                <title>Application</title>`);
      mywindow.document.write('</head><body >');
      mywindow.document.write('<button onclick="printFunc()" class="print">Print</button>');
      mywindow.document.write(html);
      mywindow.document.write(`
      <div class="divFooter" style="position: fixed;
      bottom: 0;    left: 25%;color:#e3e3e3;">Powered by Health Information and
        Services Delivery Unit</div>
                  <script>
            function printFunc() {
              window.print();
            }
            </script>
        `);
      mywindow.document.write('</body></html>');

      mywindow.document.close(); // necessary for IE >= 10
      mywindow.focus(); // necessary for IE >= 10
      mywindow.print();
      mywindow.close();
    }
  }

  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
}
