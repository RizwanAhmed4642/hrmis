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
@Component({
  selector: 'app-mutual-transfer',
  templateUrl: './mutual-transfer.component.html',
  styles: [`
  
  `]
})
export class MutualTransferComponent implements OnInit, OnDestroy {
  @ViewChild('f', { static: true }) ngForm: NgForm;
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
  public showStampsOfficerIds: any[] = [];
  public officerStamp: string = '';
  public afterSubmitStep: number = 0;
  public orderBarcodeStyle: number = 0;
  public savingApplicationText: string = '';
  public user: any;
  public userhfmisCode: string = '';
  public application: any;
  public application2: any;
  public esr: any;
  public esrView: ESRView;
  public esr2: any;
  public esrView2: ESRView;
  public leaveOrder: any;
  public leaveOrderView: any;
  public profiles: any[] = [];
  public applicationType: string = 'New Order';
  public vacantString: string = '';
  public divisions: any[] = [];
  public districts: any[] = [];
  public tehsils: any[] = [];
  public healthFacilities: any[] = [];
  public designationsForTransfer: any[] = [];
  public divisionsForTransfer: any[] = [];
  public districtsForTransfer: any[] = [];
  public tehsilsForTransfer: any[] = [];
  public healthFacilitiesForTransfer: any[] = [];
  public loadingHealthFacilities = false;
  public toggleEmployeeInfo = false;
  public toggleEmployeeInfo2 = false;
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
  public fileChosenLoading: boolean = false;
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
  public selectedCopyForwardTo: any[] = [];
  public esrForwardToOfficers: EsrForwardToOfficer[] = [];
  public markupCC: string = '';
  public esrDetail: ESRDetail = new ESRDetail();
  public db;
  constructor(private sanitizer: DomSanitizer,
    private _rootService: RootService,
    private _notificationService: NotificationService,
    private _orderService: OrderService,
    private route: ActivatedRoute,
    private router: Router,
    private _tnpService: TnPService,
    private _livePreviewService: LivePreviewService,
    private _authenticationService: AuthenticationService) { }

  ngOnInit() {
    this.insertEditorScript();
    this.user = this._authenticationService.getUser();
    this.userhfmisCode = this._authenticationService.getUserHfmisCode();
    this.selectedFiltersModel = this.dropDowns.selectedFiltersModel;
    this.initializeProps();
    this.fetchParams();
  }
  insertEditorScript() {
    let script = document.querySelector('script[src="https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js"]');
    if (!script) {
      var externalScript = document.createElement('script');
      externalScript.setAttribute('src', 'https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js');
      document.head.appendChild(externalScript);
    }
  }
  private initializeProps() {
    let today = new Date();
    let dd: any = today.getDate(), mm: any = this.makeOrderMonth(today.getMonth() + 1), yyyy = today.getFullYear();

    this.dateNow = this.makeOrderDate(dd) + ', ' + mm + ' ' + yyyy;
    /*  setInterval(() => {
       this.updateLivePreview(true);
     }, 1000); */
  }

  private fetchParams() {
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.hasOwnProperty('id')) {
          let cnic = params['cnic'];
          let cnic2 = params['cnic2'];
          let typeId = +params['id'];
          let appId = +params['appid'];
          let tracking = +params['trackingid'];

          if (cnic && typeId && appId && tracking) {
            this._orderService.getApplication(appId, tracking).subscribe((res: any) => {
              if (res && res.application) {
                this.application = res.application as ApplicationView;
                this.mapApplicationToOrder();
                this.loading = false;
                this.newOrder = false;
              }
            }, err => {
              this.handleError(err);
            })
          } else {
            this.application = new ApplicationMaster(typeId);
            this.application.CNIC = cnic;
            if (this.application.ApplicationType_Id == 5) {
              this.application.FromDate = new Date();
              this.application.ToDate = new Date();
              this.application.TotalDays = 1;
            }
            if (cnic2) {
              this.application2 = new ApplicationMaster(typeId);
              this.application2.CNIC = cnic2;
            }
            this.subscribeCNIC();
            this.getProfileByCNIC();
            this.subscribeToForm();
            this.inputChange.next(cnic);
            this.fetchData();
            this.getAll(this.userhfmisCode);
            this.setOrderCC(this.application.ApplicationType_Id);
            this.loading = false;
            this.newOrder = true;
          }
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
            this.divisionsForTransfer = res;
            if (res.length == 1) {
              this.dropDowns.selectedFiltersModel.divisionForTransfer = res[0];
            }
          },
            err => { this.handleError(err); }
          );
          this._rootService.getDistricts(toHFCode.substring(0, 6)).subscribe((res: any) => {
            this.districtsForTransfer = res;
            if (res.length == 1) {
              this.dropDowns.selectedFiltersModel.districtForTransfer = res[0];
            }
          },
            err => { this.handleError(err); }
          );
          this._rootService.getTehsils(toHFCode.substring(0, 9)).subscribe((res: any) => {
            this.tehsilsForTransfer = res;
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
        this.designationsForTransfer = res.vpMasters;
      }
      this.checkingVacancy = false;
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
    this.getOfficerStampId();
    this.getDivisions(this.userhfmisCode);
    this.getDistricts(this.userhfmisCode);
    this.getTehsils(this.userhfmisCode);
    this.getLeaveTypes();
    this.getDesignations();
    this.getDepartments();
    this.getDisposalOf();
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

    if (this.user.HfmisCode == '0' && (this.user.RoleName == 'Administrator' || this.user.RoleName == 'Hisdu Order Team' || this.user.UserName == 'ordercell')) {
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
    if (this.application2 && this.application2.CNIC) {
      this._rootService.getProfileByCNIC(this.application2.CNIC).subscribe((res2: any) => {
        if (res2 == false) {
          this.outRangeProfile = true;
        }
        if (res2 == 404) {
          this.profile2 = null;
          this.profileExist = false;
        } else {
          this.profileExist = true;
          this.profile2 = res2;
          this.mapProfileToApplicant2(this.profile2);
        }
      }, err => {
        this.handleError(err);
      });
    }
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
  private getHealthFacilities = (hfmisCode: string, profileHfmisCode?: string, profileHfmisCode2?: boolean) => {
    this._rootService.getHealthFacilities(hfmisCode).subscribe((res: any) => {
      this.dropDowns.healthFacilities = res;
      this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();
      if (profileHfmisCode && !profileHfmisCode2) { this.setProfileDefaultValues(profileHfmisCode); }
      if (profileHfmisCode2) { this.setProfileDefaultValues2(profileHfmisCode); }
    },
      err => { this.handleError(err); }
    );
  }
  private getHealthFacilitiesForTransfer = (hfmisCode, toHfmisCode?: boolean) => {
    this.loadingHealthFacilities = true;
    this._rootService.getHealthFacilities(hfmisCode, this.application.ToDept_Id).subscribe((res: any) => {
      if (res.length > 0) {
        this.healthFacilitiesForTransfer = res;
        if (this.healthFacilitiesForTransfer.length == 1) {
          this.dropDowns.selectedFiltersModel.healthFacilityForTransfer = { Name: this.healthFacilitiesForTransfer[0].Name, Id: this.healthFacilitiesForTransfer[0].Id };
        }
        this.loadingHealthFacilities = false;
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
      if (designations && this.profile2) {
        let designation = designations.find(x => x.Id == this.profile2.Designation_Id);
        if (designation) {
          this.application2.CurrentScale = designation.Scale;
          this.application2.fromScale = designation.Scale;
          this.application2.Designation_Id = this.profile2.Designation_Id ? this.profile2.Designation_Id : this.application2.Designation_Id;
          this.application2.designationName = designation.Name;
        }
      }
    },
      err => { this.handleError(err); }
    );
  }
  private getOfficerStampId = () => {
    this._orderService.getOfficerStampId().subscribe((res: any) => {
      this.showStampsOfficerIds = res;
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
  private getDisposalOf = () => {
    this._rootService.getDisposalOf().subscribe((res: any) => {
      this.dropDowns.disposalOf = res;
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
      this.application.HfmisCode = value.HfmisCode;
      this.application.HealthFacility_Id = value.Id;
      this.application.fromHealthFacility = value.Name;
    }
    if (filter == 'dd') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.getAll(this.application.ToHFCode);
    }
    if (filter == 't') {
      this.application.ToHFCode = value.Code;
      this.application.ToHF_Id = null;
      this.resetDropsBelow('tehsil');
      this.getHealthFacilitiesForTransfer(value.Code);
    }

    if (filter == 'healthFacility2') {
      this.application.ToHFCode = value.HfmisCode;
      this.application.ToHF_Id = value.Id;
      this.application.toHealthFacility = value.Name;
      if (this.application.ApplicationType_Id != 2) {
        this.setDesignationForTransfer(this.application.ToHF_Id, this.application.ToHFCode, this.application.Designation_Id);
      }
    }
    if (filter == 'allOfficer') {
      this.application.ForwardingOfficer_Id = value.Id;
      this.application.ForwardingOfficerName = value.DesignationName;
      let officerStampExist = this.showStampsOfficerIds.find(x => x.Officer_Id == this.application.ForwardingOfficer_Id);
      if (officerStampExist) {
        this.officerStamp = value.Name;
        this.showOfficerStamp = true;
      } else {
        this.showOfficerStamp = false;
      }
    }

    if (filter == 'leaveType') {
      this.application.LeaveType_Id = value.Id;
      this.application.leaveType = value.LeaveType1;
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
      if (this.application.ToDept_Id != 7) {
        this.application.toHealthFacility = this.application.toDepartmentName;
      }
    }
    if (filter == 'toDesignation') {
      if (value.Vacant && value.Vacant > 0) {
        this.vacantSeatAvailable = true;
        this.application.ToDesignation_Id = value.Desg_Id;
        this.application.toDesignationName = value.DsgName;
        this.application.toScale = value.BPS;
      } else {
        this.vacantSeatAvailable = false;
      }
    }
    if (filter == 'orderReason') {
      this.application.ComplaintType_id = value.Id;
      this.application.Reason = value.Id == 5 ? '' : value.Name;
    }
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
  public mapProfileToApplicant = (profile: any) => {
    if (profile) {
      this.application.Profile_Id = profile.Profile_Id ? profile.Profile_Id : profile.Id;
      this.application.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application.EMaiL = profile.EMaiL ? profile.EMaiL : '';


      this.application.Department_Id = profile.Department_Id ? profile.Department_Id : this.application.Department_Id;
      this.application.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;
      this.application.EmpMode_Id = profile.EmpMode_Id;
      this.application.EmpStatus_Id = profile.EmpStatus_Id;

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
        this.getHealthFacilities(profile.HfmisCode.substring(0, 9), profile.HfmisCode, false);
      }
      if (this.application.ApplicationType_Id == 3) {
        this.application.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
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
  public mapProfileToApplicant2 = (profile: any) => {

    if (profile) {
      this.application2.Profile_Id = profile.Profile_Id ? profile.Profile_Id : profile.Id;
      this.application2.EmployeeName = profile.EmployeeName ? profile.EmployeeName : '';
      this.application2.FatherName = profile.FatherName ? profile.FatherName : '';
      this.application2.DateOfBirth = profile.DateOfBirth ? new Date(profile.DateOfBirth) : new Date(2000, 1, 1);
      this.application2.Gender = profile.Gender ? profile.Gender : 'Select Gender';
      this.application2.MobileNo = profile.MobileNo ? profile.MobileNo : '';
      this.application2.EMaiL = profile.EMaiL ? profile.EMaiL : '';


      this.application2.Department_Id = profile.Department_Id ? profile.Department_Id : this.application2.Department_Id;
      this.application2.DepartmentName = profile.Department_Id == 28 ? 'Specialized Healthcare & Medical Education' : 'Primary & Secondary Healthcare Department';
      this.dropDowns.selectedFiltersModel.departmentDefault = profile.Department_Id == 28 ? { Name: 'Specialized Healthcare & Medical Education', Id: 28 } : profile.Department_Id == 25 ? { Name: 'Primary & Secondary Healthcare Department', Id: 25 } : this.dropDowns.selectedFiltersModel.department;
      this.application2.EmpMode_Id = profile.EmpMode_Id;
      this.application2.EmpStatus_Id = profile.EmpStatus_Id;

      let designations = this.dropDowns.designations as any[];
      if (designations) {
        let designation = designations.find(x => x.Id == profile.Designation_Id);
        if (designation) {
          this.application2.CurrentScale = designation.Scale;
          this.application2.fromScale = designation.Scale;
          this.application2.Designation_Id = profile.Designation_Id ? profile.Designation_Id : this.application2.Designation_Id;
          this.application2.designationName = designation.Name;
          this.dropDowns.selectedFiltersModel.designation = { Name: designation.Name, Id: designation.Name.Id };
        }
      }
      if (profile.HealthFacility_Id && profile.HfmisCode && profile.HfmisCode.length == 19) {
        this.getDivisions(this.userhfmisCode);
        this.getDistricts(this.userhfmisCode);
        this.getTehsils(this.userhfmisCode);
        this.getHealthFacilities(profile.HfmisCode.substring(0, 9), profile.HfmisCode, true);
      }
      if (this.application2.ApplicationType_Id == 3) {
        this.application2.CurrentScale = profile.CurrentGradeBPS ? profile.CurrentGradeBPS : 0;
        this.application2.SeniorityNumber = profile.SeniorityNo ? profile.SeniorityNo : '';
      }

      if (this.application2.ApplicationType_Id != 4 && this.application2.ApplicationType_Id != 6 && this.application2.ApplicationType_Id != 8) {
        this.vacantSeatAvailable = true;
      }
      //this.application2.TrackingNumber = '00000';
      this.generateOrderBarcode();
    } else {
      let cnicPredict = this.application2.CNIC;
      this.application2.EmployeeName = '';
      this.application2.FatherName = '';
      this.application2.DateOfBirth = new Date(2000, 1, 1);
      this.application2.Gender = 'Select Gender';
      this.application2.MobileNo = '';
      this.application2.EMaiL = '';
      this.application2.Department_Id = 25;
      this.application2.DepartmentName = 'Primary & Secondary Healthcare Department';
      this.application2.Designation_Id = 0;
      this.application2.designationName = '';
      this.dropDowns.selectedFiltersModel.designation = { Name: 'Select Designation', Id: 0 };
      this.dropDowns.selectedFiltersModel.healthFacility = { Name: 'Select Health Facility', Id: 0 };
      this.application2.fromHealthFacility = '';
      this.application2.HfmisCode = '';
      this.application2.HealthFacility_Id = 0;
      this.application2.fromHealthFacility = '';
    }
    this.updateLivePreview(true);
  }
  public setOrderCC(transferType?: number) {
    if (transferType == 1) {
      let districts = this._tnpService.esr.DistrictFrom === this._tnpService.esr2.DistrictFrom ? this._tnpService.esr.DistrictFrom : this._tnpService.esr.DistrictFrom + ' & ' + this._tnpService.esr2.DistrictFrom;
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'Director General Health Services Punjab', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
        { Name: 'Master File', selected: true }
      ];
    } else if (transferType == 2) {
      this.copyForwardTo = [
        { Name: 'PSO to Secretary P&S Healthcare Department', selected: true },
        { Name: 'Director General Health Services Punjab', selected: true },
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
        { Name: 'Director General Health Services Punjab', selected: true },
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
        { Name: 'Director General Health Services Punjab', selected: true },
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
        { Name: 'Director General Health Services Punjab', selected: true },
        { Name: 'PA to Additional Secretary Concerned', selected: true },
        { Name: 'PA to Deputy Secretary Concerned', selected: true },
        { Name: 'Senior Data Processor Concerned', selected: true },
        { Name: 'Officer / Official Concerned', selected: true },
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
        if (this.application2) { this.application2.toHealthFacility = healthFacility.Name; }
        this._tnpService.healthFacilityFromName = this.application.fromHealthFacility;
      }
    }
    this.updateLivePreview(true);
  }
  public setProfileDefaultValues2 = (profileHfmisCode) => {
    if (profileHfmisCode) {
      /*  let divisions = this.dropDowns.divisions as any;
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
       } */
      let healthFacilities = this.dropDowns.healthFacilities as any;
      let healthFacility = healthFacilities.find(x => x.HfmisCode == profileHfmisCode);
      if (healthFacility) {
        //this.dropDowns.selectedFiltersModel.healthFacility = { Id: healthFacility.Id, HfmisCode: healthFacility.HfmisCode, Name: healthFacility.Name };
        this.application2.HfmisCode = healthFacility.HfmisCode;
        this.application2.HealthFacility_Id = healthFacility.Id;
        this.application2.fromHealthFacility = healthFacility.Name;
        if (this.application) { this.application.toHealthFacility = healthFacility.Name; }
        this._tnpService.healthFacilityFromName2 = this.application2.fromHealthFacility;
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
    this.fileChosenLoading = true;
    this.savingApplication = true;
    let inputValue = event.target;
    let applicationAttachment: ApplicationAttachment = new ApplicationAttachment();
    applicationAttachment.Document_Id = document.Id;
    applicationAttachment.documentName = document.Name;
    applicationAttachment.files = inputValue.files;
    this.applicationAttachments.push(applicationAttachment);
    this.applicationDocuments.find(x => x.Id == document.Id).attached = true;
    this.fileChosenLoading = false;
    this.savingApplication = false;
    /* this.updateLivePreview(true); */
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
    this._orderService.submitMutualOrder({ applicationMaster : this.application, applicationMaster2: this.application2 }).subscribe((response: any) => {
      if (response.orderResponse) {
        console.log(response);
        this.application.Id = response.orderResponse.applicationMaster.Id;
        this.esr = response.orderResponse.esr;
        this.esr2 = response.orderResponse.esr2;
        this.esrView = response.orderResponse.esrView;
        this.esrView2 = response.orderResponse.esrView2;
        this.profiles = response.orderResponse.profiles;
        if (response.barCode) {
          if (this.newOrder && this.esr) {
            this.application.TrackingNumber = this.esr.Id;
          } else {
            this.application.TrackingNumber = response.orderResponse.applicationMaster.TrackingNumber;
          }
          let barcode = response.barCode;
          this.application.barcode = barcode;
          this.uploadAttachments();
          //Process Further
          this.signedApplication = new ApplicationAttachment();
          this.signedApplication.Document_Id = 1;
          window.scroll(0, 0);
          this.canPrint = true;
          this.afterSubmitStep = 1;

          this._tnpService.esr = this.esr;
          this._tnpService.esr2 = this.esr2;
          this._tnpService.esr.officerStamp = this.officerStamp;
          this._tnpService.esrView = this.esrView;
          this._tnpService.esrView2 = this.esrView2;
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

  /*insertEditorScript() {
    var externalScript = document.createElement('script');
    externalScript.setAttribute('src', 'https://cdn.ckeditor.com/4.5.11/full-all/ckeditor.js');
    document.head.appendChild(externalScript);

      var createCKE = window.setInterval(function () {
       if (CKEDITOR) {
         createEditor();
         clearInterval(createCKE);
       }
     }, 100);
  } */

  public uploadAttachments() {
    if (this.applicationAttachments.length > 0) {
      this._orderService.uploadApplicationAttachments(this.applicationAttachments, this.esr.Id).subscribe((response) => {
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
    this.subscription.unsubscribe();
    this.formChangesSubscription.unsubscribe();
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
        Service Delivery Unit</div>
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