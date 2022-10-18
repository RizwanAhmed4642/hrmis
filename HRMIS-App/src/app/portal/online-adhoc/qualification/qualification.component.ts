import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NotificationService } from '../../../_services/notification.service';
import { CookieService } from '../../../_services/cookie.service';
import { AuthenticationService } from '../../../_services/authentication.service';
import { Subscription } from 'rxjs/Subscription';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { RootService } from '../../../_services/root.service';
import { LocalService } from '../../../_services/local.service';
import { OnlineAdhocService } from '../online-adhoc.service';
import { debounceTime } from 'rxjs-compat/operators/debounceTime';
import { Subject } from 'rxjs-compat/Subject';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-qualification-adhoc',
  templateUrl: './qualification.component.html',
  styles: []
})

export class QualificationComponent implements OnInit, OnDestroy {
  @Input() public profile: any;
  public qualifications: any[] = [];
  public orders: any[] = [];
  public leaveOrders: any[] = [];
  public selectedOrder: any;
  public viewOrderWindow: boolean = false;
  public searchingHfs: boolean = false;
  public saving: boolean = false;
  public hfsList: any[] = [];
  public searchEvent = new Subject<any>();
  public searchSubcription: Subscription;
  public serviceHistory: any = {};
  public addNew: boolean = false;
  public loading: boolean = false;
  public hfValue: string = '';
  public qualificationTypeId: number = 0;
  public serviceTotalHistroy: any[] = [];
  public joiningDate: Date = new Date();
  public dropDowns: DropDownsHR = new DropDownsHR();
  public photoSrc = '';
  public cnic = '';
  public photoSrces: any[] = [];
  public photoFile: any[] = [];
  public requiredDegreesAdded: boolean = false;
  public documentMissing: boolean = false;
  public savingRemarks: boolean = false;
  public uploadingFile: boolean = false;
  public uploadingFileError: boolean = false;
  public currentUser: any = {};
  public applicant: any = {};
  public designationId: number = 0;
  public houseJobxperience: any;
  public urdu: any = {
    info: `اپنے تعلیمی کوائف کا درست انتخاب و اندراج کیجئے۔ ٹوٹل و حاصل کردہ نمبروں کا اندراج اور مطلوبہ کاغذات اپلوڈ کریں۔ کسی بھی قسم کے غلط اندراج کی صورت میں آپ کی درخواست پر عمل درآمد روک دیا جائے گا۔`,
    infoeng: 'Please correctly enter your educational data including total marks and obtained marks and upload the required document. In case of any wrong entry your request will not be processed'
  };
  public spRequireDegreeIds = [58, 65, 62, 119, 118, 117, 116, 122, 123];
  public spRequireForeignDegreeIds = [58, 65, 119, 122, 123];
  public mowmoRequireForeignDegreeIds = [58, 65, 119];
  public dsRequireForeignDegreeIds = [58, 65, 113, 134, 135, 136];
  public mowmoRequireDegreeIds = [58, 65, 62, 119, 118, 117, 116];
  public cnRequireDegreeIds = [58, 129, 131, 132, 133];
  public dsRequireDegreeIds = [58, 65, 113, 134, 135, 136];

  constructor(private sanitized: DomSanitizer, private _rootService: RootService,
    private _cookieService: CookieService,
    private _onlineAdhocService: OnlineAdhocService) { }

  ngOnInit() {
    this.cnic = this._cookieService.getCookie('cnicussradhoc');
    this.designationId = +this._cookieService.getCookie('cnicussradhocdesig');
    this.serviceHistory.DegreeType = 'Pakistan';
    this.handleSearchEvents();
    this.getApplicant();
  }
  private getApplicant() {
    this.documentMissing = false;
    this._onlineAdhocService.getApplicant(this.cnic).subscribe((res: any) => {
      if (res) {
        this.applicant = res;
        if (!this.applicant.PMDCDoc || !this.applicant.CNICDoc || !this.applicant.DomicileDoc) {
          this.documentMissing = true;
        }
        this.getApplicantQualification();
      }
    }, err => {
      console.log(err);
    });
  }
  public getApplicantQualification() {
    this._onlineAdhocService.getApplicantQualification(this.applicant.Id).subscribe((data: any) => {
      this.getDegrees();
      if (data) {
        this.qualifications = data;
        if (this.designationId == 302) {
          this.houseJobxperience = {};
        }
        this.houseJobxperience = this.qualifications.find(x => x.Required_Degree_Id == 121);
        this.checkRequiredQualification();
      }
      this.addNew = true;
    },
      err => {
        console.log(err);
      });
  }
  public checkRequiredQualification() {
    let foreignDegree = this.qualifications.find(x => x.DegreeType == 'Foreign');
    let desigIds: number[] = [362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313];
    let requireDegreeIds: number[] = [];
    let desigId = desigIds.find(x => x == this.designationId);
    if (desigId) {
      requireDegreeIds = foreignDegree ? this.spRequireForeignDegreeIds : this.spRequireDegreeIds;
    } else if (this.designationId == 802 || this.designationId == 1320) {
      requireDegreeIds = foreignDegree ? this.mowmoRequireForeignDegreeIds : this.mowmoRequireDegreeIds;
    } else if (this.designationId == 302) {
      requireDegreeIds = this.cnRequireDegreeIds;
    } else if (this.designationId == 431) {
      requireDegreeIds = foreignDegree ? this.dsRequireForeignDegreeIds : this.dsRequireDegreeIds;
    }
    let count: number = 0;
    let temp = [];
    requireDegreeIds.forEach(degreeId => {
      let reqD = this.qualifications.find(x => x.Required_Degree_Id == degreeId);
      if (reqD) {
        temp.push(reqD);
        count++;
      }

      if (!reqD) {
        if (degreeId == Degree.Matriculation) {
          reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.Matriculation || x.Required_Degree_Id == Degree.OLevel);
          if (reqD) {
            temp.push(reqD);
            count++;
          }
        } else if (degreeId == Degree.FSCPreMedical) {
          reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FSCPreMedical || x.Required_Degree_Id == Degree.ALevel);
          if (reqD) {
            temp.push(reqD);
            count++;
          }
        } else if (degreeId == Degree.FCPSPartI) {
          reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.MCPSPartI);
          if (reqD) {
            temp.push(reqD);
            count++;
          }
        } else if (degreeId == Degree.FCPSPartII) {
          reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.MCPSPartI);
          if (reqD) {
            temp.push(reqD);
            count++;
          }
        }
        if (this.designationId == 362) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DiplomainAnesthesia);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DiplomainAnesthesia);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 365) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DiplomainCardiology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DiplomainCardiology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 368) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DiplomainDermatology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DiplomainDermatology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 369) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DLO);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DLO);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 373) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DGO);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DGO);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 374) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DOMS);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DOMS);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 381) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DCH);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DCH);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 382) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DCP || x.Required_Degree_Id == Degree.MPhill );
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DCP || x.Required_Degree_Id == Degree.MPhill);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 385) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DMRD);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DMRD);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 390) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.MSUrology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.MSUrology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 1594) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DiplomainNephrology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DiplomainNephrology);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 2136) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DTCD);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DTCD);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 384) {
          if (degreeId == Degree.FCPSPartI) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartI || x.Required_Degree_Id == Degree.DPM);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.FCPSPartII) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.FCPSPartII || x.Required_Degree_Id == Degree.DPM);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
        if (this.designationId == 302) {
          if (degreeId == Degree.GeneralNursing1stYear ) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.GeneralNursing1stYear || x.Required_Degree_Id == Degree.BSNursing);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          } else if (degreeId == Degree.GeneralNursing2ndYear) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.GeneralNursing2ndYear || x.Required_Degree_Id == Degree.BSNursing);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
          else if (degreeId == Degree.GeneralNursing3rdYear) {
            reqD = this.qualifications.find(x => x.Required_Degree_Id == Degree.GeneralNursing3rdYear || x.Required_Degree_Id == Degree.BSNursing);
            if (reqD) {
              temp.push(reqD);
              count++;
            }
          }
        }
      }
    });
    this.houseJobxperience = this.qualifications.find(x => x.Required_Degree_Id == 121);
    if (count >= requireDegreeIds.length) {
      this.requiredDegreesAdded = true;
    } else {
      this.requiredDegreesAdded = false;
    }
  }
  public submitHouseJob() {
    this.saving = true;
    this.serviceHistory.Applicant_Id = this.applicant.Id;
    this.serviceHistory.Required_Degree_Id = 121;
    this.serviceHistory.QualificationTypeId = 5;
    if (this.serviceHistory.PassingYear) {
      this.serviceHistory.PassingYear = this.serviceHistory.PassingYear.toDateString();
    }
    this._onlineAdhocService.saveApplicantQualification(this.serviceHistory).subscribe((res: any) => {
      if (res.Id) {
        if (this.photoFile.length > 0) {
          this._onlineAdhocService.uploadApplicantQualification(this.photoFile, res.Id).subscribe((x: any) => {
            if (!x) {
              this.uploadingFileError = true;
            }
            this.serviceHistory = {};
            this.hfValue = '';
            this.saving = false;
            this.getApplicantQualification();
          }, err => {
            console.log(err);
            this.uploadingFileError = true;
            this.uploadingFile = false;
          });
        } else {
          this.serviceHistory = {};
          this.hfValue = '';
          this.saving = false;
          this.getApplicantQualification();
        }

      }
    }, err => {
      console.log(err);
    });
  }

  public submitServiceHistory() {
    this.saving = true;
    this.serviceHistory.Applicant_Id = this.applicant.Id;
    if (this.serviceHistory.PassingYear) {
      this.serviceHistory.PassingYear = this.serviceHistory.PassingYear.toDateString();
    }
    if (this.serviceHistory.ExpFrom) {
      this.serviceHistory.ExpFrom = this.serviceHistory.ExpFrom.toDateString();
    }
    if (this.serviceHistory.ExpTo) {
      this.serviceHistory.ExpTo = this.serviceHistory.ExpTo.toDateString();
    }
    this._onlineAdhocService.saveApplicantQualification(this.serviceHistory).subscribe((res: any) => {
      this.addNew = false;
      if (res.Id) {
        if (this.photoFile.length > 0) {
          this._onlineAdhocService.uploadApplicantQualification(this.photoFile, res.Id).subscribe((x: any) => {
            if (!x) {
              this.uploadingFileError = true;
            }
            this.serviceHistory = {};
            this.serviceHistory.DegreeType = 'Pakistan';
            this.photoFile = [];
            this.hfValue = '';
            this.saving = false;
            this.getApplicantQualification();
          }, err => {
            console.log(err);
            this.uploadingFileError = true;
            this.uploadingFile = false;
          });
        } else {
          this.serviceHistory = {};
          this.serviceHistory.DegreeType = 'Pakistan';
          this.hfValue = '';
          this.saving = false;
          this.getApplicantQualification();
        }

      }
    }, err => {
      console.log(err);
    });
  }
  public viewOrder(order) {
    this.selectedOrder = order;
    if (this.selectedOrder) {
      console.log(this.selectedOrder);

      this.openViewOrderWindow();
    }
  }
  public openViewOrderWindow() {
    this.viewOrderWindow = true;
  }
  public closeViewOrderWindow() {
    this.viewOrderWindow = false;
    this.selectedOrder = null;
  }
  private getQualificationTypes = () => {
    this.dropDowns.qualificationTypes = [];
    this._rootService.getQualificationType().subscribe((res: any) => {
      this.dropDowns.qualificationTypes = res;
    },
      err => {
        console.log(err);
      }
    );
  }
  private getDegrees = () => {
    this.dropDowns.degrees = [];
    this._onlineAdhocService.getDegrees(this.designationId).subscribe((res: any) => {
      this.setDegrees(res);
      this.setRequiredDegrees(this.dropDowns.degrees);
    },
      err => {
        console.log(err);
      }
    );
  }
  private setDegrees(degrees: any[]) {
    let desigIds: number[] = [362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313];
    let requireDegreeIds: number[] = [];
    let desigId = desigIds.find(x => x == this.designationId);
    if (desigId) {
      requireDegreeIds = this.spRequireDegreeIds;
    } else if (this.designationId == 802 || this.designationId == 1320) {
      requireDegreeIds = this.mowmoRequireDegreeIds;
    } else if (this.designationId == 302) {
      requireDegreeIds = this.cnRequireDegreeIds;
    } else if (this.designationId == 431) {
      requireDegreeIds = this.dsRequireDegreeIds;
    }
    degrees.forEach(degree => {
      if (degree) {
        let dId = requireDegreeIds.find(x => x == degree.Id);
        if (dId) {
          degree.required = true;
          degree.text = 'required';
        }
        if (degree.Id == 124 && desigId) {
          degree.text = '(2 years relevant experience required)';
        }

      }
    });
    this.dropDowns.degrees = degrees;
  }
  private setRequiredDegrees(degrees: any[]) {
    let desigIds: number[] = [362, 365, 368, 369, 373, 374, 375, 381, 382, 383, 384, 385, 387, 390, 1594, 1598, 2136, 2313];
    let requireDegreeIds: number[] = [];
    let desigId = desigIds.find(x => x == this.designationId);
    if (desigId) {
      requireDegreeIds = this.spRequireDegreeIds;
    } else if (this.designationId == 802 || this.designationId == 1320) {
      requireDegreeIds = this.mowmoRequireDegreeIds;
    } else if (this.designationId == 302) {
      requireDegreeIds = this.cnRequireDegreeIds;
    } else if (this.designationId == 431) {
      requireDegreeIds = this.dsRequireDegreeIds;
    }
    let temp: any = [];
    degrees.forEach(degree => {
      if (degree) {
        let q = this.qualifications.find(x => x.Required_Degree_Id == degree.Id);
        if (!q) {
          temp.push(degree);
        };
      }
    });
    this.dropDowns.degrees = temp;
  }
  public degreeTypeChanged() {
    let degrees = this.dropDowns.degrees;
    this.dropDowns.degrees = [];
    if (this.serviceHistory.DegreeType == 'Pakistan') {
      this.spRequireDegreeIds = [58, 65, 62, 119, 118, 117, 116, 122, 123];
      this.mowmoRequireDegreeIds = [58, 65, 62, 119, 118, 117, 116];
      this.dsRequireDegreeIds = [58, 65, 113, 134, 135, 136];
    } else {
      this.spRequireDegreeIds = this.spRequireForeignDegreeIds;
      this.mowmoRequireDegreeIds = this.mowmoRequireForeignDegreeIds;
      this.dsRequireDegreeIds = this.dsRequireForeignDegreeIds;
    }
    this.getDegrees();
  }
  public readUrlAndUpload(event: any, dataItem: any) {
    if (event.target.files && event.target.files[0]) {
      dataItem.photoFile = [];
      let inputValue = event.target;
      dataItem.photoFile = inputValue.files;
      dataItem.uploadingFile = true;
      this._onlineAdhocService.uploadServiceAttachement(dataItem.photoFile, dataItem.Id).subscribe((x: any) => {
        if (!x) {
          dataItem.uploadingFileError = true;
        }
        dataItem.uploadingFile = false;
        this.serviceHistory = {};
        this.hfValue = '';
        this.dropDowns.selectedFiltersModel.designation = this.dropDowns.defultFiltersModel.designation;
        this.dropDowns.selectedFiltersModel.employementMode = this.dropDowns.defultFiltersModel.employementMode;
        this.saving = false;
        this.getApplicantQualification();
      }, err => {
        console.log(err);
        dataItem.uploadingFileError = true;
        dataItem.uploadingFile = false;
      });
    }
  }
  public dropdownValueChanged = (value, filter) => {
    if (!value) {
      return;
    }
    if (filter == 'qualificationType') {
      this.qualificationTypeId = value.Id;
      this.serviceHistory.QualificationTypeId = this.qualificationTypeId;
      this.getDegrees();
    }
    if (filter == 'degree') {
      this.serviceHistory.Required_Degree_Id = value.Id;
      this.serviceHistory.QualificationTypeId = value.QualificationTypeId;
      console.log(this.serviceHistory.QualificationTypeId);

    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'designation') {
      this.dropDowns.designationsData = this.dropDowns.designations.filter((s: any) => s.Name.toLowerCase().startsWith(value.toLowerCase()));
    }
    if (filter == 'employementMode') {
      this.dropDowns.employementModesData = this.dropDowns.employementModes.filter((s: any) => s.Name.toLowerCase().startsWith(value.toLowerCase()));
    }
  }
  public readUrl(event: any, filter: string) {
    if (event.target.files && event.target.files[0]) {
      if (filter == 'pic') {
        this.photoFile = [];
        let inputValue = event.target;
        this.photoFile = inputValue.files;
        /*  var reader = new FileReader();
         reader.onload = ((event: any) => {
           this.photoSrc = event.target.result;
         }).bind(this);
         reader.readAsDataURL(event.target.files[0]); */
      }
    }
  }
  public openInNewTab(link: string) {
    window.open(link, '_blank');
  }
  public removeApplicantQualification(item: any) {
    if (confirm('Confirm Remove Qualification?')) {
      item.removing = true;
      this._onlineAdhocService.removeApplicantQualification(item.Id).subscribe((data: any) => {
        this.getApplicantQualification();
        item.removing = false;
      },
        err => {
          console.log(err);
        });
    }
  }
  public search(value: string, filter: string) {
    if (filter == 'hfs') {
      this.hfsList = [];
      if (value.length > 2) {
        this.searchingHfs = true;
        this._rootService.searchHealthFacilitiesAll(value).subscribe((data) => {
          this.hfsList = data as any[];
          this.searchingHfs = false;
        });
      }
    }
  }
  public searchClicked(FullName, filter) {

    if (filter == 'hfs') {
      let item = this.hfsList.find(x => x.FullName === FullName);
      if (item) {
        this.serviceHistory.HF_Id = item.Id;
      }
    }
    if (filter == 'whfs') {
      if (!FullName) {
        this.profile.WorkingHealthFacility_Id = null;
        this.profile.WorkingHFMISCode = null;
      }
    }
  }
  transform(value) {
    return this.sanitized.bypassSecurityTrustHtml(value);
  }
  public handleSearchEvents() {
    this.searchSubcription = this.searchEvent.pipe(
      debounceTime(400)).subscribe((x: any) => {
        this.search(x.event, x.filter);
      });
  }
  ngOnDestroy() {
    this.searchSubcription.unsubscribe();
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
}

enum Degree {
  Matriculation = 58,
  FirstProfessionalMBBSI = 62,
  FSCPreMedical = 65,
  OLevel = 93,
  ALevel = 94,
  FirstProfessionalBDS = 113,
  FirstProfessionalMBBSII = 116,
  SecondProfessionalMBBS = 117,
  ThirdProfessionalMBBS = 118,
  FinalProfessionalMBBS = 119,
  FCPSPartI = 122,
  FCPSPartII = 123,
  MCPSPartI = 124,
  MSMDPartI = 125,
  MSMDPartII = 126,
  MCPSPartII = 127,
  MastersinPublicHealth = 128,
  BSNursing = 63,
  GeneralNursing1stYear = 129,
  DiplomainAnesthesia = 130,
  GeneralNursing2ndYear = 131,
  GeneralNursing3rdYear = 132,
  DiplomainGeneralNursingandMidwifery = 133,
  SecondProfessionalBDS = 134,
  ThirdProfessionalBDS = 135,
  FinalProfessionalBDS = 136,
  DMRD = 137,
  DiplomainCardiology = 138,
  DiplomainDermatology = 139,
  DLO = 140,
  DOMS = 141,
  DCH = 142,
  DCP = 143,
  MSUrology = 144,
  DiplomainNephrology = 145,
  DTCD = 146,
  DGO = 147,
  MPhill = 120,
  DPM = 148
}
