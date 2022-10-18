import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { debounce } from 'rxjs/operator/debounce';
import { RootService } from '../../../_services/root.service';
import { PublicPortalMOService } from '../public-portal-mo.service';

@Component({
  selector: 'hrmis-promotion-application',
  templateUrl: './promotion-application.component.html',
  styles: [`.well{
    padding: 18px;
    border-radius: 6px;
    box-shadow: 0px 1px 2px black;
    background: rgba(212, 212, 212, 0.48);
    /* color: white; */
  }
  .k-switch-label-on{
    left: -40px !important;
  }
  .DisplayNone{
    display:none;
  }
  `]
})
export class PromotionApplicationComponent implements OnInit {

  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  // public promotionJobApplication: PromotionJobApplication = new PromotionJobApplication();
  public promotionJobApplication: any = {};
  public bindingData: any = null;
  public flags: any = {};
  public extras: any = {};
  public presentStatues = ["Active/Working", "Extra Ordinary Leave", "Awaiting Posting"];
  public dateTime: Date = new Date('2019-12-31T23:59:59');
  @ViewChild('AppliedForDesignationDDL', { static: false }) AppliedForDesignationDDL: any;
  @ViewChild('AppliedForDesignationDDLSR', { static: false }) AppliedForDesignationDDLSR: any;
  @ViewChild('PresentDesignationDDL', { static: false }) PresentDesignationDDL: any;
  @ViewChild('HfNameDDL', { static: false }) HfNameDDL: any;
  @ViewChild('DistrictNameDDL', { static: false }) DistrictNameDDL: any;
  @ViewChild('rpt', { static: true }) rpt: any;

  public searching: boolean = false;



  constructor(private _rootService: RootService, private common: PublicPortalMOService, private formBuilder: FormBuilder) {

    this.initPromotionJobApplicationModel();



  }

  initPromotionJobApplicationModel() {
    this.promotionJobApplication.PromotionJobApplicationServiceStatements = [];
    this.promotionJobApplication.PromotionJobApplicationServiceStatements.push(
      {
        Text: ''
      }
    );

    //setting default values for DDLs
    this.promotionJobApplication.RequestedAs = '';
    this.promotionJobApplication.AppliedForDesignation_Id = '';
    this.promotionJobApplication.AppliedForDesignationSR_Id = '';
    this.promotionJobApplication.Designation_Id = '';
    this.promotionJobApplication.DistrictOfDomicile = '';
    this.extras.healthFacilities = [];


    this.promotionJobApplication.CertificateOfWorkingFilepathRpt = 'No';
    this.promotionJobApplication.NoEnquiryCeritificateFilepathRpt = 'No';
    this.promotionJobApplication.MatricFScMbbsDegreeFilepathRpt = 'No';
    this.promotionJobApplication.PostgraduateDegreeFilepathRpt = 'No';
    this.promotionJobApplication.PmdcFilepathRpt = 'No';
    this.promotionJobApplication.NoEnquiryCertifcateAttestedFilepathRpt = 'No';
    this.promotionJobApplication.SeniorityNoFilepathRpt = 'No';
    this.promotionJobApplication.ExperienceCertificateFilepathRpt = 'No';

    //setting default flags
    this.flags.cow = 'false';
    this.flags.noe = 'false';
    this.flags.Pst = 'false';
    this.flags.Postgraduate = 'false';
    this.flags.Pmdc = 'false';
    this.flags.Noea = 'false';
    this.flags.SeniorityNo = 'false';
    this.flags.expc = 'false';
    this.flags.HideReport = true;
    this.flags.isLoading = false;
    this.flags.showSeniorityNoPhotoSizeError = true;

    // this.extras.AppReport = false;

    this.extras.existingApplication = false;
    this.extras.existingProfile = false;
  }

  ngOnInit() {
    this.loadData();
  }
  loadData() {
    this.common.getBindingData().subscribe((x: any) => {
      this.bindingData = x.Data;
      this.extras.divisions = x.Data.Divisions.Data;
    });


  }
  checkProfile() {
    this.searching = true;
    this.extras.existingProfile = null;
    this.extras.existingApplication = false;
    this.flags.profileError = false;

    var cnic = this.promotionJobApplication.CNIC.replace(/[^0-9]/g, '');

    this.common.checkProfile(this.promotionJobApplication.CNIC).subscribe((x) => {
      this.validateApplicant(x, this.promotionJobApplication.CNIC);
      this.searching = false;
    });
  }
  validateApplicant(x: any, cnic: any) {
    x.Data = JSON.parse(x.Data);

    if (x.Type == "Exception") { this.flags.profileError = x.Message; console.log(x.exception); return; }

    this.extras.existingProfile = x.Data.Profile;
    this.extras.existingApplication = x.Data.PromotionApp;
    this.extras.existingApplicationSS = x.Data.PromotionAppServiceStatements;

    var pro = this.validateApplicantProfile();
    var app = this.validateExistingPromotion();
    console.log(pro);
    if (app && pro == null) {
      // this.extras.AppReport = this.extras.existingApplication;
      // this.extras.AppReport = true;
      this.promotionJobApplication = this.extras.existingApplication;
      this.flags.HideReport = false;
    }
    if (app && pro) {
      // this.extras.AppReport = this.extras.existingApplication;
      // this.extras.AppReport = true;
      this.promotionJobApplication = this.extras.existingApplication;
      this.flags.HideReport = false;
    }
    if (!app && pro) {
      this.bindExistingProfileToForm();
      this.extras.showForm = true;
    }
    if (!app && pro == null) {
      this.extras.showForm = true;
      this.promotionJobApplication = {};
      this.initPromotionJobApplicationModel();
      this.promotionJobApplication.CNIC = cnic;
    }
    if (!app && pro == false) {
      this.flags.profileError = 'Only Medical Officer(MO) and Women Medical Officers(WMO) are eligible for this promotion';
    }



  }
  bindExistingProfileToForm() {
    this.promotionJobApplication.Profile_Id = this.extras.existingProfile.Id;
    this.promotionJobApplication.Name = this.extras.existingProfile.EmployeeName;
    this.promotionJobApplication.FatherName = this.extras.existingProfile.FatherName;
    this.promotionJobApplication.DateOfBirth = this.extras.existingProfile.DateOfBirth ? new Date(this.extras.existingProfile.DateOfBirth) : null;
    this.promotionJobApplication.CNIC = this.extras.existingProfile.CNIC;
    this.promotionJobApplication.HFMIS = this.extras.existingProfile.HfmisCode;
    this.promotionJobApplication.Address = this.extras.existingProfile.PermanentAddress;
    this.promotionJobApplication.Email = this.extras.existingProfile.EMaiL;
    this.promotionJobApplication.MobileNumber = this.extras.existingProfile.MobileNo;
    // this.promotionJobApplication.PromotionToCurrentScale = this.extras.existingProfile.LastPromotionDate;
    this.promotionJobApplication.SeniorityNo = this.extras.existingProfile.SeniorityNo;
  }
  validateExistingPromotion(): boolean {
    if (this.extras.existingApplication != null) {

      this.extras.existingApplication.CertificateOfWorkingFilepathRpt =
        (this.extras.existingApplication.CertificateOfWorkingFilepath == '' ||
          this.extras.existingApplication.CertificateOfWorkingFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.NoEnquiryCeritificateFilepathRpt =
        (this.extras.existingApplication.NoEnquiryCeritificateFilepath == '' ||
          this.extras.existingApplication.NoEnquiryCeritificateFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.MatricFScMbbsDegreeFilepathRpt =
        (this.extras.existingApplication.MatricFScMbbsDegreeFilepath == '' ||
          this.extras.existingApplication.MatricFScMbbsDegreeFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.PostgraduateDegreeFilepathRpt =
        (this.extras.existingApplication.PostgraduateDegreeFilepath == '' ||
          this.extras.existingApplication.PostgraduateDegreeFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.PmdcFilepathRpt =
        (this.extras.existingApplication.PmdcFilepath == '' ||
          this.extras.existingApplication.PmdcFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.NoEnquiryCertifcateAttestedFilepathRpt =
        (this.extras.existingApplication.NoEnquiryCertifcateAttestedFilepath == '' ||
          this.extras.existingApplication.NoEnquiryCertifcateAttestedFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.SeniorityNoFilepathRpt =
        (this.extras.existingApplication.SeniorityNoFilepath == '' ||
          this.extras.existingApplication.SeniorityNoFilepath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.ExperienceCertificateFilepathRpt =
        (this.extras.existingApplication.ExperienceCertFilePath == '' ||
          this.extras.existingApplication.ExperienceCertFilePath == null) ? 'No' : 'Yes';

      this.extras.existingApplication.PromotionJobApplicationServiceStatements = this.extras.existingApplicationSS;
      if (this.extras.existingApplication.PromotionJobApplicationServiceStatements == null)
        this.extras.existingApplication.PromotionJobApplicationServiceStatements = [];



      return true;
    }
    else {
      return false;
    }
  }
  validateApplicantProfile(): any {

    if (this.extras.existingProfile == null) return null;
    /* else return true; */
    if
      (
      this.extras.existingProfile.Designation_Id == 802 ||
      this.extras.existingProfile.Designation_Id == 1320 ||
      this.extras.existingProfile.WDesignation_Id == 802 ||
      this.extras.existingProfile.WDesignation_Id == 1320 ||
      this.extras.existingProfile.Designation_Id == 2404 ||
      this.extras.existingProfile.WDesignation_Id == 2404 ||
      this.extras.existingProfile.Designation_Id == 362 ||
      this.extras.existingProfile.Designation_Id == 365 ||
      this.extras.existingProfile.Designation_Id == 368 ||
      this.extras.existingProfile.Designation_Id == 369 ||
      this.extras.existingProfile.Designation_Id == 373 ||
      this.extras.existingProfile.Designation_Id == 374 ||
      this.extras.existingProfile.Designation_Id == 375 ||
      this.extras.existingProfile.Designation_Id == 381 ||
      this.extras.existingProfile.Designation_Id == 382 ||
      this.extras.existingProfile.Designation_Id == 383 ||
      this.extras.existingProfile.Designation_Id == 384 ||
      this.extras.existingProfile.Designation_Id == 385 ||
      this.extras.existingProfile.Designation_Id == 387 ||
      this.extras.existingProfile.Designation_Id == 389 ||
      this.extras.existingProfile.Designation_Id == 390 ||
      this.extras.existingProfile.Designation_Id == 1594 ||
      this.extras.existingProfile.Designation_Id == 1598 ||
      this.extras.existingProfile.Designation_Id == 2136 ||
      this.extras.existingProfile.Designation_Id == 2149 ||
      this.extras.existingProfile.WDesignation_Id == 362 ||
      this.extras.existingProfile.WDesignation_Id == 365 ||
      this.extras.existingProfile.WDesignation_Id == 368 ||
      this.extras.existingProfile.WDesignation_Id == 369 ||
      this.extras.existingProfile.WDesignation_Id == 373 ||
      this.extras.existingProfile.WDesignation_Id == 374 ||
      this.extras.existingProfile.WDesignation_Id == 375 ||
      this.extras.existingProfile.WDesignation_Id == 381 ||
      this.extras.existingProfile.WDesignation_Id == 382 ||
      this.extras.existingProfile.WDesignation_Id == 383 ||
      this.extras.existingProfile.WDesignation_Id == 384 ||
      this.extras.existingProfile.WDesignation_Id == 385 ||
      this.extras.existingProfile.WDesignation_Id == 387 ||
      this.extras.existingProfile.WDesignation_Id == 389 ||
      this.extras.existingProfile.WDesignation_Id == 390 ||
      this.extras.existingProfile.WDesignation_Id == 1594 ||
      this.extras.existingProfile.WDesignation_Id == 1598 ||
      this.extras.existingProfile.WDesignation_Id == 2136 ||
      this.extras.existingProfile.WDesignation_Id == 2149
    ) {

      return true;
    }
    else {

      return false;
    }
  }
  AddServiceStatement() {
    this.promotionJobApplication.PromotionJobApplicationServiceStatements.push(
      {
        Text: ''
      }
    );
  }
  removeServiceStatement(index: number) {
    this.promotionJobApplication.PromotionJobApplicationServiceStatements.splice(index, 1);
  }
  submitApplication(form: any) {

    debugger;
    // Check if photos are null
    this.promotionJobApplication.CNIC = this.promotionJobApplication.CNIC.replace(/[^0-9]/g, '');

    this.promotionJobApplication.MobileNumber = this.promotionJobApplication.MobileNumber.replace(/[^0-9]/g, '');

    this.flags.validCnic = (this.promotionJobApplication.CNIC.length == 13) ? true : false;

    if (this.promotionJobApplication.DateOfBirth) {
      debugger;
      let currentDate = new Date(this.promotionJobApplication.DateOfBirth);
      this.promotionJobApplication.DateOfBirth = currentDate.toDateString();      
    }

    if (this.promotionJobApplication.PromotionToCurrentScale) {
      let currentDateSC = new Date(this.promotionJobApplication.PromotionToCurrentScale);
      this.promotionJobApplication.PromotionToCurrentScale = currentDateSC.toDateString();
    }
    console.log('dob: ', this.promotionJobApplication.DateOfBirth, 'reg: ', this.promotionJobApplication.PromotionToCurrentScale);
    if (this.extras.CowPhoto == null && this.flags.cow == 'true') this.flags.showCowPhotoSizeError = true;
    if (this.extras.NoePhoto == null && this.flags.noe == 'true') this.flags.showNoePhotoSizeError = true;
    if (this.extras.PstPhoto == null && this.flags.Pst == 'true') this.flags.showPstPhotoSizeError = true;
    if (this.extras.PostgraduatePhoto == null && this.flags.Postgraduate == 'true') this.flags.showPostgraduatePhotoSizeError = true;
    if (this.extras.PmdcPhoto == null && this.flags.Pmdc == 'true') this.flags.showPmdcPhotoSizeError = true;
    if (this.extras.NoeaPhoto == null && this.flags.Noea == 'true') this.flags.showNoeaPhotoSizeError = true;
    if (this.extras.SeniorityNoPhoto == null && this.flags.SeniorityNo == 'true') this.flags.showNoeaPhotoSizeError = true;
    if (this.extras.ExpcPhoto == null && this.flags.Expc == 'true') this.flags.showExpcPhotoSizeError = true;

    if (this.extras.SignedCopyPhoto == null) this.flags.showSignedCopyPhotoSizeError = true;


    // Check if required fields are provided photos,duplicate CNIC etc.
    if (
      this.flags.showCowPhotoSizeError != true &&
      this.flags.showNoePhotoSizeError != true &&
      this.flags.showPstPhotoSizeError != true &&
      this.flags.showPostgraduatePhotoSizeError != true &&
      this.flags.showPmdcPhotoSizeError != true &&
      this.flags.showNoeaPhotoSizeError != true &&
      this.flags.showSignedCopyPhotoSizeError != true && this.flags.showSignedCopyPhotoSizeError != true &&
      this.flags.showExpcPhotoSizeError != true &&
      this.flags.validCnic
    ) {
      this.flags.isLoading = true;
      let fd = new FormData();
      //add form values in request
      fd.append("model", JSON.stringify(this.promotionJobApplication));



      //add images in request
      if (this.flags.showCowPhotoSizeError == false) fd.append("Cow", this.extras.CowPhoto);
      if (this.flags.showNoePhotoSizeError == false) fd.append("Noe", this.extras.NoePhoto);
      if (this.flags.showPstPhotoSizeError == false) fd.append("Pst", this.extras.PstPhoto);
      if (this.flags.showPostgraduatePhotoSizeError == false) fd.append("Postgraduate", this.extras.PostgraduatePhoto);
      if (this.flags.showPmdcPhotoSizeError == false) fd.append("Pmdc", this.extras.PmdcPhoto);
      if (this.flags.showNoeaPhotoSizeError == false) fd.append("Noea", this.extras.NoeaPhoto);
      if (this.flags.showSeniorityNoPhotoSizeError == false) fd.append("SeniorityNo", this.extras.SeniorityNoPhoto);
      if (this.flags.showSignedCopyPhotoSizeError == false) fd.append("SignedCopy", this.extras.SignedCopyPhoto);
      if (this.flags.showExpcPhotoSizeError == false) fd.append("ExperienceCertifcate", this.extras.ExpcPhoto);


      this.common.savePromotionApplication(fd).subscribe((x: any) => {

        this.flags.isLoading = false;
        if (x.Type == 'Success') {
          this.flags.showSuccessLbl = true;
          this.extras.submittedApplication = x.Data;
        }
        if (x.Type == 'Error') {
          this.flags.showErrorLbl = x.Message;
          console.log(x.Message);
        };
        if (x.Type == 'Exception') {
          console.log(x.exception);
          console.log(x.Message);
          this.flags.showErrorLbl = 'Oops!! something went wrong. Please try again later. Thank You.';
        }
      });
    }
    else this.flags.showErrorLbl = 'Please remove all the errors to successfully save the data.';

  }

  AppliedForDesignationDdlConsChanged() {

    var obj = this.bindingData.ConsultantDesignations
      .find(x => x.Id == this.promotionJobApplication.AppliedForDesignation_Id);
    this.promotionJobApplication.AppliedForDesignationName = (obj == null) ? '' : obj.Name;

  }
  AppliedForDesignationDdlSrChanged() {

    var obj = this.bindingData.SrDesignations
      .find(x => x.Id == this.promotionJobApplication.AppliedForDesignationSR_Id);
    this.promotionJobApplication.AppliedForDesignationSrName = (obj == null) ? '' : obj.Name;

  }
  PresentDesignationDDLChanged() {
    var obj = this.bindingData.EligibleDesignations
      .find(x => x.Id == this.promotionJobApplication.Designation_Id);
    this.promotionJobApplication.CurrentDesignationName = (obj == null) ? '' : obj.Name;
  }
  DistrictNameDdlChanged() {
    var obj3 = this.bindingData.Districts.Data
      .find(x => x.Id == this.promotionJobApplication.DistrictOfDomicile);
    this.promotionJobApplication.DistrictName = (obj3 == null) ? '' : obj3.Name;
  }
  HfNameDdlChanged() {
    var obj2 = this.extras.healthFacilities
      .find(x => x.HFMISCode == this.promotionJobApplication.HFMIS);
    this.promotionJobApplication.HfName = (obj2 == null) ? '' : obj2.FullName;
  }

  /*  showReport() {
 
    this._mainService.printHTML(this.rpt.nativeElement.innerHTML);
   } */
  showReport() {
    /* let html = document.getElementById('applicationPrint').innerHTML; */
    var mywindow = window.open('', 'PRINT', 'height=600,width=900');
    if (mywindow) {
      mywindow.document.write(this.rpt.nativeElement.innerHTML);
      mywindow.document.write('</body></html>');
      //show upload signed copy input chooser
      /*     mywindow.document.close(); // necessary for IE >= 10
          mywindow.focus(); // necessary for IE >= 10
          mywindow.print();
          mywindow.close(); */
    }
  }
  //region Setting Images

  validateExtension(filename: string): boolean {

    var arr = filename.split('.');
    var extension = arr[arr.length - 1];
    if (
      filename.endsWith('png') ||
      filename.endsWith('PNG') ||
      filename.endsWith('jpg') ||
      filename.endsWith('JPG') ||
      filename.endsWith('JPEG') ||
      filename.endsWith('jpeg') ||
      filename.endsWith('pdf') ||
      filename.endsWith('PDF')
    ) {
      return true;
    }
    else {
      return false;
    }

  }

  setSeniorityNoFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.SeniorityNoPhoto = null;
      this.promotionJobApplication.SeniorityNoFilepathRpt = 'No';
      this.flags.SeniorityNo = 'false';
    }
    else {

      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.SeniorityNoPhoto = null;
        this.promotionJobApplication.SeniorityNoFilepathRpt = 'No';
        this.flags.showSeniorityNoPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.SeniorityNoFilepathRpt = 'Yes';
        this.extras.SeniorityNoPhoto = event.target.files[0];
        this.flags.showSeniorityNoPhotoSizeError = false;
      }
    }
  }

  setCowFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.CowPhoto = null;
      this.promotionJobApplication.CertificateOfWorkingFilepathRpt = 'No';
      this.flags.cow = 'false';
    }
    else {

      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.CowPhoto = null;
        this.promotionJobApplication.CertificateOfWorkingFilepathRpt = 'No';
        this.flags.showCowPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.CertificateOfWorkingFilepathRpt = 'Yes';
        this.extras.CowPhoto = event.target.files[0];
        this.flags.showCowPhotoSizeError = false;
      }
    }
  }
  setNoeFile(event: any) {
    debugger;
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.NoePhoto = null;
      this.promotionJobApplication.NoEnquiryCeritificateFilepathRpt = 'No';
      this.flags.noe = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.NoePhoto = null;
        this.promotionJobApplication.NoEnquiryCeritificateFilepathRpt = 'No';
        this.flags.showNoePhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.NoEnquiryCeritificateFilepathRpt = 'Yes';
        this.extras.NoePhoto = event.target.files[0];
        this.flags.showNoePhotoSizeError = false;
      }
    }
  }
  setPstFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.PstPhoto = null;
      this.promotionJobApplication.MatricFScMbbsDegreeFilepathRpt = 'No';
      this.flags.Pst = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.PstPhoto = null;
        this.promotionJobApplication.MatricFScMbbsDegreeFilepathRpt = 'No';
        this.flags.showPstPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.MatricFScMbbsDegreeFilepathRpt = 'Yes';
        this.extras.PstPhoto = event.target.files[0];
        this.flags.showPstPhotoSizeError = false;
      }
    }
  }
  setPostgraduateFile(event: any) {
    debugger;
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.PostgraduatePhoto = null;
      this.promotionJobApplication.PostgraduateDegreeFilepathRpt = 'No';
      this.flags.Postgraduate = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.PostgraduatePhoto = null;
        this.promotionJobApplication.PostgraduateDegreeFilepathRpt = 'No';
        this.flags.showPostgraduatePhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.PostgraduateDegreeFilepathRpt = 'Yes';
        this.extras.PostgraduatePhoto = event.target.files[0];
        this.flags.showPostgraduatePhotoSizeError = false;
      }
    }
  }
  setPmdcFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.PmdcPhoto = null;
      this.promotionJobApplication.PmdcFilepathRpt = 'No';
      this.flags.Pmdc = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.PmdcPhoto = null;
        this.promotionJobApplication.PmdcFilepathRpt = 'No';
        this.flags.showPmdcPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.PmdcFilepathRpt = 'Yes';
        this.extras.PmdcPhoto = event.target.files[0];
        this.flags.showPmdcPhotoSizeError = false;
      }
    }
  }
  setNoeaFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.NoeaPhoto = null;
      this.promotionJobApplication.NoEnquiryCertifcateAttestedFilepathRpt = 'No';
      this.flags.Noea = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.NoeaPhoto = null;
        this.promotionJobApplication.NoEnquiryCertifcateAttestedFilepathRpt = 'No';
        this.flags.showNoeaPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.NoEnquiryCertifcateAttestedFilepathRpt = 'Yes';
        this.extras.NoeaPhoto = event.target.files[0];
        this.flags.showNoeaPhotoSizeError = false;
      }
    }
  }


  setExpCertFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.ExpcPhoto = null;
      this.promotionJobApplication.ExperienceCertificateFilepathRpt = 'No';
      this.flags.expc = 'false';
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.ExpcPhoto = null;
        this.promotionJobApplication.ExperienceCertificateFilepathRpt = 'No';
        this.flags.showExpcPhotoSizeError = true;
      }
      else {
        this.promotionJobApplication.ExperienceCertificateFilepathRpt = 'Yes';
        this.extras.ExpcPhoto = event.target.files[0];
        this.flags.showExpcPhotoSizeError = false;
      }
    }
  }

  setSignedCopyFile(event: any) {
    if (event.target.files[0] == null || typeof event.target.files[0] == 'undefined') {
      this.extras.SignedCopyPhoto = null;
    }
    else {
      if (event.target.files[0].size / 1024 / 1024 > 4 || this.validateExtension(event.target.files[0].name) == false) {
        this.extras.SignedCopyPhoto = null;
        this.flags.showSignedCopyPhotoSizeError = true;
      }
      else {
        this.extras.SignedCopyPhoto = event.target.files[0];
        this.flags.showSignedCopyPhotoSizeError = false;
      }
    }
  }

  //endregion

  //region HealthFacility
  loadFromHFData() {


  }
  public onChangeDivision(value: any) {
    if (value === null) {
      return;
    }
    this.resetOthersOnDivisionChange();
    this._rootService.getDistricts('0').subscribe(x => {
      this.extras.districts = x;
    });
  }
  public onChangeDistricts(value: any) {
    if (value === null) {
      return;
    }
    this.resetOthersOnDistrictnChange();
    this._rootService.getTehsils('0').subscribe(x => {
      this.extras.tehsils = x;
    });
  }
  onChangeTehsil(code: string) {
    if (code === null) {
      return;
    }
    this.extras.healthFacilities = [];
    this.common.getDetailList(code).subscribe((x: any) => {
      this.extras.healthFacilities = x.Data;
    });
  }

  public resetOthersOnDivisionChange() {
    this.extras.districts = [];
    this.extras.tehsils = [];
  }


  public resetOthersOnDistrictnChange() {
    this.extras.tehsils = [];
  }
  //endregion

}
