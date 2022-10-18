import { Component, OnInit, OnDestroy } from '@angular/core';
import { DropDownsHR } from '../../_helpers/dropdowns.class';
import { ActivatedRoute, Router } from '@angular/router';
import { FileTrackingSystemService } from '../file-tracking-system.service';
import { ApplicationMaster, ApplicationView, ApplicationLog, ApplicationDocument, ApplicationAttachment, ApplicationLogView } from '../../modules/application-fts/application-fts';
import { Subscription } from 'rxjs/Subscription';
import { RootService } from '../../_services/root.service';
import { AuthenticationService } from '../../_services/authentication.service';
import { DomSanitizer } from '@angular/platform-browser';
import { NotificationService } from '../../_services/notification.service';
import { User } from '../../_models/user.class';
import { FirebaseHisduService } from '../../_services/firebase-hisdu.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { PandSOfficerView } from '../../modules/user/user-claims.class';
import { VpPack } from '../../modules/vacancy-position/vacancy-position.class';

@Component({
  selector: 'app-summary',
  templateUrl: './summary.component.html',
  styles: [`
  `]
})
export class SummaryComponent implements OnInit, OnDestroy {
  public loading: boolean = true; public mobileMask: string = "0000-0000000";

  public logRefreshing: boolean = true;
  public user: User;
  public editApplication: boolean = false;
  public requestingFile: boolean = false;
  public changeFileNumber: boolean = false;
  public changingFileNumber: boolean = false;
  public logAscOrder: boolean = true;
  public fileNotAvailable: boolean = false;
  public fileIssued: boolean = false;
  public fileRequstedBy: string = '';
  public fileRequstedStatus: string = '';
  public forwardingApplication: boolean = false;
  public shouldGenerateLetter: boolean = false;
  public shouldForwardHisdu: boolean = false;
  public forwardedToHisdu: boolean = false;
  public generateOrder: boolean = false;
  public imageWindowOpened: boolean = false;
  public actionDialogOpened: boolean = false;
  public successDialogOpened: boolean = false;
  public requestDialogOpened: boolean = false;
  public lawFileDialogOpened: boolean = false;
  public trackingDialogOpened: boolean = false;
  public approvingApplication: boolean = false;
  public rejectingApplication: boolean = false;
  public searchingDDSFiles: boolean = false;
  public removingApplication: boolean = false;
  public removingApplicationAttachment: boolean = false;
  public uploadingAttachments: boolean = false;
  public uploadingSignedCopy: boolean = false;
  public loadingDocs: boolean = false;
  public isCEO: boolean = false;
  public loadingHealthFacilities: boolean = false;
  public vacantString: string = '';
  public imagePath: string = '';
  public letterType: number = 0;
  private subscription: Subscription;
  private appsRealtimeSubscription: Subscription;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public application: any;
  public applicationLog: ApplicationLog = new ApplicationLog();
  public applicationLogs: ApplicationLogView[] = [];
  public signedApplication: ApplicationAttachment = new ApplicationAttachment();
  public applicationDueDate: any = {};
  public applicationOlds: any[] = [];
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];
  public healthFacilities: any[] = [];
  public hfsList: any[] = [];
  public hfsList2: any[] = [];
  public vpPack: VpPack = new VpPack();
  public personAppeared: any;
  public selectedFiltersModel: any = {};

  public applicationDocuments: ApplicationDocument[] = [];
  public ddsFilesListAll: any[] = [];
  public ddsFilesList: any[] = [];
  public lawFilesListAll: any[] = [];
  public lawFilesList: any[] = [];
  public hrfilesByFileName: any[] = [];
  public hrfilesByFileCNIC: any[] = [];
  public hrfilesByFileNumber: any[] = [];
  public ddsFile: any;
  public ddsfilesByFileName: any[] = [];
  public ddsfilesByFileCNIC: any[] = [];
  public ddsfilesByFileNumber: any[] = [];

  public filesByFileName: any[] = [];
  public filesByFileCNIC: any[] = [];
  public filesByFileNumber: any[] = [];

  public fileRequisitions: any[] = [];

  public sectionOfficers: any[] = [];
  public sectionOfficersData: any[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public applicationAttachmentsUpload: ApplicationAttachment[] = [];
  public barcodeImgSrc: string = '';
  public applicationType: string = 'New Application';
  public selectedTabName: string = '';
  public activeIndex: number = 0;
  public activeApplicationAttachmentId: number = 0;
  public applicationId: number = 0;
  public applicationtracking: number = 0;
  public lastLogId: number = 0;
  public actionId: number = 0;
  public actionName: string = '';

  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription = null;

  public currentOfficer: PandSOfficerView;
  constructor(private sanitizer: DomSanitizer, private _fileTrackingSystemService: FileTrackingSystemService,
    private route: ActivatedRoute, public _notificationService: NotificationService,
    private router: Router, private _firebaseHisduService: FirebaseHisduService, private _rootService: RootService, private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    window.scroll(0, 0);
    this.user = this._authenticationService.getUser();
    this.isCEO = this.user.UserName.toLowerCase().startsWith('ceo.') ? true : false;
    this._rootService.getCurrentOfficer().subscribe((res: PandSOfficerView) => {
      if (res) {
        this.currentOfficer = res;
      }
    });
    this.fetchParams();
    this.handleSearchEvents();
  }
  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id') && +params['id'] && params.hasOwnProperty('tracking') && +params['tracking']) {
          this.applicationId = +params['id'];
          this.applicationtracking = +params['tracking'];
          this.getApplication(this.applicationId, this.applicationtracking);
        } else {
        }
      }
    );
  }
  private loadDropdownValues = () => {

    this.getLeaveTypes();
    this.getDepartments();
    this.getApplicationPurposes();
    this.getDesignations();

    if (this.isCEO) {
      this.getPandSOfficers('ceo');
    } else {
      if (this.application.ApplicationType_Id == 77 || this.application.ApplicationType_Id == 88) {
        this.getPandSOfficers('punjab');
      } else {
        this.getPandSOfficers('concerned');
      }
    }
  }
  private subscribeLiveApp() {
    /*   this.appsRealtimeSubscription = this._firebaseHisduService.getAppsChanged().subscribe((apps: any) => {
        if (apps && this.application) {
          let app = apps.find(x => x.trackingNo == this.application.TrackingNumber);
          if (app) {
            this.getApplicationLog(false);
          }
        }
      }); */
  }
  public sendFileRequest(fileId: number) {
    this.requestingFile = true;
    this.applicationLog = new ApplicationLog();
    this.applicationLog.Application_Id = this.application.Id;
    this._fileTrackingSystemService.generateFileRequest(this.applicationLog, fileId).subscribe((data: any) => {
      if (data) {
        if (data.result) {
          this.applicationLog = new ApplicationLog();
          this.requestingFile = false;
          this.fileRequstedBy = '';
          this.fileRequstedStatus = '';
          this._notificationService.notify('success', 'Requested File ' + this.ddsFile.DiaryNo);
          /*  this._firebaseHisduService.updateApplicationFirebase(this.applicationtracking); */

          this.getApplication(this.applicationId, this.applicationtracking);
          this.closeRequestDialog();
        } else if (data.result == false) {
          this.applicationLog = new ApplicationLog();
          this.requestingFile = false;
          this.fileRequstedBy = data.madeBy;
          this.fileRequstedStatus = data.status;
        }
      }
    }, err => {
      this.requestingFile = false;
      this.handleError(err);
    });
  }
  private getApplicationDocuments = () => {
    this.loadingDocs = true;
    this.applicationDocuments = [];
    this._rootService.getApplicationDocuments(this.application.ApplicationType_Id).subscribe((res: any[]) => {
      if (res) {

        let tempIds = [];
        this.applicationAttachments.forEach(x => {
          let doc = res.find(y => y.Id == x.Document_Id);
          if (doc) {
            tempIds.push(doc.Id);
          }
        });
        res.forEach(doc => {
          let id = tempIds.find(z => z == doc.Id);
          if (!id) {
            this.applicationDocuments.push(doc);
          }
        });
        /*       this.applicationDocuments = res.filter(doc => {
                return this.applicationAttachments.findIndex(m => m.Document_Id == doc.Id) < 0;
              }); */
      }

      this.loadingDocs = false;
      this.uploadingAttachments = false;
      this.applicationAttachmentsUpload = [];
    },
      err => { this.handleError(err); }
    );
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  private getPandSOfficers = (type: string) => {
    this.sectionOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.sectionOfficers = res;
        this.sectionOfficersData = this.sectionOfficers;

        if (this.user.UserName == 'pshd') {
          this._rootService.getPandSOfficers('asds').subscribe((res2: any) => {
            if (res2) {
              res2.forEach(r => {
                this.sectionOfficers.push(r);
              });
              this.sectionOfficersData = this.sectionOfficers;
            }
          },
            err => { this.handleError(err); }
          );
        }
      }
    },
      err => { this.handleError(err); }
    );
    this.dropDowns.selectedFiltersModel.officer = { Id: this.application.ToOfficer_Id, DesignationName: this.application.ToOfficerName };
  }
  private checkVacancy = () => {
    this._fileTrackingSystemService.checkVacancy(this.application.ToHF_Id, this.application.ToHFCode, this.application.ToDesignation_Id).subscribe((res: any) => {
      if (res) {
        this.vacantString = res;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getVpMaster = () => {
    this.vpPack.vpMaster = null;
    this.vpPack.vpDetails = null;
    this.vpPack.vpMasterLog = null;
    this.vpPack.vpMasterLogs = null;
    this._fileTrackingSystemService.getVpMaster({
      HealthFacility_Id: this.application.ToHF_Id,
      Designation_Id: this.application.ToDesignation_Id,
    }).subscribe((res: any) => {
      if (res) {
        console.log(res);
        this.vpPack.vpMaster = res.vpMaster;
        this.vpPack.vpDetails = res.vpDetails;
        this.vpPack.vpMasterLog = res.vpMasterLog;
        this.vpPack.vpMasterLogs = res.vpMasterLogs;
        console.log(this.vpPack.vpMaster);
      }
      this.loading = false;
    },
      err => { this.handleError(err); }
    );
  }
  public changeFileNumberEdit() {
    this.changeFileNumber = true;
    this.openRequestDialog();
  }
  public changeLawFileNumberEdit() {
    this.changeFileNumber = true;
    this.openLawFileDialog();
  }
  private getApplicationStatus() {
    this._rootService.getApplicationStatus().subscribe(
      (response: any) => {
        if (response) {
          let statuses = [];
          response.forEach(element => {
            /*   if (element.Id == 1 || element.Id == 2 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
                statuses.push(element);
              } */
            /* if (this.user.RoleName == 'Section Officer' && this.user.UserName != 'so.conf' && this.application.ApplicationType_Id != 7 && this.application.ApplicationType_Id != 26 && this.application.ApplicationType_Id != 27) {
              if (element.Id == 1 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
                statuses.push(element);
              }
            } else {
              if (element.Id == 1 || element.Id == 2 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
                statuses.push(element);
              }
            } */

            if (element.Id == 1 || element.Id == 3 || element.Id == 8 || element.Id == 10) {
              statuses.push(element);
            }
            if (this.application.ApplicationType_Id != 16 &&
              this.application.ApplicationType_Id != 30 &&
              this.application.ApplicationType_Id != 33 &&
              this.application.ApplicationType_Id != 31) {

              if (element.Id == 2) {
                statuses.push(element);
              }
            }
            /*  if (element.Id == 4 && this.application.FileRequested && this.application.FileRequestStatus_Id == 3) {
               statuses.push(element);
             } */
            /* if (element.Id == 4 && this.application.ApplicationSource_Id == 5) {
              statuses.push(element);
            } */
          });
          this.dropDowns.applicationStatus = statuses;
          this.dropDowns.applicationStatusData = this.dropDowns.applicationStatus;
        }
      },
      err => this.handleError(err)
    );
  }
  private getDivisions = (code: string) => {
    this.dropDowns.divisions = [];
    this.dropDowns.divisionsData = [];
    this._rootService.getDivisions(code).subscribe((res: any) => {

      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {
        let division = this.dropDowns.divisionsData.find(x => x.Code == this.application.HfmisCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
        }
      }
    },
      err => { this.handleError(err); }
    );
  }

  private getDistricts = (code: string) => {
    this.dropDowns.districts = [];
    this.dropDowns.districtsData = [];
    this._rootService.getDistricts(code).subscribe((res: any) => {
      this.dropDowns.districts = res;
      this.dropDowns.districtsData = this.dropDowns.districts.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {

        let district = this.dropDowns.districtsData.find(x => x.Code == this.application.HfmisCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
        }

      }
    },
      err => { this.handleError(err); }
    );
  }
  private getTehsils = (code: string) => {
    this.dropDowns.tehsils = [];
    this.dropDowns.tehsilsData = [];
    this._rootService.getTehsils(code).subscribe((res: any) => {
      this.dropDowns.tehsils = res;
      this.dropDowns.tehsilsData = this.dropDowns.tehsils.slice();
      if (this.application && this.application.HfmisCode && this.application.HfmisCode.length == 19) {
        let tehsil = this.dropDowns.tehsilsData.find(x => x.Code == this.application.HfmisCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();

    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesForTransfer = (hfmisCode) => {
    this.loadingHealthFacilities = true;
    this._rootService.getHealthFacilities(hfmisCode, this.application.ToDept_Id).subscribe((res: any) => {
      this.healthFacilities = res;
      this.loadingHealthFacilities = false;
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getDepartments = () => {
    this._rootService.getDepartmentsHealth().subscribe((res: any) => {
      this.dropDowns.departments = res;
      this.dropDowns.departmentsData = this.dropDowns.departments.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getApplicationPurposes = () => {
    this._rootService.getApplicationPurposes().subscribe((res: any) => {
      this.dropDowns.applicationPurposes = res;
    },
      err => { this.handleError(err); }
    );
  }
  private getLeaveTypes = () => {
    this._rootService.getLeaveTypes().subscribe((res: any) => {
      this.dropDowns.leaveTypes = res;
      this.dropDowns.leaveTypesData = this.dropDowns.leaveTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  private getAll = (code: string) => {
    if (code.length <= 1) {
      this._rootService.getDivisions(code).subscribe((res: any) => {
        this.divisions = res;
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 3) {
      this.resetDropsBelow('division');
      this._rootService.getDistricts(code).subscribe((res: any) => {
        this.districts = res;
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 6) {
      this.resetDropsBelow('district');
      this._rootService.getTehsils(code).subscribe((res: any) => {
        this.tehsils = res;
      },
        err => { this.handleError(err); }
      );
    }
  }

  private resetDrops = (filter: string) => {
    this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
    this.application.fromHealthFacility = '';
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.district = { Name: 'Select District', Code: '0' };
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.application.fromHealthFacility = '';
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  private resetDropsBelow = (filter: string) => {
    this.application.toHealthFacility = '';
    this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: 'Select Health Facility', Id: 0 };
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.districtForTransfer = { Name: 'Select District', Code: '0' };
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
    if (filter == 'district') {
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '0' };
    }
  }
  private getApplication(id, tracking) {
    this._fileTrackingSystemService.getApplication(+id, +tracking).subscribe((data: any) => {
      if (data) {
        this.application = data.application;
        this.application.DateOfBirth = new Date(this.application.DateOfBirth);
        this.application.FromDate = new Date(this.application.FromDate);
        this.application.ToDate = new Date(this.application.ToDate);
        this._rootService.generateBars(this.application.TrackingNumber).subscribe((res: any) => { this.barcodeImgSrc = res.barCode; }, err => { this.handleError(err); });
        this.applicationAttachments = data.applicationAttachments as ApplicationAttachment[];
        this.applicationLog.Application_Id = this.application.Id;
        this.dropDowns.selectedFiltersModel.applicationStatus = { Id: this.application.Status_Id, Name: this.application.StatusName };
        this.dropDowns.selectedFiltersModel.leaveType = { Id: this.application.LeaveType_Id, LeaveType1: this.application.leaveType };
        this.loadDropdownValues();
        this.getApplicationStatus();
        this.getAll('0');
        this.getDivisions('0');
        this.getDistricts('0');
        this.getTehsils('0');

        this.loading = false;
        this.subscribeLiveApp();
        this.getApplicationData("logs");
        this.getApplicationData("oldlogs");
        this.getApplicationData("file");
        this.getApplicationData("filereqs");
        this.getApplicationData("parliamentarian");
        this.getApplicationDocuments();
        if (this.application.ApplicationType_Id == 2 && (this.user.UserName == 'og1' || this.user.UserName == 'ds.admin' || this.user.UserName == 'a.system' || this.user.UserName == 'so.toqeer' || this.user.UserName == 'ds.general' || this.user.UserName == 'ordercell')) {
          this.checkVacancy();
          this.getVpMaster();
        }
        this.closeWindow();
      }
    }, err => { this.handleError(err); }
    );
  }

  public getApplicationLog(all: boolean) {
    this.logRefreshing = true;
    let lastLog = null;
    if (this.applicationLogs.length > 0) {
      lastLog = this.applicationLogs[this.applicationLogs.length - 1];
      this.lastLogId = lastLog ? lastLog.Id : 0;
    }
    if (!all) {
      this.lastLogId = 0;
    }
    this._fileTrackingSystemService.getApplicationLogs(this.application.Id, this.lastLogId, this.logAscOrder).subscribe((data: any) => {
      if (data) {
        if (lastLog) {
          data.forEach(element => {
            if (this.logAscOrder) {
              this.applicationLogs.push(element);
            } else {
              this.applicationLogs.unshift(element);
            }
          });
        } else {
          this.applicationLogs = data as ApplicationLogView[];
        }
        this.logRefreshing = false;
      }
    }, err => { this.logRefreshing = false; this.handleError(err); }
    );
  }
  private getApplicationData(type) {
    this._fileTrackingSystemService.getApplicationData(this.application.Id, type).subscribe((data: any) => {
      if (data) {
        if (type == "logs") {
          this.getApplicationLog(true);
        }
        else if (type == "oldlogs") {
          this.applicationOlds = data.applicationForwardLogs;
        }
        else if (type == "file") {
          this.ddsFile = data.File;
        }
        else if (type == "parliamentarian") {
          this.personAppeared = data.applicationPersonAppeared;
        }
        else if (type == "filereqs") {
          this.fileRequisitions = data.applicationFileRecositions;
          if (this.fileRequisitions.length > 0) {
            console.log(this.fileRequisitions);
            /*  this.fileRequstedBy */
          }
        }

        this.loading = false;
      }
    }, err => { this.handleError(err); }
    );
  }
  public attachVacancy() {
    if (this.vpPack.vpMaster && this.vpPack.vpMaster.Id > 0 && this.vpPack.vpMaster.Vacant > 0) {
      this.vpPack.vpHolder.Id = 0;
      this.vpPack.vpHolder.VpMaster_Id = this.vpPack.vpMaster.Id;
      this.vpPack.vpHolder.TotalSeats = this.vpPack.vpMaster.TotalSanctioned;
      this.vpPack.vpHolder.TotalSeatsHold = 1;
      this.vpPack.vpHolder.TotalSeatsVacant = this.vpPack.vpMaster.TotalSanctioned - this.vpPack.vpMaster.TotalWorking;
      this.vpPack.vpHolder.VpDetail_Id = null;
      this.vpPack.vpHolder.TrackingNumber = this.application.TrackingNumber;
      this.vpPack.vpHolder.EmployeeName = this.application.EmployeeName;
      this.vpPack.vpHolder.FileNumber = this.application.FileNumber;
      this.saveVacancyHolder();
    }
  }
  public saveVacancyHolder() {
    this._fileTrackingSystemService.saveVacancyHolder(this.vpPack.vpHolder).subscribe((res) => {
      if (res) {
        this._notificationService.notify('success', 'Vacancy Holded');
      }
    }, err => {
      console.log(err);
    });
  }
  public dropdownValueChanged = (value, filter) => {

    if (filter == 'purpose') {
      this.applicationLog.Purpose_Id = value.Id;
      this.applicationLog.Purpose = value.Purpose;
    }
    if (filter == 'officer') {
      this.applicationLog.ToOfficer_Id = value.Id;
      this.applicationLog.ToOfficerName = value.DesignationName;
    }
    if (filter == 'status') {
      this.applicationLog.ToStatus_Id = value.Id;
      this.applicationLog.ToStatus = value.Name;
      if ((this.applicationLog.ToStatus_Id == 2 || this.applicationLog.ToStatus_Id == 3 || this.applicationLog.ToStatus_Id == 8) && this.application.MobileNo) {
        this.applicationLog.SMS_SentToApplicant = true;
      }
    }
    if (filter == 'division') {
      this.application.HfmisCode = value.Code;
      this.resetDrops(filter);
      this.getDistricts(value.Code);
      this.getTehsils(value.Code);
    }
    if (filter == 'district') {
      this.application.HfmisCode = value.Code;
      this.resetDrops(filter);
      this.getTehsils(value.Code);
    }
    if (filter == 'tehsil') {
      this.resetDrops(filter);
      this.application.HfmisCode = value.Code;
      this.getHealthFacilities(this.application.HfmisCode);
    }
    if (filter == 'healthFacility') {
      let item = this.hfsList.find(x => x.FullName === value);
      if (item) {
        this.application.HfmisCode = item.HFMISCode;
        this.application.HealthFacility_Id = item.Id;
        this.application.FromHF_Id = item.Id;
        this.application.fromHealthFacility = item.FullName;
        let divisions = this.dropDowns.divisions as any;
        let division = divisions.find(x => x.Code == this.application.HfmisCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
        }

        let districts = this.dropDowns.districts as any;
        let district = districts.find(x => x.Code == this.application.HfmisCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
        }

        let tehsils = this.dropDowns.tehsils as any;
        let tehsil = tehsils.find(x => x.Code == this.application.HfmisCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }
    }
    if (filter == 'd d') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.getAll(this.application.ToHFCode);
    }
    if (filter == 't') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.resetDropsBelow('teh sil');
      this.getHealthFacilitiesForTransfer(value.Code);
    }

    if (filter == 'healthFacil ity2') {
      let item = this.hfsList2.find(x => x.FullName === value);
      if (item) {
        this.application.ToHFCode = item.HFMISCode;
        this.application.ToHF_Id = item.Id;
        this.application.toHealthFacility = item.FullName;

        let divisions = this.dropDowns.divisions as any;
        let division = divisions.find(x => x.Code == this.application.ToHFCode.substring(0, 3));
        if (division) {
          this.dropDowns.selectedFiltersModel.divisionForTransfer = { Code: division.Code, Name: division.Name };
        }

        let districts = this.dropDowns.districts as any;
        let district = districts.find(x => x.Code == this.application.ToHFCode.substring(0, 6));
        if (district) {
          this.dropDowns.selectedFiltersModel.districtForTransfer = { Code: district.Code, Name: district.Name };
        }

        let tehsils = this.dropDowns.tehsils as any;
        let tehsil = tehsils.find(x => x.Code == this.application.ToHFCode.substring(0, 9));
        if (tehsil) {
          this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Code: tehsil.Code, Name: tehsil.Name };
        }
      }

    }
  }
  transform(value) {
    return this.sanitizer.bypassSecurityTrustHtml(value);
  }
  public approveReject() {
    if (this.actionId == 0 || (this.actionId != 2 && this.actionId != 3)) return;
    this.applicationLog.ToStatus_Id = this.actionId;
    if (this.actionId == 2) {
      this.applicationLog.ToStatus = 'Approved';
    } else if (this.actionId == 3) {
      this.applicationLog.ToStatus = 'Rejected';
    }
    this.updateLog();
  }
  public changeFileNumberOfApplication() {
    this.changingFileNumber = true;
    this._fileTrackingSystemService.changeFileNumberOfApplication(this.application).subscribe((res) => {
      if (res) {
        this.getApplicationData('file');
        this._notificationService.notify('success', 'File Number Changed: ' + this.application.FileNumber);
        this.fileRequstedBy = '';
        this.closeRequestDialog();
      } else {
        this.application.DDS_Id = null;
        this.application.FileNumber = null;
      }
      this.changingFileNumber = false;
    }, err => {
      this.handleError(err);
    })
  }
  public search(value: string, filter: string) {
    if (filter == 'ddsfiles') {
      this.ddsFilesListAll = [];
      this.ddsFilesList = [];
      this.application.FileNumber = value;
      this.application.DDS_Id = null;
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getDDSFilesByFileNumber(value).subscribe((data) => {
          this.ddsFilesListAll = data as any[];
          this.ddsFilesList = this.ddsFilesListAll.slice(0, 37);
          this.searchingDDSFiles = false;
        });
      }
    }
    if (filter == 'lawfiles') {
      this.lawFilesListAll = [];
      this.lawFilesList = [];
      //this.application.FileNumber = value;

      this.application.DDS_Id = null;
      if (value.length > 2) {
        this.searchingDDSFiles = true;
        this._rootService.getLawFilesByFileNumber(value).subscribe((data) => {
          this.lawFilesListAll = data as any[];
          this.lawFilesList = this.lawFilesListAll;
          this.searchingDDSFiles = false;
        });
      }
    }
  }
  public searchClicked(FileNumber, filter) {
    if (filter == 'ddsfiles') {
      let item = this.ddsFilesList.find(x => x.DiaryNo === FileNumber);
      if (item) {
        this.application.DDS_Id = item.Id;
        this.application.FileNumber = item.DiaryNo;
      }
    }
    if (filter == 'lawfiles') {
      let item = this.lawFilesList.find(x => x.FileNumber === FileNumber);
      if (item) {
        this.application.DDS_Id = item.Id;
        this.application.FileNumber = item.FileNumber;
        this.application.CaseNumber = item.CaseNumber;
        this.application.CourtTitle = item.CourtTitle;
        this.application.Title = item.Title;
      }
    }
  }
  updateLog() {
    this.forwardingApplication = true;
    this.shouldGenerateLetter = false;
    this.shouldForwardHisdu = false;
    if (this.user.UserName.toLowerCase().startsWith('ceo.')) {
      this.applicationLog.FromOfficer_Id = 71;
      this.applicationLog.ToOfficer_Id = 75;
      this.applicationLog.ToOfficerName = 'Project Director (HISDU)';
    }
    if (this.applicationLog.DueDate) {
      this.applicationLog.DueDate = this.applicationLog.DueDate.toDateString();
    }
    this._fileTrackingSystemService.createApplicationLog(this.applicationLog).subscribe((response: ApplicationLog) => {
      if (response.Id) {
        this.forwardingApplication = false;
        //this._firebaseHisduService.updateApplicationFirebase(this.applicationtracking);
        /*  if (response.ToStatus_Id) {
           // if (response.ToStatus_Id == 8 || response.ToStatus_Id == 2 || response.ToStatus_Id == 3 || response.ToStatus_Id == 4) {
           //below code was user for only waiting document letter but there is none yet 
           if (response.ToStatus_Id == 8) {
             this.letterType = 1;
             this.generateLetter();
             this.shouldGenerateLetter = true;
           } else if (response.ToStatus_Id == 2 && !this.forwardedToHisdu) {
             //this.shouldForwardHisdu = true;
           }
         } */
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }

  public leaveInputsChanged(type: number) {
    if (type == 1) {
      if (this.application.FromDate && this.application.ToDate) {
        this._rootService.calcDate(this.application.FromDate.toDateString(), this.application.ToDate.toDateString(), 0).subscribe((x: number) => {
          this.application.TotalDays = x;
        });
      }
    } else if (type == 2) {
      this._rootService.calcDate(this.application.FromDate.toDateString(), 'noDate', this.application.TotalDays).subscribe((x: any) => {
        this.application.ToDate = new Date(x);
      });
    }
  }
  updateMarkLog() {
    this.forwardingApplication = true;
    this.applicationLog.ToOfficer_Id = this.application.ForwardingOfficer_Id;
    this.applicationLog.ToOfficerName = this.application.ForwardingOfficerName;
    this.applicationLog.ToStatus_Id = 11;
    this.applicationLog.ToStatus = 'Marked';

    this._fileTrackingSystemService.createApplicationLog(this.applicationLog).subscribe((response: any) => {
      if (response.Id) {
        this.forwardingApplication = false;
        this.openSuccessDialog();
      }
    },
      err => { this.handleError(err); }
    );
  }
  public forwardHisdu() {
    this.closeSuccessDialog();
    this.applicationLog = new ApplicationLog();
    this.applicationLog.Application_Id = this.application.Id;
    this.applicationLog.ToOfficer_Id = 67;
    this.applicationLog.ToOfficerName = 'Order Cell';
    this.forwardedToHisdu = true;
    this.shouldForwardHisdu = false;
    this.updateLog();
  }
  public requestMissingDocuments() {
    this.applicationLog.ToStatus_Id = 8;
    this.applicationLog.ToStatus = 'Waiting Documents';
    this.updateLog();

  }

  public handleFilter = (value, filter) => {
    if (filter == 'forwardingOfficer') {
      this.sectionOfficersData = this.sectionOfficers.filter((s: any) => s.DesignationName.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  uploadSignedCopy(event) {
    let inputValue = event.target;
    this.signedApplication.files = inputValue.files;
    this.signedApplication.Document_Id = 1;
    this.signedApplication.attached = true;
    //show send button
  }
  uploadSignedApplication() {
    // upload signed copy
    this.uploadingSignedCopy = true;
    this._fileTrackingSystemService.uploadSignedApplication(this.signedApplication, this.application.Id).subscribe((response) => {
      if (response) {
        this.getApplication(this.applicationId, this.applicationtracking);
        this.uploadingSignedCopy = false;
      }
    }, err => {
      this.handleError(err);
    });
  }
  public selectFile(event, document: ApplicationDocument): void {
    let inputValue = event.target;
    let applicationAttachment: ApplicationAttachment = new ApplicationAttachment();
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachmentsUpload.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
  }
  public uploadAttachments() {
    this.uploadingAttachments = true;
    if (this.applicationAttachmentsUpload.length > 0) {
      this._fileTrackingSystemService.uploadApplicationAttachments(this.applicationAttachmentsUpload, this.application.Id).subscribe((response) => {
        if (response) {
          this.getApplication(this.applicationId, this.applicationtracking);
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public removeApplication() {
    if (confirm('Remove Tracking # ' + this.application.TrackingNumber + '?')) {
      this.removingApplication = true;
      this.applicationLog.Application_Id = this.application.Id;
      this.applicationLog.Action_Id = 17;
      this._fileTrackingSystemService.removeApplication(this.application.Id, this.application.TrackingNumber, this.applicationLog).subscribe((res: any) => {
        if (res) {
          this.removingApplication = false;
          this.router.navigate(['/application']);
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  public removeApplicationAttachment() {
    if (confirm('Remove Attachment?') && this.activeApplicationAttachmentId > 0) {
      this.removingApplicationAttachment = true;
      this._fileTrackingSystemService.removeApplicationAttachment(this.activeApplicationAttachmentId).subscribe((res: any) => {
        if (res) {
          this.removingApplicationAttachment = false;
          this.activeApplicationAttachmentId = 0;
          this.signedApplication.attached = false;
          this.getApplication(this.applicationId, this.applicationtracking);
        }
      }, err => {
        this.handleError(err);
      });
    } else {
      this.removingApplicationAttachment = false;
    }
  }
  public generateLetter() {
    if (this.application.Status_Id == 3) {
      this.router.navigate(['/fts/letter/' + this.application.Id + '/' + this.application.TrackingNumber + '/3'])

    }
    if (this.application.Status_Id == 8) {
      this.router.navigate(['/fts/letter/' + this.application.Id + '/' + this.application.TrackingNumber + '/1'])

    }

  }
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
  }
  public openWindow(imagePath: string, index: number, attahcmentId: number) {
    this.activeIndex = index;
    this.activeApplicationAttachmentId = attahcmentId;
    this.imagePath = imagePath;
    this.imageWindowOpened = true;
  }
  public closeWindow() {
    this.imageWindowOpened = false;
  }
  public openSuccessDialog() {
    this.successDialogOpened = true;
  }
  public closeSuccessDialog() {
    this.successDialogOpened = false;
    this.router.navigate(['/fts/my-applications']);
  }
  public openActionDialog(actionId: number, actionName: string) {
    this.actionId = actionId;
    this.actionId == 2 ? this.approvingApplication = true : this.rejectingApplication = true;
    this.actionName = actionName;
    this.actionDialogOpened = true;
  }
  public closeActionDialog() {
    this.approvingApplication = false;
    this.rejectingApplication = false;
    this.actionDialogOpened = false;
  }
  public openTrackingDialog() {
    this.trackingDialogOpened = true;
  }
  public closeTrackingDialog() {
    this.trackingDialogOpened = false;
  }
  public openRequestDialog() {
    this.requestDialogOpened = true;
  }
  public closeRequestDialog() {
    this.requestingFile = false;
    this.changeFileNumber = false;
    this.requestDialogOpened = false;
    this.getApplication(this.applicationId, this.applicationtracking);
  }
  public openLawFileDialog() {
    this.lawFileDialogOpened = true;
  }
  public closeLawFileDialog() {
    this.lawFileDialogOpened = false;
  }
  public getAwesomeDate(date: any) {
    let newDate = new Date(date);
    let dd: any = newDate.getDate(), mm: any = this.makeOrderMonth(newDate.getMonth() + 1), yyyy = newDate.getFullYear();

    return this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
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
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.barcodeImgSrc);
  }
  public dashifyCNIC(cnic: string) {
    if (!cnic) return '';
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
  private handleError(err: any) {
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  ngOnDestroy() {
    this.subscription.unsubscribe();
  }
  public onTabSelect(e) {
    this.selectedTabName = e.heading;
  }

  printApplication() {
    let html = document.getElementById('applicationPrint').innerHTML;
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(`<html><head>
        <style>
            body {
              font-family: -apple-system, BlinkMacSystemFont, 
            'Segoe UI', 'Roboto' , 'Oxygen' , 'Ubuntu' , 'Cantarell' , 'Fira Sans' , 'Droid Sans' , 'Helvetica Neue' ,
              sans-serif !important;
            }
          p {
            margin-top: 0;
            margin-bottom: 1rem !important;
        }.mt-2 {
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
        .application-page {
    
          padding: 50px;
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
        .watermark-hisdu {
          text-align: center;
          position: absolute;
          left: 0;
          width: 100%;
          opacity: 0.25;
        }

        .watermark-hisdu img {
          display: inline-block;
        }

        table.header-pshealth,
        .applicant-information,
        .application-type-detail-preview,
        .attached-document,
        .remarks-preview,
        .info-application-preview,
        table.pshealth {
          border-color: transparent !important;
          width: 100%;
        }

        table.header-pshealth td {
          border-color: transparent !important;
        }

        table.header-pshealth td.gop-logo-a4-header {
          text-align: left;
        }

        table.header-pshealth td.gop-logo-a4-header img {
          display: inline-block;
          width: 134px;
        }

        table.header-pshealth td.pshealth-right-a4-td-header {
          text-align: right;
        }

        table.header-pshealth td.pshealth-right-a4-td-header .pshealth-right-a4-text-header {
          display: inline-block;
          text-align: center;
        }

        table.header-pshealth td.app-type-preview {
          text-align: left;
          width: 100%;
        }

        /* Applicant Information */

        table.applicant-information {
          border-color: transparent !important;
          width: 100%;
        }

        table.applicant-information td.applicant-info-heading,
        table.application-type-detail-preview td.application-type-detail-preview-heading,
        table.remarks-preview td.remarks-heading,
        table.info-application-preview td.info-application-preview-heading,
        table.attached-document td.attached-document-heading {
          text-align: center;
          border: 1px solid black;
        }

        table.applicant-information td.applicant-info-detail-1 {
          width: 20% !important;
        }

        table.applicant-information td.applicant-info-detail-2 {
          width: 40% !important;
        }

        table.applicant-information td.applicant-info-detail-3 {
          width: 10% !important;
        }

        table.applicant-information td.applicant-info-detail-4 {
          width: 30% !important;
        }
        table.applicant-information td.applicant-info-detail-5 {
          width: 15% !important;
        }
        
        table.applicant-information td.applicant-info-detail-6 {
          width: 30% !important;
        }
        
        table.applicant-information td.applicant-info-detail-7 {
          width: 20% !important;
        }
        
        table.applicant-information td.applicant-info-detail-8 {
          width: 35% !important;
        }
        table.info-application-preview td.info-application-preview-left {
          border-left: 1px solid black;
        }

        table.info-application-preview td.info-application-preview-right {
          text-align: center;
          margin: 5px 5px !important;
          border-right: 1px solid black;
          border-left: 1px solid black;
        }

        table.application-route-detail {
          border-color: transparent !important;
          width: 100% !important;
          text-align: center;
        }

        table.application-route-detail td.application-route-detail-header {
          width: 50% !important;
          border: 1px solid black;
        }
        @page 
        {
            size:  auto;   
            margin: 0mm;
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
                <script>
          function printFunc() {
            window.print();
          }
          </script>
      `);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }





}
