import { Component, OnInit, ViewChild, OnDestroy } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Subscription } from 'rxjs/Subscription';
import { User } from '../../../_models/user.class';
import { ApplicationMaster, ApplicationAttachment, ApplicationDocument, ApplicationProfileViewModel, ApplicationLog, ApplicationView } from '../../application-fts/application-fts';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { DomSanitizer } from '@angular/platform-browser';
import { RootService } from '../../../_services/root.service';
import { OrderService } from '../order.service';
import { ActivatedRoute, Router } from '@angular/router';
import { LivePreviewService } from '../../../_services/live-preview.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators/debounceTime';
import { OrderSaveModel } from '../order';
import { cstmdummyOrderReasons } from '../../../_models/cstmdummydata';
import { NotificationService } from '../../../_services/notification.service';
import { TnPService } from '../transfer-n-posting.service';
import { EsrForwardToOfficer } from '../TransferAndPosting/EsrForwardToOfficer.class';
import { ESRDetail, ESRView, ESR } from '../TransferAndPosting/ESR.class';
import { PlatformLocation } from '@angular/common';
@Component({
  selector: 'app-combine-order',
  templateUrl: './combine-order.component.html',
  styles: [`
  
  `]
})
export class CombineOrderComponent implements OnInit, OnDestroy {
  @ViewChild('f', { static: false }) ngForm: NgForm;
  private formChangesSubscription: Subscription;
  public loading: boolean = true;
  public savingApplication: boolean | number = false;
  public uploadingOrder: boolean | number = false;
  public newOrder: boolean = true;
  public lockFileNumber: boolean = false;
  public lockDesignation: boolean = false;
  public lockSignedBy: boolean = false;
  public saveDialogOpened: boolean = false;
  public showOfficerStamp: boolean = false;
  public showStampsOfficerIds: number[] = [172];
  public officerStamp: string = '';
  public afterSubmitStep: number = 0;
  public orderBarcodeStyle: number = 0;
  public savingApplicationText: string = '';
  public user: any;
  public userhfmisCode: string = '';
  public application: any;
  public esr: any;
  public esrView: any;
  public leaveOrder: any;
  public leaveOrderView: any;
  public profiles: any[] = [];
  public divisionForTransfer: any = {};
  public divisionsTransfered: any = {};
  public applicationType: string = 'New Combine Order';
  public errorCombineOrderString: string = '';
  public errorCombineOrderVacancy: number = 0;
  public errorCombineOrderProfile: number = 0;
  public vacantString: string = '';
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];
  public healthFacilities: any[] = [];
  public designationsForTransferOrigional: any[] = [];
  public divisionsForTransferOrigional: any[] = [];
  public districtsForTransferOrigional: any[] = [];
  public tehsilsForTransferOrigional: any[] = [];
  public healthFacilitiesForTransferOrigional: any[] = [];
  public designationsForTransfer: any[] = [];
  public divisionsForTransfer: any[] = [];
  public districtsForTransfer: any[] = [];
  public tehsilsForTransfer: any[] = [];
  public healthFacilitiesForTransfer: any[] = [];
  public loadingHealthFacilities = false;
  public toggleEmployeeInfo = true;
  public toggleEmployeeInfo2 = true;
  public toggleDetailInfo = true;
  public toggleDocuments = true;
  public toggleSignedBy = true;
  public allOfficers: any[] = [];
  public applicationTypes: any[] = [];
  public applicationAttachments: ApplicationAttachment[] = [];
  public applicationDocuments: ApplicationDocument[] = [];
  public signedApplication: ApplicationAttachment;
  public canPrint = false;
  public maxDate = new Date(2000, 1, 1);
  public nowDate = new Date();
  public dateNow: string = '';
  public dropDowns: DropDownsHR = new DropDownsHR();
  public selectedFiltersModel: any = {};
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public previewApplication: number = 2;
  private subscription: Subscription;
  public inputChange: Subject<any>;
  private cnicSubscription: Subscription;
  public checkingVacancy: boolean = false;
  public vacantSeatAvailable: boolean = false;
  public loadingCNIC: boolean = false;
  public isTrackingOrder: boolean = false;
  //
  public profileExist: boolean = false;
  public profileNotExist: boolean = false;
  public profile: any;
  public profile2: any;

  public copyForwardTo: any[] = [];
  public showOrderCopyFwd: boolean = false;
  public isGeneratingOrder: boolean = false;
  public orderType: string = '';
  //
  public outRangeProfile: boolean = false;

  public isHisduAdmin: boolean = false;
  public isCEO: boolean = false;
  public isDGH: boolean = false;
  public isPHFMC: boolean = false;
  public generateOrder: boolean = false;
  public searchingProfiles: boolean = false;
  public selectedCopyForwardTo: any[] = [];
  public esrForwardToOfficers: EsrForwardToOfficer[] = [];
  public markupCC: string = '';
  public esrDetail: ESRDetail = new ESRDetail();

  public editProfile: boolean = false;
  public selectedProfile: any;
  public selectedProfileIndex: number;

  public savingProfile: boolean = false;
  public tableView: boolean = true;
  public searchQuery: string = '';

  public applications: any[] = [];
  public directorates: any[] = [];
  public searchEvent = new Subject<any>();
  public orderResponses: any;

  public profilesList: any[] = [];

  private searchSubcription: Subscription;
  public differentTransfer: boolean = true;
  public orderFieldsDialog: boolean = false;

  public dataItem: any = {};

  constructor(private sanitizer: DomSanitizer,
    private _rootService: RootService,
    private _notificationService: NotificationService,
    private _orderService: OrderService,
    private route: ActivatedRoute,
    private platformLocation: PlatformLocation,
    private router: Router,
    private _tnpService: TnPService,
    private _livePreviewService: LivePreviewService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.user = this._authenticationService.getUser();
    this.userhfmisCode = this._authenticationService.getUserHfmisCode();
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;
    this.initializeProps();
    this.fetchParams();
  }

  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();

    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
    /*  setInterval(() => {
       this.updateLivePreview(true);
     }, 1000); */
    this.getOrderTypes();
    this.handleSearchEvents();
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(200)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  public search(value: string, filter: string) {
    if (!value) return;
    if (filter == 'employee') {
      this.profilesList = [];
      this.searchingProfiles = true;
      this._rootService.searchEmployees({ searchTerm: value }).subscribe((data) => {
        if (data != 404) {
          this.profilesList = data as any[];
        }
        this.searchingProfiles = false;
      }, err => {
        this.searchingProfiles = false;
        console.log(err);
      });
    }
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          let cnic = params['cnic'];
          let typeId = +params['id'];


          this.application = new ApplicationMaster(typeId);
          //this.application.CNIC = cnic;


        } else {
          this.loading = false;
        }
      }
    );
  }
  public setEditorContents() {
    let html = document.getElementById('applicationPrintOld').innerHTML;
    this.application.RawText = html;
  }
  private mapApplicationToOrder() {
    if (this.application) {
      this.getProfileByCNIC();
      this.subscribeToForm();
      this.fetchData();
      this.generateOrderBarcode();
      this.application.DateOfBirth = new Date(this.application.DateOfBirth);
      //Reset remarks for order remarks to shift esr
      this.application.Remarks = '';
      //bind esr type to application type
      //leave to leave
      if (this.application.ApplicationType_Id == 1) {
        this.application.ApplicationType_Id = 5;
      }
      //Transfer to General Transfer
      if (this.application.ApplicationType_Id == 2) {
        this.application.ApplicationType_Id = 4;
      }
      this.setOrderCC(this.application.ApplicationType_Id);
      if (this.application.ApplicationType_Id == 5) {
        this.dropDowns.selectedFiltersModel.leaveType = { LeaveType1: this.application.leaveType, Id: this.application.LeaveType_Id };
        this.application.FromDate = new Date(this.application.FromDate);
        this.application.ToDate = new Date(this.application.ToDate);
      }
      if (this.application.ApplicationType_Id == 4) {
        this.setDesignationForTransfer(this.application.ToHF_Id, this.application.ToHFCode, this.application.Designation_Id);
        if (this.application.ToHFCode && this.application.ToHFCode.length == 19) {
          let toHFCode: string = this.application.ToHFCode;
          this.getHealthFacilitiesForTransfer(toHFCode, true);
          this._rootService.getDivisions(toHFCode.substring(0, 3)).subscribe((res: any) => {
            this.divisionsForTransferOrigional = res;
            this.divisionsForTransfer = this.divisionsForTransferOrigional;
            if (res.length == 1) {
              this.dropDowns.selectedFiltersModel.divisionForTransfer = res[0];
            }
          },
            err => { this.handleError(err); }
          );
          this._rootService.getDistricts(toHFCode.substring(0, 6)).subscribe((res: any) => {
            this.districtsForTransferOrigional = res;
            this.districtsForTransfer = this.districtsForTransferOrigional;
            if (res.length == 1) {
              this.dropDowns.selectedFiltersModel.districtForTransfer = res[0];
            }
          },
            err => { this.handleError(err); }
          );
          this._rootService.getTehsils(toHFCode.substring(0, 9)).subscribe((res: any) => {
            this.tehsilsForTransferOrigional = res;
            this.tehsilsForTransfer = this.tehsilsForTransferOrigional;
            if (res.length == 1) {
              this.dropDowns.selectedFiltersModel.tehsilForTransfer = res[0];
            }
          },
            err => { this.handleError(err); }
          );
        }
        /*   this.lockFileNumber = true; */
        /*  this.lockSignedBy = true; */
        /*   this.dropDowns.selectedFiltersModel.designationForTransfer = { LeaveType1: this.application.leaveType, Id: this.application.LeaveType_Id };
          this.dropDowns.selectedFiltersModel.designationForTransfer = { LeaveType1: this.application.leaveType, Id: this.application.LeaveType_Id };
          this.dropDowns.selectedFiltersModel.designationForTransfer = { LeaveType1: this.application.leaveType, Id: this.application.LeaveType_Id };
          this.dropDowns.selectedFiltersModel.designationForTransfer = { LeaveType1: this.application.leaveType, Id: this.application.LeaveType_Id };
        */
      }
    }
  }
  public setDesignationForTransfer(hf_Id: number, hfmisCode: string, designation_Id: number) {
    this.checkingVacancy = true;
    //this.checkVacancy(hf_Id, hfmisCode, designation_Id);
    this._orderService.getOrderDesignations(hf_Id, hfmisCode).subscribe((res: any) => {
      if (res.vpMasters) {
        this.designationsForTransferOrigional = res.vpMasters;
        this.designationsForTransfer = this.designationsForTransferOrigional;
      }
      this.checkingVacancy = false;
    }, err => {
      this.handleError(err);
    });
  }

  public setDesignationForTransferApp(hf_Id: number, hfmisCode: string, designation_Id: number, app: any) {
    app.checkingVacancy = true;
    //this.checkVacancy(hf_Id, hfmisCode, designation_Id);
    this._orderService.getOrderDesignations(hf_Id, hfmisCode).subscribe((res: any) => {
      if (res.vpMasters) {
        app.designationsForTransfer = res.vpMasters;
      }
      app.checkingVacancy = false;
    }, err => {
      this.handleError(err);
    });
  }
  public unlock() {
    this.lockFileNumber = false;
    this.lockDesignation = false;
    this.lockSignedBy = false;
  }
  private generateOrderBarcode() {
    if (this.application.TrackingNumber) {
      this._orderService.generateOrderBars(this.application.TrackingNumber).subscribe((res: any) => {
        if (res && res.barCode) {
          this.application.barcode = res.barCode;
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  private fetchData() {
    this.orderType = this.application.ApplicationType_Id == 7 || this.application.ApplicationType_Id == 9 ? 'N O T I F I C A T I O N' : 'O R D E R';
    if (this.application.ApplicationType_Id == 2) {
      this.application.ToDept_Id = this.application.Department_Id;
      this.application.toDepartmentName = this.application.DepartmentName;
    }
    this.loadDropdownValues();
    this.authenticateUser();
  }
  private getOrderTypes() {
    this._rootService.getOrderTypes().subscribe((data: any) => {
      this.applicationTypes = data;
      this.applicationType = this.applicationTypes.find(x => x.Id == this.application.ApplicationType_Id).Name;
      this.orderType = this.application.ApplicationType_Id == 7 || this.application.ApplicationType_Id == 9 ? 'N O T I F I C A T I O N' : 'O R D E R';
      /*  this.dropDowns.orderReasons = cstmdummyOrderReasons;
       if (this.application.ApplicationType_Id != 4) {
         this.dropDowns.orderReasons.splice(2);
       } */
      if (this.application.ApplicationType_Id == 2) {
        this.application.ToDept_Id = this.application.Department_Id;
        this.application.toDepartmentName = this.application.DepartmentName;
      }
      this.loadDropdownValues();
      this.authenticateUser();
    });
  }
  private subscribeToForm() {
    this.formChangesSubscription = this.ngForm.form.valueChanges.subscribe(x => {
      this.updateLivePreview(true);
    });
  }
  public updateLivePreview(go: boolean) {
    /*  if (go) {
       let elem = document.getElementById('applicationPrint');
       let html = elem ? elem.innerHTML : '';
       this._livePreviewService.update(html ? html : '');
     }
     else {
       this._livePreviewService.update('');
     } */
  }
  private loadDropdownValues = () => {
    this.getDivisions(this.userhfmisCode);
    this.getDistricts(this.userhfmisCode);
    this.getTehsils(this.userhfmisCode);
    this.getLeaveTypes();
    this.getDesignations();
    this.getDepartments();
    if (this.user.RoleName == 'PHFMC' || this.user.RoleName == 'PHFMC Admin') {
      this.getPandSOfficers('phfmc');
    } else if (this.userhfmisCode.length > 1) {
      this.getPandSOfficers('district');
    } else {
      this.getPandSOfficers('section');
    }
    this.getOrderDocuments(this.application.ApplicationType_Id);
    this.getHealthFacilitiesAtDisposal();
  }
  public authenticateUser = () => {
    console.log(this.user);

    if (this.user.HfmisCode == '0' && (this.user.RoleName == 'Administrator' || this.user.RoleName == 'Hisdu Order Team' || this.user.RoleName == 'Order Generation' || this.user.UserName == 'ordercell')) {
      this.isHisduAdmin = true;
    }
    if (this.user.RoleName == 'PHFMC Admin') {
      this.isPHFMC = true;
    }
    if (this.user.RoleName == 'PHFMC') {
      this.isPHFMC = true;
    }
    if (this.user.RoleName == 'Chief Executive Officer') {
      this.isCEO = true;
    }
    if (this.user.RoleName == 'Districts') {
      this.isCEO = true;
    }
    if (this.user.RoleName == 'DG Health') {
      this.isDGH = true;
    }
  }
  public getProfileByCNIC() {
    this.outRangeProfile = false;
    this._rootService.getProfileByCNIC(this.application.CNIC).subscribe((res: any) => {
      if (res == false) {
        this.outRangeProfile = true;
      }
      if (res == 404) {
        this.profile = null;
        this.profileExist = false;
      } else {
        this.profileExist = true;
        this.profile = res;
        this.mapProfileToApplicant(this.profile);
      }
    }, err => {
      this.handleError(err);
    });
  }
  private getAll = (code: string) => {
    if (code.length <= 1) {
      this._rootService.getDivisions(code).subscribe((res: any) => {
        this.divisionsForTransfer = res;
        if (res.length == 1) {
          this.dropDowns.selectedFiltersModel.divisionsForTransfer = res[0];
        }
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 3) {
      this.resetDropsBelow('division');
      this._rootService.getDistricts(code).subscribe((res: any) => {
        this.districtsForTransfer = res;
        if (res.length == 1) {
          this.dropDowns.selectedFiltersModel.districtForTransfer = res[0];
        }
      },
        err => { this.handleError(err); }
      );
    }
    if (code.length <= 6) {
      this.resetDropsBelow('district');
      this._rootService.getTehsils(code).subscribe((res: any) => {
        this.tehsilsForTransfer = res;
        if (res.length == 1) {
          this.dropDowns.selectedFiltersModel.tehsilForTransfer = res[0];
        }
      },
        err => { this.handleError(err); }
      );
    }
  }
  private getDivisions = (code: string) => {
    this.dropDowns.divisions = [];
    this.dropDowns.divisionsData = [];
    this._rootService.getDivisions(code).subscribe((res: any) => {
      this.dropDowns.divisions = res;
      this.dropDowns.divisionsData = this.dropDowns.divisions.slice();
      if (res.length == 1) {
        this.dropDowns.selectedFiltersModel.division = this.dropDowns.divisionsData[0];
        this.dropDowns.defultFiltersModel.division = this.dropDowns.divisionsData[0];
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
      if (res.length == 1) {
        this.dropDowns.defultFiltersModel.district = this.dropDowns.districtsData[0];
        this.dropDowns.selectedFiltersModel.district = this.dropDowns.districtsData[0];
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
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesAtDisposal = () => {
    this._rootService.getHealthFacilitiesAtDisposal().subscribe((res: any) => {
      if (res) {
        this.dropDowns.healthFacilitiesAtDisposal = res as any[];
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      if (profileHfmisCode) { this.setProfileDefaultValues(profileHfmisCode); }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesApp = (hfmisCode: string, profileHfmisCode?: string, app?: any) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      if (profileHfmisCode) { this.setProfileDefaultValuesApp(profileHfmisCode, app); }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesForTransfer = (hfmisCode, toHfmisCode?: boolean) => {
    this.loadingHealthFacilities = true;
    this._rootService.getHealthFacilities(hfmisCode, 1).subscribe((res: any) => {
      if (res.length > 0) {
        this.healthFacilitiesForTransferOrigional = res;
        this.healthFacilitiesForTransfer = this.healthFacilitiesForTransferOrigional;
        if (this.healthFacilitiesForTransfer.length == 1) {
          this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: this.healthFacilitiesForTransfer[0].Name, Id: this.healthFacilitiesForTransfer[0].Id };
        }
        this.loadingHealthFacilities = false;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesForTransferApp = (hfmisCode, toHfmisCode?: boolean, app?: any) => {
    app.loadingHealthFacilities = true;
    this._rootService.getHealthFacilities(hfmisCode, 1).subscribe((res: any) => {
      if (res.length > 0) {
        app.healthFacilitiesForTransferOrigional = res;
        app.healthFacilitiesForTransfer = app.healthFacilitiesForTransferOrigional;
        if (app.healthFacilitiesForTransfer.length == 1) {
          this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: app.healthFacilitiesForTransfer[0].Name, Id: app.healthFacilitiesForTransfer[0].Id };
        }
        app.loadingHealthFacilities = false;
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getPandSOfficers = (type: string) => {
    this.allOfficers = [];
    this._rootService.getPandSOfficers(type).subscribe((res: any) => {
      if (res) {
        this.allOfficers = res;
        if (this.application.ForwardingOfficer_Id != 0) {
          let officer = this.allOfficers.find(x => x.Id == this.application.ForwardingOfficer_Id);
          if (officer) {
            this.selectedFiltersModel.sectionOfficer = { Id: officer.Id, DesignationName: officer.DesignationName }
          }
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getOrderDocuments = (applicationTypeId: number) => {
    this.applicationDocuments = [];
    this._rootService.getOrderDocuments(applicationTypeId).subscribe((res: any) => {
      this.applicationDocuments = res;
    },
      err => { this.handleError(err); }
    );
  }
  private getDesignations = () => {
    this._rootService.getDesignations().subscribe((res: any) => {
      this.dropDowns.designations = res;
      this.dropDowns.designationsData = this.dropDowns.designations.slice();
      let designations = this.dropDowns.designations as any[];
      if (designations && this.profile) {
        let designation = designations.find(x => x.Id == this.profile.Designation_Id);
        if (designation) {
          this.application.CurrentScale = designation.Scale;
          this.application.fromScale = designation.Scale;
          this.application.Designation_Id = this.profile.Designation_Id ? this.profile.Designation_Id : this.application.Designation_Id;
          this.application.designationName = designation.Name;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
        }
      }
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
  private getLeaveTypes = () => {
    this._rootService.getLeaveTypes().subscribe((res: any) => {
      this.dropDowns.leaveTypes = res;
      this.dropDowns.leaveTypesData = this.dropDowns.leaveTypes.slice();
    },
      err => { this.handleError(err); }
    );
  }
  public dropdownValueChanged = (value, filter) => {

    if (filter == 'type') {
      if (value) {
        this.loading = true;
        this.applicationType = value.Name;
        this.application = new ApplicationMaster(value.Id);
        if (this.application.ApplicationType_Id == 5) {
          this.application.FromDate = new Date();
          this.application.ToDate = new Date();
          this.application.TotalDays = 1;
        }
        this.subscribeCNIC();
        this.fetchData();
        this.getAll(this.userhfmisCode);
        this.loading = false;
        this.newOrder = true;
      }
    }
    if (filter == 'employee') {
      if (+value) {
        var profile = this.profilesList.find(x => x.CNIC == value);
        if (profile && profile.Id) {
          var alreadyExist = this.applications.find(x => x.Profile_Id == profile.Id);
          if (!alreadyExist) {
            this.applications.push(this.mapProfileToApplication(profile));
          }
        }
      }
    }
    if (filter == 'allOfficer') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
      let officerStampExist = this.showStampsOfficerIds.find(x => x == this.application.ForwardingOfficer_Id);
      if (officerStampExist) {
        this.officerStamp = value.Name;
        this.showOfficerStamp = true;
      } else {
        this.showOfficerStamp = false;
      }
    }

    this.applications.forEach(app => {
      if (filter == 'allOfficer') {
        app.ForwardingOfficer_Id = value.Id;
        app.ForwardingOfficerName = value.DesignationName;
        let officerStampExist = this.showStampsOfficerIds.find(x => x == app.ForwardingOfficer_Id);
        if (officerStampExist) {
          this.officerStamp = value.Name;
          this.showOfficerStamp = true;
        } else {
          this.showOfficerStamp = false;
        }
      }
      if (filter == 'division') {
        this.resetDrops(filter);
        this.getDistricts(value.Code);
        this.getTehsils(value.Code);
      }
      if (filter == 'district') {
        this.dropDowns.selectedFiltersModel.district = value;
        this.resetDrops(filter);
        this.getTehsils(value.Code);
      }
      if (filter == 'tehsil') {
        this.resetDrops(filter);
        this.getHealthFacilities(value.Code);
      }
      if (filter == 'healthFacility') {
        app.HfmisCode = value.HfmisCode;
        app.HealthFacility_Id = value.Id;
        app.fromHealthFacility = value.Name;
      }
      if (filter == 'dd') {
        app.ToHFCode = value.Code;
        app.ToHF_Id = null;
        if (value.Code && value.Code.length == 3) {
          this.divisionForTransfer = value;
        }
        this.setOrderCC();
        this.getAll(app.ToHFCode);
        this.getHealthFacilitiesForTransfer(value.Code);
      }
      if (filter == 't') {
        app.ToHFCode = value.Code;
        app.ToHF_Id = null;
        this.resetDropsBelow('tehsil');
        this.getHealthFacilitiesForTransfer(value.Code);
      }

      if (filter == 'healthFacility2') {
        app.ToHFCode = value.HfmisCode;
        app.ToHF_Id = value.Id;
        app.toHealthFacility = value.Name;
        if (app.ApplicationType_Id != 2) {
          this.setDesignationForTransfer(app.ToHF_Id, app.ToHFCode, app.Designation_Id);
        }
      }
      if (filter == 'leaveType') {
        app.LeaveType_Id = value.Id;
        app.leaveType = value.LeaveType1;
      }
      if (filter == 'retirementType') {
        app.RetirementType_Id = value.Id;
        app.retirementTypeName = value.Name;
      }
      if (filter == 'designation') {
        app.Designation_Id = value.Id;
        app.designationName = value.Name;
        app.CurrentScale = value.Scale;
        app.fromScale = value.Scale;
      }
      if (filter == 'department') {
        app.Department_Id = value.Id;
        app.DepartmentName = value.Name;
      }
      if (filter == 'toDepartment') {
        app.ToDept_Id = value.Id;
        app.toDepartmentName = value.Name;
      }
      if (filter == 'toDesignation') {
        if (value.Vacant && value.Vacant > 0) {
          this.vacantSeatAvailable = true;
          app.ToDesignation_Id = value.Desg_Id;
          app.toDesignationName = value.DsgName;
          app.toScale = value.BPS;
        } else {
          this.vacantSeatAvailable = false;
        }
      }
      if (filter == 'orderReason') {
        app.ComplaintType_id = value.Id;
        app.Reason = value.Id == 5 ? '' : value.Name;
      }
    });
  }
  public dropdownValueChangedApp = (value, filter, app) => {
    if (filter == 'hfs') {
      this.application.HealthFacility_Id = value.Id;
      this.application.fromHealthFacility = value.Name;
    }
    if (filter == 'dd') {
      app.ToHFCode = value.Code;
      app.ToHF_Id = null;
      if (value.Code && value.Code.length == 3) {
        this.divisionForTransfer = value;
      }
      this.setOrderCC();
      this.getAll(app.ToHFCode);
      this.getHealthFacilitiesForTransferApp(value.Code, app.ToHFCode, app);
    }
    if (filter == 't') {
      app.ToHFCode = value.Code;
      app.ToHF_Id = null;
      if (value.Code && value.Code.length == 3) {
        this.divisionForTransfer = value;
      }
      this.setOrderCC();
      this.getAll(app.ToHFCode);
      this.getHealthFacilitiesForTransferApp(value.Code, app.ToHFCode, app);
    }
    if (filter == 'healthFacility2') {
      app.ToHFCode = value.HfmisCode;
      app.ToHF_Id = value.Id;
      app.toHealthFacility = value.Name;
      if (app.ApplicationType_Id != 2) {
        this.setDesignationForTransferApp(app.ToHF_Id, app.ToHFCode, app.Designation_Id, app);
      }
    }
    if (filter == 'disposalOffice') {
      app.ToHF_Id = value.Id;
      app.toHealthFacility = value.DesignationName;

    }
    if (filter == 'allOfficer') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
      let officerStampExist = this.showStampsOfficerIds.find(x => x == this.application.ForwardingOfficer_Id);
      if (officerStampExist) {
        this.officerStamp = value.Name;
        this.showOfficerStamp = true;
      } else {
        this.showOfficerStamp = false;
      }
    }
    if (filter == 'employee') {
      if (+value) {
        var profile = this.profilesList.find(x => x.CNIC == value);
        console.log(profile);
        var alreadyExist = this.applications.find(x => x.Profile_Id == profile.Id);
        if (!alreadyExist) {

          this.applications.push(this.mapProfileToApplication(profile));
        }
      }
    }

    if (filter == 'leaveType') {
      app.LeaveType_Id = value.Id;
      app.leaveType = value.LeaveType1;
    }
    if (filter == 'retirementType') {
      this.application.RetirementType_Id = value.Id;
      this.application.retirementTypeName = value.Name;
    }
    if (filter == 'designation') {
      this.application.Designation_Id = value.Id;
      this.application.designationName = value.Name;
      this.application.CurrentScale = value.Scale;
      this.application.fromScale = value.Scale;
    }
    if (filter == 'department') {
      this.application.Department_Id = value.Id;
      this.application.DepartmentName = value.Name;
    }
    if (filter == 'toDepartment') {
      this.application.ToDept_Id = value.Id;
      this.application.toDepartmentName = value.Name;
    }
    if (filter == 'toDesignation') {
      if (value.Vacant && value.Vacant > 0) {
        app.vacantSeatAvailable = true;
        app.ToDesignation_Id = value.Desg_Id;
        app.toDesignationName = value.DsgName;
        app.toScale = value.BPS;
        this.checkVacantSeatsAlready(app.ToHF_Id, app.ToDesignation_Id, value.Vacant);
      } else {
        app.vacantSeatAvailable = false;
      }
    }
    if (filter == 'orderReason') {
      app.ComplaintType_id = value.Id;
      app.Reason = value.Id == 5 ? '' : value.Name;
    }
  }
  public checkVacantSeatsAlready(hf_Id: number, designationId: number, vacant: number) {
    this.errorCombineOrderString = '';
    this.errorCombineOrderVacancy = 0;
    this.errorCombineOrderProfile = 0;
    let count: number = 0;
    this.applications.forEach(app => {
      app.vacancyFill = false;
      if (app.ToHF_Id == hf_Id && app.ToDesignation_Id == designationId) {
        count++;
        if (count > vacant) {
          app.vacancyFill = true;
        }
      }
    });
  }
  private resetDrops = (filter: string) => {
    this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
    this.application.fromHealthFacility = '';
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.district = { Name: 'Select District', Code: '000' };
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '000' };
    }
    if (filter == 'district') {
      this.application.fromHealthFacility = '';
      this.dropDowns.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: '000' };
    }
  }
  private resetDropsBelow = (filter: string) => {
    //this.application.toHealthFacility = '';
    this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: 'Select Health Facility', Id: 0 };
    if (filter == 'division') {
      this.dropDowns.selectedFiltersModel.districtForTransfer = { Name: 'Select District', Code: '000' };
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '000' };
    }
    if (filter == 'district') {
      this.dropDowns.selectedFiltersModel.tehsilForTransfer = { Name: 'Select Tehsil', Code: '000' };
    }
  }
  public searchProfile = () => {
    if (!this.application.CNIC) {
      this.mapProfileToApplicant(null);
      return;
    }
    this._orderService.searchProfile(this.application.CNIC).subscribe((data: any) => {
      if (data == 'Invalid') {
        this.mapProfileToApplicant(null);
      }
      if (data) {
        this.mapProfileToApplicant(data);
      }
    });
  }
  public openInNewTab(link) {
    window.open((this.platformLocation as any).location.origin + '/' + link, '_blank');
  }
  public mapProfileToApplicant = (profile: any) => {
    if (profile) {
      this.application.Profile_Id = profile.Profile_Id ? profile.Profile_Id : profile.Id;
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';
      this.application.FileNumber = profile.FileNumber ? profile.FileNumber : '';


      this.application.Department_Id = profile.Department_Id ? profile.Department_Id : this.application.Department_Id;
      this.application.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;

      let designations = this.dropDowns.designations as any[];
      if (designations) {
        let designation = designations.find(x => x.Id == profile.Designation_Id);
        if (designation) {
          this.application.CurrentScale = designation.Scale;
          this.application.fromScale = designation.Scale;
          this.application.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.application.Designation_Id;
          this.application.designationName = designation.Name;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
        }
      }
      if (profile.HealthFacility_Id && profile.HfmisCode && profile.HfmisCode.length == 19) {
        this.getDivisions(this.userhfmisCode);
        this.getDistricts(this.userhfmisCode);
        this.getTehsils(this.userhfmisCode);
        this.getHealthFacilities(profile.HfmisCode.substring(0, 9), profile.HfmisCode);
      }
      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
      if (this.application.ApplicationType_Id == 13) {
        this.application.ToHFCode = '0350020160010060001';
        this.application.ToHF_Id = 302;
        this.application.toHealthFacility = 'PLGB, Lahore';
      }
      if (this.application.ApplicationType_Id != 4 && this.application.ApplicationType_Id != 6 && this.application.ApplicationType_Id != 8) {
        this.vacantSeatAvailable = true;
      }

      //this.application.TrackingNumber = '00000';
      this.generateOrderBarcode();
    } else {
      let cnicPredict = this.application.CNIC;
      this.application.EmployeeName = '';
      this.application.FatherName = '';
      this.application.DateOfBirth = new Date(2000, 1, 1);
      this.application.Gender = 'Select Gender';
      this.application.MobileNo = '';
      this.application.EMaiL = '';
      this.application.Department_Id = 25;
      this.application.DepartmentName = 'Primary & Secondary Healthcare Department';
      this.application.Designation_Id = 0;
      this.application.designationName = '';
      this.dropDowns.selectedFiltersModel.designation = { Name: 'Select Designation', Id: 0 };
      this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
      this.application.fromHealthFacility = '';
      this.application.HfmisCode = '';
      this.application.HealthFacility_Id = 0;
      this.application.fromHealthFacility = '';
    }
    this.updateLivePreview(true);
  }
  public saveProfile(profile: any) {
    this.savingProfile = true;
    this._orderService.saveProfile(
      {
        Id: profile.Profile_Id,
        MobileNo: profile.MobileNo,
        DateOfBirth: profile.DateOfBirth ? profile.DateOfBirth.toDateString() : null,
        FileNumber: profile.FileNumber,
        EMaiL: profile.EMaiL
      }).subscribe((res: any) => {
        if (res) {
          this._notificationService.notify('success', 'Profile Saved');
          let designations = this.dropDowns.designations as any[];
          if (designations) {
            let designation = designations.find(x => x.Id == profile.Designation_Id);
            if (designation) {
              this.selectedProfile.CurrentScale = designation.Scale;
              this.selectedProfile.fromScale = designation.Scale;
              this.selectedProfile.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.selectedProfile.Designation_Id;
              this.selectedProfile.designationName = designation.Name;
              this.selectedProfile.Designation_Name = designation.Name;
              /*  this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id }; */
            }
          }
          if (profile.HealthFacility_Id) {
            let offices = this.dropDowns.healthFacilities as any[];
            if (offices) {
              let office = offices.find(x => x.Id == profile.HealthFacility_Id);
              if (office) {
                this.selectedProfile.HealthFacility_Id = office.Id;
                this.selectedProfile.HealthFacility = profile.HealthFacility ? profile.HealthFacility : '';
                this.selectedProfile.fromHealthFacility = profile.HealthFacility ? profile.HealthFacility : '';
              }
            }
          }
          this.selectedProfile.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
          this.selectedProfile.MobileNo = profile.MobileNo ? profile.MobileNo : '';
          this.selectedProfile.EMaiL = profile.EMaiL ? profile.EMaiL : '';

          this.selectedProfile.open = true;
          this.applications[this.selectedProfileIndex] = this.selectedProfile;
        }
        this.savingProfile = false;
        this.editProfile = false;
      },
        err => { this.handleError(err); this.savingProfile = false; }
      );
  }
  public selectProfile(profile: any, index: any) {

    this.selectedProfile = profile;
    this.selectedProfileIndex = index;
    let designations = this.dropDowns.designations as any[];
    if (designations) {
      let designation = designations.find(x => x.Id == this.selectedProfile.Designation_Id);
      if (designation) {
        this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Id };
      }
    }
    if (this.selectedProfile.HealthFacility_Id) {
      let offices = this.dropDowns.healthFacilities as any[];
      if (offices) {
        let office = offices.find(x => x.Id == this.selectedProfile.Office_Id);
        if (office) {
          this.dropDowns.selectedFiltersModel.healthFacility = { DesignationName: office.DesignationName, Id: office.Id };
        }
      }
    }
    this.editProfile = true;
  }
  public removeProfile() {
    if (confirm("Are you sure?")) {
      this.applications.splice(this.selectedProfileIndex, 1);
      this.cancelProfile();
    }
  }
  public cancelProfile() {
    this.selectedProfile.open = false;
    this.selectedProfile = {};
    this.editProfile = false;
  }
  public setOrderCC(transferType?: number) {
    if (transferType == 1) {
      let districts = this._tnpService.esr.DistrictFrom === this._tnpService.esr2.DistrictFrom ? this._tnpService.esr.DistrictFrom : this._tnpService.esr.DistrictFrom + ' & ' + this._tnpService.esr2.DistrictFrom;
      this.copyForwardTo = [
        { Name: 'Chief Executive Officer (DHA),', selected: true },
        { Name: 'District Accounts Officer,', selected: true },
        { Name: 'Medical Superintendent,', selected: true },
        { Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Doctor Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    } else if (transferType == 2) {
      this.copyForwardTo = [
        { Name: 'Chief Executive Officer (DHA),', selected: true },
        { Name: 'District Accounts Officer,', selected: true },
        { Name: 'Medical Superintendent,', selected: true },
        { Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Doctor Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType == 3) {
      this.copyForwardTo = [
        { Name: 'Chief Executive Officer (DHA),', selected: true },
        { Name: 'District Accounts Officer,', selected: true },
        { Name: 'Medical Superintendent,', selected: true },
        { Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Doctor Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType == 4) {
      let districts = this._tnpService.esr.DistrictFrom === this._tnpService.esr.DistrictTo ? this._tnpService.esr.DistrictFrom : this._tnpService.esr.DistrictFrom + ' & ' + this._tnpService.esr.DistrictTo;
      this.copyForwardTo = [
        { Name: 'Chief Executive Officer (DHA),', selected: true },
        { Name: 'District Accounts Officer,', selected: true },
        { Name: 'Medical Superintendent,', selected: true },
        { Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Doctor Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType >= 5) {
      this.copyForwardTo = [
        { Name: 'Chief Executive Officer (DHA),', selected: true },
        { Name: 'District Accounts Officer,', selected: true },
        { Name: 'Medical Superintendent,', selected: true },
        { Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true },
        { Name: 'Doctor Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
  }
  public setProfileDefaultValues = (profileHfmisCode) => {
    if (profileHfmisCode) {
      let divisions = this.dropDowns.divisions as any;
      let division = divisions.find(x => x.Code == profileHfmisCode.substring(0, 3));
      if (division) {
        this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
      }
      let districts = this.dropDowns.districts as any;
      let district = districts.find(x => x.Code == profileHfmisCode.substring(0, 6));
      if (district) {
        this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
      }
      let tehsils = this.dropDowns.tehsils as any;
      let tehsil = tehsils.find(x => x.Code == profileHfmisCode.substring(0, 9));
      if (tehsil) {
        this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
      }
      let healthFacilities = this.dropDowns.healthFacilities as any;
      let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
      if (healthFacility) {
        this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
        this.application.HfmisCode = healthFacility.HfmisCode;
        this.application.HealthFacility_Id = healthFacility.Id;
        this.application.fromHealthFacility = healthFacility.Name;
      }
    }
    this.updateLivePreview(true);
  }
  public setProfileDefaultValuesApp = (profileHfmisCode, app) => {
    if (profileHfmisCode) {
      let divisions = this.dropDowns.divisions as any;
      let division = divisions.find(x => x.Code == profileHfmisCode.substring(0, 3));
      if (division) {
        this.dropDowns.selectedFiltersModel.division = { Code: division.Code, Name: division.Name };
      }

      let districts = this.dropDowns.districts as any;
      let district = districts.find(x => x.Code == profileHfmisCode.substring(0, 6));
      if (district) {
        this.dropDowns.selectedFiltersModel.district = { Code: district.Code, Name: district.Name };
      }

      let tehsils = this.dropDowns.tehsils as any;
      let tehsil = tehsils.find(x => x.Code == profileHfmisCode.substring(0, 9));
      if (tehsil) {
        this.dropDowns.selectedFiltersModel.tehsil = { Code: tehsil.Code, Name: tehsil.Name };
      }
      let healthFacilities = this.dropDowns.healthFacilities as any;
      let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
      if (healthFacility) {
        this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
        app.HfmisCode = healthFacility.HfmisCode;
        app.HealthFacility_Id = healthFacility.Id;
        app.fromHealthFacility = healthFacility.Name;
      }
    }
    this.updateLivePreview(true);
  }
  private checkVacancy = (hf_Id: number, hfmisCode: string, designation_Id: number) => {
    this.checkingVacancy = true;
    this._orderService.checkVacancy(hf_Id, hfmisCode, designation_Id).subscribe((res: any) => {
      if (res) {
        this.vacantString = res;
        if (this.vacantString == 'SV') {
          this._rootService.getDesignations().subscribe((res: any) => {
            if (res) {
              let designations = res as any[];
              if (designations.length > 0) {
                let designation = designations.find(x => x.Id == designation_Id);
                if (designation) {
                  if (designation_Id == 802 || designation_Id == 1320 || designation_Id == 2404) {
                    let mo = designations.find(x => x.Id == 802);
                    let wmo = designations.find(x => x.Id == 1320);
                    let moANDwmo = designations.find(x => x.Id == 2404);
                    if (mo && wmo && moANDwmo) {
                      this.designationsForTransfer = [mo, wmo, moANDwmo];
                    }
                  }
                  else {
                    this.designationsForTransfer = [designation];
                    this.lockDesignation = true;
                  }
                  this.dropDowns.defultFiltersModel.designationForTransfer = { Name: 'Select Designation', Id: null };
                  this.dropDowns.selectedFiltersModel.designationForTransfer = { Name: designation.Name, Id: designation.Id };
                  this.application.ToDesignation_Id = designation.Id;
                  this.application.toDesignationName = designation.Name;
                  this.application.toScale = designation.Scale;
                  this.vacantSeatAvailable = true;
                  this.checkingVacancy = false;
                  this.lockDesignation = false;
                }
              }
            }
          },
            err => { this.handleError(err); }
          );
        } else {
          this.dropDowns.selectedFiltersModel.designationForTransfer = { Name: this.vacantString, Id: 0 };
          this.dropDowns.defultFiltersModel.designationForTransfer = { Name: this.vacantString, Id: 0 };
          this.vacantSeatAvailable = false;
          this.checkingVacancy = false;
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  public selectFile(event, document: ApplicationDocument): void {
    let inputValue = event.target;
    let applicationAttachment: ApplicationAttachment = new ApplicationAttachment();
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
    this.updateLivePreview(true);
  }
  public onSubmit() {
    this.savingApplication = true;
    this.generateOrder = false;
    this.isGeneratingOrder = true;
    this.application.RawText = null;
    /*  let html = document.getElementById('applicationPrintOld').innerHTML;
     if (html) {
       this.application.RawText = null;
     } */
    this._orderService.submitApplication(this.application).subscribe((response: any) => {
      if (response.orderResponse) {
        debugger;
        this.application.Id = response.orderResponse.applicationMaster.Id;
        this.esr = response.orderResponse.esr;
        this.esrView = response.orderResponse.esrView;
        this.leaveOrder = response.orderResponse.leaveOrder;
        this.leaveOrderView = response.orderResponse.leaveOrderView;
        this.profiles = response.orderResponse.profiles;
        if (response.barCode) {
          if (this.newOrder && this.esr) {
            this.application.TrackingNumber = this.esr.Id;
          } else {
            this.application.TrackingNumber = response.orderResponse.applicationMaster.TrackingNumber;
          }
          let barcode = response.barCode;
          this.application.barcode = barcode;
          this.application.qrCode = response.qrSrc;
          this.uploadAttachments();
          //Process Further
          this.signedApplication = new ApplicationAttachment();
          this.signedApplication.Document_Id = 1;
          window.scroll(0, 0);
          this.canPrint = true;
          this.afterSubmitStep = 1;

          this._tnpService.esr = this.esr;
          this._tnpService.esr.officerStamp = this.officerStamp;
          this._tnpService.esrView = this.esrView;
          this._tnpService.leaveOrder = this.leaveOrder;
          this._tnpService.leaveOrderView = this.leaveOrderView;
          this._tnpService.esr.DistrictFrom = this.capitalize(this.dropDowns.districtsData[0].Name);
          this._tnpService.esr.DistrictTo = this.dropDowns.selectedFiltersModel.district.Name;
          if (this.application.ApplicationType_Id == 5) {
            this._tnpService.esrView = new ESRView();
            this._tnpService.esrView.SectionOfficer = this.leaveOrder.SignedBy;
            this._tnpService.esrView.TransferTypeID = 5;
          }
          this._tnpService.esrView.AppointmentEffect = this.application.AppointmentEffect;
          this._tnpService.esrView.AppointmentDate = this.application.AdhocExpireDate;
          this.selectedCopyForwardTo = this.copyForwardTo;
          let markupCC: string = '';
          this._tnpService.esr.Remarks = this.application.Remarks;

          this._tnpService.esr.EsrForwardToOfficers = [];
          this.selectedCopyForwardTo.forEach(ccTo => {
            var esrForwardToOfficer = new EsrForwardToOfficer();
            esrForwardToOfficer.Title = ccTo.Name;
            this.esrForwardToOfficers.push(esrForwardToOfficer);
            markupCC += `<li> <span>${ccTo.Name}</span> </li>`;
          });

          this._tnpService.esr.EsrForwardToOfficers = this.esrForwardToOfficers;
          this.markupCC = markupCC;
          this._tnpService.markupCC = markupCC;
          this._tnpService.esrDetail = this.esrDetail;
          this.isGeneratingOrder = false;
          this.generateOrder = true;
        }
      }
      this.savingApplication = false;
    },
      err => { this.handleError(err); }
    );
    /* if (!this.isHisduAdmin && !this.isPHFMC && !this.isDGH) {
      this._tnpService.esr.EsrSectionOfficerID = 30;
      this._tnpService.esr.SectionOfficer = 'Chief Executive Officer';
    } */
  }

  public combineOrder() {
    this.savingApplication = true;
    this.errorCombineOrderString = '';
    this.errorCombineOrderVacancy = 0;
    this.errorCombineOrderProfile = 0;
    this.applications.forEach(app => {
      app.FileNumber = this.application.FileNumber;
      if (app.vacancyFill) {
        this.errorCombineOrderString = 'Error';
        this.errorCombineOrderVacancy++;
        return;
      }
      if (app.missingCount > 0) {
        this.errorCombineOrderString = 'Error';
        this.errorCombineOrderProfile++;
        return;
      }
    });
    if (this.errorCombineOrderVacancy > 0 || this.errorCombineOrderProfile > 0) {
      this.savingApplication = false;
      return;
    }
    this._orderService.submitCombineOrder({ applicationMaster: this.application, applications: this.applications }).subscribe((response: any) => {
      if (response.orderResponses && response.orderResponses.length > 0) {
        response.orderResponse = response.orderResponses.find(x => x.esr.MutualESR_Id == null);
        this.application.Id = response.orderResponse.applicationMaster.Id;
        this.orderResponses = response.orderResponses;
        this.esr = response.orderResponse.esr;
        this.esrView = response.orderResponse.esrView;
        this.esrView.AppointmentEffect = this.application.AppointmentEffect;
        this.esrView.AppointmentDate = new Date(this.application.AdhocExpireDate);
        this.leaveOrder = response.orderResponse.leaveOrder;
        this.leaveOrderView = response.orderResponse.leaveOrderView;
        this.profiles = response.orderResponse.profiles;
        this.orderResponses.forEach(oR => {
          let app = this.applications.find(x => x.CNIC == oR.esrView.CNIC);
          if (app) {
            oR.esrView.AppointmentEffect = app.AppointmentEffect;
            oR.esrView.AppointmentDate = new Date(app.AdhocExpireDate);
            console.log(oR.esrView.AppointmentDate);
          }
        });
        this._tnpService.orderResponses = [];
        this._tnpService.orderResponses = this.orderResponses;
        if (response.barCode) {
          if (this.newOrder && this.esr) {
            this.application.TrackingNumber = this.esr.Id;
          } else {
            this.application.TrackingNumber = response.orderResponse.applicationMaster.TrackingNumber;
          }
          let barcode = response.barCode;
          this.application.barcode = barcode;
          this.application.qrCode = response.qrSrc;
          this.uploadAttachments();
          //Process Further
          this.signedApplication = new ApplicationAttachment();
          this.signedApplication.Document_Id = 1;
          window.scroll(0, 0);
          this.canPrint = true;
          this.afterSubmitStep = 1;
          this._tnpService.esr = this.esr;
          this._tnpService.esr.officerStamp = this.officerStamp;
          this._tnpService.esrView = this.esrView;
          this._tnpService.leaveOrder = this.leaveOrder;
          this._tnpService.leaveOrderView = this.leaveOrderView;
          this._tnpService.esr.DistrictFrom = this.capitalize(this.dropDowns.districtsData[0].Name);
          this._tnpService.esr.DistrictTo = this.dropDowns.selectedFiltersModel.district.Name;
          if (this.application.ApplicationType_Id == 5) {
            this._tnpService.esrView = new ESRView();
            this._tnpService.esrView.SectionOfficer = this.leaveOrder.SignedBy;
            this._tnpService.esrView.TransferTypeID = 5;
          }
          this._tnpService.esrView.AppointmentEffect = this.application.AppointmentEffect;
          this._tnpService.esrView.AppointmentDate = this.application.AdhocExpireDate;
          this.copyForwardTo = [];
          let division = this.divisionForTransfer && this.divisionForTransfer.Code != '0' ? this.divisionForTransfer.Name : '';
          this.copyForwardTo.push({ Name: 'Chief Executive Officer (DHA),', selected: true });
          this.copyForwardTo.push({ Name: 'District Accounts Officer,', selected: true });
          this.copyForwardTo.push({ Name: 'Medical Superintendent,', selected: true });
          this.copyForwardTo.push({ Name: 'PA to Additional Secretary (Admn), Primary & Secondary Healthcare Department', selected: true });
          this.copyForwardTo.push({ Name: 'PA to Deputy Secretary (Est.), Primary & Secondary Healthcare Department', selected: true });
          this.copyForwardTo.push({ Name: 'Deputy Director (HISDU), Primary & Secondary Healthcare Department', selected: true });
          this.copyForwardTo.push({ Name: 'Doctors Concerned', selected: true });
          this.copyForwardTo.push({ Name: 'Master File' + (this.applications.length > 1 ? 's' : ''), selected: true });

          this.selectedCopyForwardTo = this.copyForwardTo;
          let markupCC: string = '';
          this._tnpService.esr.Remarks = this.application.Remarks;
          this._tnpService.esr.EsrForwardToOfficers = [];
          this.selectedCopyForwardTo.forEach(ccTo => {
            var esrForwardToOfficer = new EsrForwardToOfficer();
            esrForwardToOfficer.Title = ccTo.Name;
            this.esrForwardToOfficers.push(esrForwardToOfficer);
            markupCC += `<li> <span>${ccTo.Name}</span> </li>`;
          });
          this._tnpService.esr.EsrForwardToOfficers = this.esrForwardToOfficers;
          this.markupCC = markupCC;
          this._tnpService.markupCC = markupCC;
          this._tnpService.esrDetail = this.esrDetail;
          this.isGeneratingOrder = false;
          this.generateOrder = true;
        }
      }
      this.savingApplication = false;
    },
      err => { this.handleError(err); }
    );
  }
  public setDirectorates() {
    this.applications.forEach(app => {
      if (app && app.Directorate_Name) {
        let exist = this.directorates.find(x => x == app.Directorate_Name);
        if (!exist) {
          this.directorates.push(app.Directorate_Name);
        }
      }
      if (app && app.ToDirectorate_Name) {
        let exist = this.directorates.find(x => x == app.ToDirectorate_Name);
        if (!exist) {
          this.directorates.push(app.ToDirectorate_Name);
        }
      }
    });
  }


  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.divisionsForTransfer = this.divisionsForTransferOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.districtsForTransfer = this.districtsForTransferOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.tehsilsForTransfer = this.tehsilsForTransferOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }

  public handleFilterApp = (value, filter, app) => {
    if (filter == 'division') {
      app.designationsForTransfer = app.designationsForTransferOrigional.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }

  }
  public openOrderFieldWindow(app) {
    this.dataItem = {};
    this.dataItem = app;
    this.orderFieldsDialog = true;
  }
  public closeOrderFieldWindow() {
    this.dataItem = {};
    this.orderFieldsDialog = false;
  }
  public uploadAttachments() {
    if (this.applicationAttachments.length > 0) {
      this._orderService.uploadApplicationAttachments(this.applicationAttachments, this.application.Id).subscribe((response) => {
        if (response) {
          this.savingApplication = false;
        }
      }, err => {
        this.handleError(err);
      });
    }
  }
  uploadSignedCopy(event) {
    let inputValue = event.target;
    this.signedApplication.files = inputValue.files;
    this.signedApplication.attached = true;
  }
  uploadAndConfirmOrder() {
    // upload signed copy
    this.uploadingOrder = true;
    this._orderService.uploadSignedOrder(this.signedApplication, this.application.Id, this.esr.Id, this.leaveOrder.Id).subscribe((response) => {
      if (response) {
        this._orderService.changeSystemWithOrder(this.esr.Id).subscribe((response: any) => {
          if (response) {
            this.uploadingOrder = false;
            this._notificationService.notify('success', 'Order Saved');
            this.openWindow();
          } else {
            this._notificationService.notify('danger', 'Something went wrong.');
          }
        },
          err => { this.handleError(err); }
        );
      }
    }, err => {
      this.handleError(err);
    });
  }
  public changeSystemWithOrder = () => {
    this._orderService.changeSystemWithOrder(this.esr.Id).subscribe((response: any) => {
      if (response) {
        this.uploadingOrder = false;
        this._notificationService.notify('success', 'Order Saved');
        this.openWindow();
      } else {
        this._notificationService.notify('danger', 'Something went wrong.');
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
  public leaveInputsChangedApp(type: number, app) {
    if (type == 1) {
      if (app.FromDate && app.ToDate) {
        this._rootService.calcDate(app.FromDate.toDateString(), app.ToDate.toDateString(), 0).subscribe((x: number) => {
          app.TotalDays = x;
        });
      }
    } else if (type == 2) {
      this._rootService.calcDate(app.FromDate.toDateString(), 'noDate', app.TotalDays).subscribe((x: any) => {
        app.ToDate = new Date(x);
      });
    }
  }
  /* proceed(transferType?: number) {
    this.isGeneratingOrder = true;
    this._tnpService.esr.TransferTypeID = this.transferTypesModel;
    if (transferType == 1) {
      let districts = this._tnpService.esr.DistrictFrom === this._tnpService.esr2.DistrictFrom ? this._tnpService.esr.DistrictFrom : this._tnpService.esr.DistrictFrom + ' & ' + this._tnpService.esr2.DistrictFrom;
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    } else if (transferType == 2) {
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType == 3) {
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType == 4) {
      let districts = this._tnpService.esr.DistrictFrom === this._tnpService.esr.DistrictTo ? this._tnpService.esr.DistrictFrom : this._tnpService.esr.DistrictFrom + ' & ' + this._tnpService.esr.DistrictTo;
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    else if (transferType >= 5) {
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    }
    this.showOrderCopyFwd = true;
    if (transferType != 5) {
      this.generalTransfer();
    } else if (transferType == 5) {
      this.leaveInputsChanged(this.orderDate, 4);
      this.generateLeaveOrder();
    }
  }

  generalTransfer() {
    if (!this.isHisduAdmin && !this.isPHFMC && !this.isDGH) {
      this.selectedSectionOfficer = 30;
      this._tnpService.esr.EsrSectionOfficerID = 30;
      this._tnpService.esr.SectionOfficer = 'Chief Executive Officer';
    }
    this.selectedCopyForwardTo = this.copyForwardTo;
    this.generateOrder = false;
    let markupCC: string = '';
    this._tnpService.esr.Remarks = this.remarks;
    if (this.transferTypesModel == 4 || this.transferTypesModel == 6 || this.transferTypesModel == 8 || this.transferTypesModel == 10) {
      this._tnpService.esr.MutualESR_Id = this.reason_Id;
    }
    if (this.transferTypesModel == 2) {
      this._tnpService.esr.Remarks = this.selectedDistrict;
    }
    this._tnpService.esr.EsrForwardToOfficers = [];
    this.selectedCopyForwardTo.forEach(ccTo => {
      var esrForwardToOfficer = new EsrForwardToOfficer();
      esrForwardToOfficer.Title = ccTo.Name;
      this.esrForwardToOfficers.push(esrForwardToOfficer);
      markupCC += `<li> <span>${ccTo.Name}</span> </li>`;
    });
    this._tnpService.esr.EsrForwardToOfficers = this.esrForwardToOfficers;
    this.markupCC = markupCC;
    this._tnpService.markupCC = markupCC;
    this._tnpService.esrDetail = this.esrDetail;
    this._mainService.addESR([this._tnpService.esr, this._tnpService.esr2], this.user.HfmisCode).subscribe((x) => {
      this.barcodeSrc = x.imgSrc;
      this._tnpService.esr.Id = x.esr.Id;
      if (this._tnpService.transferType == 9 || this._tnpService.transferType == 11) {
        this.esrDetail.Master_Id = x.esr.Id;
        this._mainService.addESRDetail(this.esrDetail).subscribe((x) => {

        });
      }
      if (this.files.length > 0) {
        this._mainService.uploadNotingFile(this.files, this.searchedProfile.CNIC, x.esr.Id).subscribe((x) => {
          if (x.result) {

          }
        });
      }
      this.isGeneratingOrder = false;
      this.generateOrder = true;
    });
  } */
  public mapProfileToApplication = (profile: any) => {
    let app: any = {};
    app.ApplicationType_Id = this.application.ApplicationType_Id;
    if (profile) {
      app.employeeProfile = profile;
      app.missingCount = 0;
      app.vacantString = '';
      app.checkingVacancy = false;
      app.vacantSeatAvailable = false;
      app.Profile_Id = profile.Profile_Id ? profile.Profile_Id : profile.Id;
      app.CNIC = profile.CNIC ? profile.CNIC : '';
      app.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      app.FatherName = profile.FatherName ? profile.FatherName : '';
      app.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      app.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      app.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      app.EMaiL = profile.EMaiL ? profile.EMaiL : '';

      if (profile.FileNumber) {
        app.FileNumber = profile.FileNumber;
      } /* else {
        app.missingCount++;
      } */
      if (profile.EmpMode_Id) {
        app.EmpMode_Id = profile.EmpMode_Id;
      } else {
        app.missingCount++;
      }
      if (profile.Status_Id) {
        app.EmpStatus_Id = profile.Status_Id;
      } else {
        app.missingCount++;
      }
      app.Department_Id = profile.Department_Id ? profile.Department_Id : app.Department_Id;
      app.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;

      let designations = this.dropDowns.designations as any[];
      if (designations) {
        let designation = designations.find(x => x.Id == profile.Designation_Id);
        if (designation) {
          app.CurrentScale = designation.Scale;
          app.fromScale = designation.Scale;
          app.Designation_Id = profile.Designation_Id ? profile.Designation_Id : app.Designation_Id;
          app.designationName = designation.Name;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
        }
      }
      if (profile.HealthFacility_Id && profile.HfmisCode && profile.HfmisCode.length == 19) {
        /*    this.getDivisions(this.userhfmisCode);
           this.getDistricts(this.userhfmisCode);
           this.getTehsils(this.userhfmisCode); */
        this.getHealthFacilitiesApp(profile.HfmisCode.substring(0, 9), profile.HfmisCode, app);
      }
      if (app.ApplicationType_Id == 3) {
        app.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        app.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }
      if (app.ApplicationType_Id == 13) {
        app.ToHFCode = '0350020160010060001';
        app.ToHF_Id = 302;
        app.toHealthFacility = 'PLGB, Lahore';
      }
      if (app.ApplicationType_Id != 4 && app.ApplicationType_Id != 6 && app.ApplicationType_Id != 8) {
        app.vacantSeatAvailable = true;
      }

      if (app.ApplicationType_Id == 5) {
        app.FromDate = new Date();
        app.ToDate = new Date();
        app.TotalDays = 1;
      }
      //app.TrackingNumber = '00000';
      this.generateOrderBarcode();
    } else {
      let cnicPredict = app.CNIC;
      app.EmployeeName = '';
      app.FatherName = '';
      app.DateOfBirth = new Date(2000, 1, 1);
      app.Gender = 'Select Gender';
      app.MobileNo = '';
      app.EMaiL = '';
      app.Department_Id = 25;
      app.DepartmentName = 'Primary & Secondary Healthcare Department';
      app.Designation_Id = 0;
      app.designationName = '';
      app.fromHealthFacility = '';
      app.HfmisCode = '';
      app.HealthFacility_Id = 0;
      app.fromHealthFacility = '';
    }
    return app;
  }
  public subscribeCNIC() {
    this.inputChange = new Subject();
    this.cnicSubscription = this.inputChange.pipe(debounceTime(400)).subscribe((query) => {
      this.searchProfile();
    });
  }



  private handleError(err: any) {
    this.checkingVacancy = false;
    this.savingApplication = false;
    this.canPrint = false;
    this.uploadingOrder = false;
    this.loading = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public barcodeSrc() {
    return this.sanitizer.bypassSecurityTrustUrl(this.application.barcode);
  }
  public setPreview() {
    console.log(this.previewApplication);
    this.previewApplication = this.previewApplication + 1;
    if (this.previewApplication == 3) this.previewApplication = 0;
    console.log(this.previewApplication);

  }
  public closeWindow() {
    this.saveDialogOpened = false;
    this.router.navigate(['/application']);
  }

  public openWindow() {
    this.saveDialogOpened = true;
  }
  public capitalize(val: string) {
    if (!val) return '';
    return val.toUpperCase();
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
  ngOnDestroy() {
    this.updateLivePreview(false);
    if (this.subscription) {
      this.subscription.unsubscribe();

    }
    if (this.formChangesSubscription) {
      this.formChangesSubscription.unsubscribe();

    }
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
            background: #424242;
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
      this.afterSubmitStep = 2;

      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
}