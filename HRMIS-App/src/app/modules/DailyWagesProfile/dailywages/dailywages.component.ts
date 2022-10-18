import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AuthenticationService } from '../../../_services/authentication.service';
import { RootService } from '../../../_services/root.service';
import { Subscription } from 'rxjs/Subscription';
import { ActivatedRoute, Router } from '@angular/router';

import { Subject } from 'rxjs/Subject';
import { debounceTime } from 'rxjs/operators';
import { DropDownsHR } from '../../../_helpers/dropdowns.class';
import { NotificationService } from '../../../_services/notification.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { DailyWagerService  } from '../../../_services/daily-wager.service'
import{ dailyWagesProfile } from '../../../_models/_db/daily-wages.model'
import { formatDate } from '@angular/common';

@Component({
  selector: 'app-dailywages',
  templateUrl: './dailywages.component.html',
  styleUrls: ['./dailywages.component.scss']
})
export class DailywagesComponent implements OnInit {

  public dailyWagesProfile = new dailyWagesProfile();
  @ViewChild('cnicFrontRef', { static: false }) public cnicFrontRef: any;
  @ViewChild('cnicBackRef', { static: false }) public cnicBackRef: any;
  @ViewChild('photoRef', { static: false }) public photoRef: any;
  public message: string;
  PersonImageURL: any = 'https://media.defense.gov/2020/Feb/19/2002251686/-1/-1/0/200219-A-QY194-002.JPG';
  public imageURL;
public imagePath;
  public loadingCNIC = true;
  public loading = true;
  dailyWagerProfileForm: FormGroup;
  public isShowHF : boolean = false;
  //PurchaseOrderFormObj = new PurchaseOrderForm();
  public oldCNIC = '';
  private subscription: Subscription;

  private cnicSubscription: Subscription;
  public selectedUC : any = 0
  public cnic: string = '0';
  public cnicFrontFile: any[] = [];
  public cnicBackFile: any[] = [];
  public cnicFrontSrc = '';
  public cnicBackSrc = '';
  public cnicMask: string = "00000-0000000-0";
  public mobileMask: string = "0000-0000000";
  public inputChange: Subject<any>;
  public changingPassword: boolean = false;
  public editProfile: boolean = false;
  public dropDowns: DropDownsHR = new DropDownsHR();
  public healthFacilityFullName = '';
  public workingHealthFacilityFullName = '';
  public profile: any;
  public photoSrc = '';
  public radnom: number = Math.random();
  public divisions: Array<{ text: string, value: string }>;
  public divisionsData: Array<{ text: string, value: string }>;
  public divisionsDataForEdit:any;
  public districtDataForEdit : any;
  public tehsilDataForEdit  :any;
  public ucDataForEdit : any;
  public districts: Array<{ text: string, value: string }>;
  public districtsData: Array<{ text: string, value: string }>;
  public uc: Array<{ text: string, value: string }>;
  public ucData: Array<{ text: string, value: string }>;
  public bank: Array<{ text: string, value: string }>;
  public bankData: Array<{ text: string, value: string }>;
  public DOBValue: Date = new Date();
  public ContractStartValue: Date = new Date();
  public ContractEndValue: Date = new Date();

  public tehsils: Array<{ text: string, value: string }>;
  public tehsilsData: Array<{ text: string, value: string }>;
  public hfTypes: Array<{ text: string, value: string }>;
  public hfTypesData: Array<{ text: string, value: string }>;
  public hfCategories: Array<{ text: string, value: string }>;
  public hfCategoriesData: Array<{ text: string, value: string }>;
  public hfacs: Array<{ text: string, value: string }>;
  public hfacsData: Array<{ text: string, value: string }>;
  public hf : Array<{ text: string, value: string }>;
  public hfData : Array<{ text: string, value: string }>;
  public designationCode = null;
  public districtCode = null;
  public divisionCode = null;
  public tehsilCode = null;
  public hfCode = null;
  public ucCode = null;
  public bankId = null;
public isShowUC = false;
public IsUpdate = false;
public allUCData : any;
public allUC : any;
public contractImageURL:any = 'https://media.defense.gov/2020/Feb/19/2002251686/-1/-1/0/200219-A-QY194-002.JPG';;
public contractImage;
public wagerId = 0;
  public selectedFiltersModel: any = {
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' },
    hf: { Name: 'Select HF', Code: '0' },
    uc: { Name: 'Select UC', Code: '0' }

  };
 

 
  public currentUser: any;
  public hfmisCode: string;
  public hfStatus: string;
  public hfmisCodeParam: string;
  public hfTypeCodes: any[] = [];
  public hfCategoryCodes: any[] = [];
  public hfACs: any[] = [];
  constructor(
    // private _profileService: ProfileService,
    private _authenticationService: AuthenticationService,
    private _rootService: RootService,
    private fb: FormBuilder,
    public _DailyWagerService : DailyWagerService,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    // this.subscribeCNIC();
    this.currentUser = this._authenticationService.getUser();
     
    this.hfmisCode = this.currentUser.HfmisCode;
    this.fetchParams();
    this.getDivisions();
    this.getDistricts();
     this.getTehsils();
     this.GetAllHFUCsForDailyWages()
    this.GetBankForDailyWages();
    this.dailyWagerProfileForm = this.fb.group({
      id: new FormControl(''),
      name: new FormControl(''),
      UserId: new FormControl(''),
      UserName: new FormControl(''),
      fatherName: new FormControl(''),
      cnic: new FormControl(''),
      mobileNumber: new FormControl(''),
      designation:new FormControl(''),
      address:new FormControl(''),
      divisionCode:new FormControl(''),
      districtCode:new FormControl(''),
      tehsilCode:new FormControl(''),
      UcCode:new FormControl(''),
      hfCode:new FormControl(''),
      HfmisCode:new FormControl(''),
      gender:new FormControl(''),
      Category : new FormControl(''),
      EmployementMode : new FormControl(''),
      personImage:new FormControl(''),
      dateOfBirth:new FormControl(Date()),
      dailyWagesContractDetail: this.fb.group({
        id:new FormControl(0),
        wagerProfileId: new FormControl(0),
        contractStartDate: new FormControl(''),
        contractEndDate: new FormControl(''),
        contractImagePath:new FormControl(''),
        contractStatus: new FormControl(''),
        createdBy:new FormControl(''),
      }),
      dailyWagesAccountDetails: this.fb.group({
        id: new FormControl(0),
        dailyWagerProfileId: new FormControl(''),
        bankId: new FormControl(''),
        accountTitle: new FormControl(''),
        accountNumber: new FormControl(''),
      }),
    
    });
    if(this.wagerId != 0){
      this.GetWagerInfoById(this.wagerId);
    }
  }
  GetWagerInfoById(WagerId:any){
    this.loading = true;
    this._DailyWagerService.GetDailyWagerbyId(this.wagerId).subscribe((res: any) => {
      if (res.Status) {
         
        this.IsUpdate = true;
        var date = Date();
        if(res.List.DateOfBirth != null){
          date = formatDate(res.List.DateOfBirth, 'dd/MM/yyyy', 'en-PST')
        }
        if(res.List.Category != null){
          this.GetDesignationByNameOnLoad(res.List.Category)
        }
        if(res.List.TehsilCode != null){
          this.getUCByTehsilCode(res.List.TehsilCode);
          this.getHealthFacilitiesOnLoad(res.List.TehsilCode);
        }
        this.dailyWagerProfileForm.get("UcCode").setValue(Number(res.List.UcCode));
        this.dailyWagerProfileForm.get("HfmisCode").setValue(Number(res.List.HfmisCode));

        this.dailyWagerProfileForm.controls
        this.dailyWagerProfileForm.get("id").setValue(res.List.Id);
        this.dailyWagerProfileForm.get("UserId").setValue(res.List.UserId);
        this.dailyWagerProfileForm.get("UserName").setValue(res.List.UserName);
        this.dailyWagerProfileForm.get("name").setValue(res.List.Name);
        this.dailyWagerProfileForm.get("cnic").setValue(res.List.CNIC);
        this.dailyWagerProfileForm.get("gender").setValue(res.List.Gender);
        this.dailyWagerProfileForm.get("fatherName").setValue(res.List.FatherName);
        this.dailyWagerProfileForm.get("Category").setValue(res.List.Category);
        this.dailyWagerProfileForm.get("designation").setValue(res.List.Designation);
        this.dailyWagerProfileForm.get("EmployementMode").setValue(res.List.EmployementMode);
        
        this.divisionsDataForEdit = this.divisionsData;
        this.districtDataForEdit = this.districtsData;
        this.tehsilDataForEdit   = this.tehsilsData;
        this.ucDataForEdit = this.ucData;
      

        //================ Show/Hide UC and HF on basis of Designation ==========//
        if(res.List.Category == 'Dengue' ||
        res.List.Category == 'Polio'){
           
         this.isShowUC = true;
         this.isShowHF = false;
       }
       else{
         this.isShowUC = false;
         this.isShowHF = true;
   
       }
        //========================== End ==============================//


        if(res.List.DateOfBirth != null){
        this.DOBValue=new Date(res.List.DateOfBirth);
        }
        this.dailyWagerProfileForm.get("mobileNumber").setValue(res.List.MobileNumber);
        // this.dailyWagerProfileForm.get("dailyWagesContractDetail").patchValue(res.List.dailyWagesContractDetail);
        // this.dailyWagerProfileForm.get("dailyWagesAccountDetails").patchValue(res.List.dailyWagesAccountDetail);
        this.dailyWagerProfileForm.get("dailyWagesAccountDetails").get("id").setValue(res.List.dailyWagesAccountDetail.Id)
        this.dailyWagerProfileForm.get("dailyWagesAccountDetails").get("accountTitle").setValue(res.List.dailyWagesAccountDetail.AccountTitle)
        this.dailyWagerProfileForm.get("dailyWagesAccountDetails").get("accountNumber").setValue(res.List.dailyWagesAccountDetail.AccountNumber)
        this.dailyWagerProfileForm.get("dailyWagesAccountDetails").get("bankId").setValue(res.List.dailyWagesAccountDetail.BankId)
        this.dailyWagerProfileForm.get("dailyWagesContractDetail").get("id").setValue(res.List.dailyWagesContractDetail.Id)

        this.ContractStartValue = new Date(res.List.dailyWagesContractDetail.ContractStartDate)
        this.ContractEndValue = new Date(res.List.dailyWagesContractDetail.ContractEndDate)
        if(res.List.dailyWagesContractDetail.ContractImagePath != null){
          this.contractImageURL = "https://hrmis.pshealthpunjab.gov.pk/DailywagerUploads/"+res.List.dailyWagesContractDetail.ContractImagePath;
          this.contractImage = this.contractImageURL;
        }
        if(res.List.PersonImage != null){
          this.PersonImageURL = "https://hrmis.pshealthpunjab.gov.pk/DailywagerUploads/"+res.List.PersonImage;
          this.imageURL = this.PersonImageURL;
        }
          //================= Division ===========================//
          var divisionName = this.divisionsDataForEdit.find(x=>{
            return x.Code == res.List.DivisionCode
          });
          this.selectedFiltersModel.division = { Code: res.List.DivisionCode, Name: divisionName.Name };
          //================= District ===========================//
  
          var districtName = this.districtDataForEdit.find(x=>{
            return x.Code == res.List.DistirctCode
          });
          this.selectedFiltersModel.district = { Code: res.List.DistirctCode, Name: districtName.Name };
          //================= Tehsil ===========================//
  
          var tehsilName = this.tehsilDataForEdit.find(x=>{
            return x.Code == res.List.TehsilCode
          });
          this.selectedFiltersModel.tehsil = { Code: res.List.TehsilCode, Name: tehsilName.Name };
          this.districtCode = res.List.DistirctCode
          this.divisionCode = res.List.DivisionCode
          this.tehsilCode = res.List.TehsilCode

          //================= UC  =================================//
  
          this.ucData = this.allUCData.filter(x=>{
            return x.TehsilCode == res.List.TehsilCode
          });
         
          this.dailyWagerProfileForm.get("designation").setValue(res.List.Designation);
          // this.dailyWagerProfileForm.reset();
this.loading = false;
      }
      else{
    this.loading = false;

        alert(res.Message)
      }
    }, err => {
      console.log(err);
    })
  }
  private fetchParams() {
      
    this.subscription = this.route.params.subscribe(
      (params: any) => {
        if (params.Id != 0) {
          this.wagerId = params.Id;
          
        }
      }
    );
  }

public EmployeeModeList : string[]=[

'Regular','Adhoc','Contract', 'Daily Wages'

]

public CategoryList : string[]=[
 'Dengue','Madadgaar','PMIS','Polio' ,'Other' 


]

  public DesignationList: string[]= [
    'Dengue',
  
    'Polio',
    'PMIS',
'Lady Sanitary Patrol',
'Electrician',
'Data Entry Operator',
'Lift Operator',
'Stretcher Bearer',
'Sanitary Patrol',
'Computer Operator / Data Entry Operator',
'Ward Servant',
'Lab Technician',
'Data Entry Operator (Health Council Budget)',
'Gatekeeper',
'Security Guard',
'Lab Attendant',
'Wheel Chair/Strature',
  ]
  
  onChangedCategory(event){
     debugger;
    var Tempvalue =event.target.value.split(': ')[1];

    if(Tempvalue == 'Dengue' ||
    Tempvalue == 'Polio'){
      this.isShowUC = true;
      this.isShowHF = false;
    }
    else{
      this.isShowUC = false;
      this.isShowHF = true;

    }
    this._DailyWagerService.GetDailyDesignationbyName(Tempvalue).subscribe((res: any) => {
      this.DesignationList = res.List;
    },
      err => { this.handleError(err); }
    );
  }

  GetDesignationByNameOnLoad(categoryName:string){
    this._DailyWagerService.GetDailyDesignationbyName(categoryName).subscribe((res: any) => {
      this.DesignationList = res.List;
    },
      err => { this.handleError(err); }
    );
  }

  onChangedDesignation(event){
    event =event.target.value.split(': ')[1];
    this.designationCode = null;
    this.designationCode = event;
    if(this.dailyWagerProfileForm.value.designation == 'Dengue' ||
     this.dailyWagerProfileForm.value.designation == 'Polio'){
      this.isShowUC = true;
      this.isShowHF = false;
    }
    else{
      this.isShowUC = false;
      this.isShowHF = true;

    }
  }
  public SubmitDailyWagerProfileForm(){
     debugger
    // if(this.imageURL == null){
    //     return alert("Add Person Image")
    // }
    
    if(this.dailyWagerProfileForm.value.name == '')
    {
     return alert("Enter Name")
    }
    else if(this.dailyWagerProfileForm.value.cnic == '')
    {
     return alert("Enter CNIC")
    }
    else if(this.dailyWagerProfileForm.value.mobileNumber == '')
    {
     return alert("Enter Mobile No")
    }
    else if(this.dailyWagerProfileForm.value.gender == '')
    {
     return alert("Enter Gender")
    }
    else if(this.dailyWagerProfileForm.value.fatherName == '')
    {
     return alert("Enter Father Name")
    }
    else if(this.dailyWagerProfileForm.value.Category == '')
    {
     return alert("Select Employee Category")
    }
    else if(this.dailyWagerProfileForm.value.designation == '')
    {
     return alert("Select Employee Designation")
    }
    else if(this.dailyWagerProfileForm.value.EmployementMode == '')
    {
     return alert("Select Employee Mood")
    }

    if(this.dailyWagerProfileForm.value.EmployementMode != 'Regular'){
      if(this.contractImage == null){
        return alert("Add Wager Contract File")
      }
      if(this.ContractStartValue == null){
       return alert("Add Contract Start Date")
      }
      if(this.ContractEndValue == null){
       return alert("Add Contract End Date")
      }
    }
    else{
      this.dailyWagesProfile.DailyWagesContractModel = null;
    }
    if(this.dailyWagerProfileForm.get('dailyWagesAccountDetails').value.bankId == 0){
      this.dailyWagesProfile.dailyWagesAccountDetail = null;
    }
    // else if(this.contractImage == null){

    //   return alert("Add Wager Contract File")
    // }
    // if(this.dailyWagerProfileForm.invalid){
    //   return alert("Fill all fields")
    // }
    if(this.tehsilCode == null){
      return alert("Select Tehsil")
    }
    this.dailyWagesProfile = this.dailyWagerProfileForm.value;
    this.dailyWagesProfile.id = this.dailyWagerProfileForm.value.id;
    this.dailyWagesProfile.UcCode = this.dailyWagerProfileForm.value.UcCode;
      this.dailyWagesProfile.divisionCode = this.divisionCode;
      this.dailyWagesProfile.DistirctCode = this.districtCode;
      this.dailyWagesProfile.tehsilCode = this.tehsilCode;
      this.dailyWagesProfile.personImage = this.imageURL;
      this.dailyWagesProfile.dateOfBirth = this.DOBValue;
      // this.dailyWagesProfile.designation = "Polio";
     
      this.dailyWagesProfile.dailyWagesAccountDetail = this.dailyWagerProfileForm.get('dailyWagesAccountDetails').value
      this.dailyWagesProfile.DailyWagesContractModel = this.dailyWagerProfileForm.get('dailyWagesContractDetail').value
      this.dailyWagesProfile.DailyWagesContractModel.contractImagePath = this.contractImage;
      this.dailyWagesProfile.DailyWagesContractModel.contractStartDate = this.ContractStartValue;
      this.dailyWagesProfile.DailyWagesContractModel.contractEndDate = this.ContractEndValue;

      // this.dailyWagesProfile.dailyWagesAccountDetail.bankId = this.bankId;
       
      // if(!this.dailyWagerProfileForm.valid){
      //   alert("Fill all fields of form")
      // }
      // else{
        this._DailyWagerService.AddDailyWages(this.dailyWagesProfile).subscribe((res: any) => {
          if (res.Status) {
            alert('Save Successfully')
              this.dailyWagerProfileForm.reset();
          }
          else{
            alert(res.Message)
          }
        }, err => {
          console.log(err);
        })
      // }

      
    
  }
  private getDivisions = () => {
     
    this.divisions = [];
    this.divisionsData = [];
    this._rootService.getDivisions(this.hfmisCode).subscribe((res: any) => {
      this.divisions = res;
       
      this.divisionsData = this.divisions.slice();
      if (this.divisions.length == 1) {
        this.selectedFiltersModel.division = this.divisionsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }
  

  CustomerImagePreview(files) {
     
    if (files.length === 0)
      return;
 
    var mimeType = files[0].type;
    if (mimeType.match(/image\/*/) == null) {
      this.message = "Only images are supported.";
      return;
    }
 
    var reader = new FileReader();
    this.imagePath = files;
    reader.readAsDataURL(files[0]); 
    reader.onload = (_event) => {
      this.PersonImageURL = reader.result; 
      this.imageURL = this.PersonImageURL.split(',')[1];
    }
  }

  ContractImagePreview(files) {
     debugger
    if (files.length === 0)
      return;
 
    var mimeType = files[0].type;
    if (mimeType.match(/image\/*/) == null) {
      this.message = "Only images are supported.";
      return;
    }
 
    var reader = new FileReader();
    this.imagePath = files;
    reader.readAsDataURL(files[0]); 
    reader.onload = (_event) => {
      this.contractImageURL = reader.result; 
      this.contractImage = this.contractImageURL.split(',')[1];
    }
  }
  
  public subscribeCNIC() {
   
  }
 
  private fetchData(cnic) {
    // this._profileService.getProfile(cnic).subscribe(
    //   (res: any) => {
    //     if (res) {
    //       this.profile = res as Profile;
    //       this.bindProfileValues();
    //     } else {
    //       this.editProfile = false;
    //     }
    //   },
    //   err => {
    //     this.handleError(err);
    //   }
    // );
  }

  private bindProfileValues() {
    if (this.profile.DateOfBirth) {
      this.profile.DateOfBirth = this.compareDateTo1900(this.profile.DateOfBirth) ? null : new Date(this.profile.DateOfBirth);
    }
    if (this.profile.DateOfFirstAppointment) {
      this.profile.DateOfFirstAppointment = this.compareDateTo1900(this.profile.DateOfFirstAppointment) ? null : new Date(this.profile.DateOfFirstAppointment);
    }
    if (this.profile.DateOfRegularization) {
      this.profile.DateOfRegularization = this.compareDateTo1900(this.profile.DateOfRegularization) ? null : new Date(this.profile.DateOfRegularization);
    }
    if (this.profile.PresentPostingDate) {
      this.profile.PresentPostingDate = this.compareDateTo1900(this.profile.PresentPostingDate) ? null : new Date(this.profile.PresentPostingDate);
    }
    if (this.profile.ContractStartDate) {
      this.profile.ContractStartDate = this.compareDateTo1900(this.profile.ContractStartDate) ? null : new Date(this.profile.ContractStartDate);
    }
    if (this.profile.ContractEndDate) {
      this.profile.ContractEndDate = this.compareDateTo1900(this.profile.ContractEndDate) ? null : new Date(this.profile.ContractEndDate);
    }
    if (this.profile.LastPromotionDate) {
      this.profile.LastPromotionDate = this.compareDateTo1900(this.profile.LastPromotionDate) ? null : new Date(this.profile.LastPromotionDate);
    }
    this.dropDowns.selectedFiltersModel.domicile = this.profile.Domicile_Id != 0 ? { Name: this.profile.Domicile_Name, Id: this.profile.Domicile_Id } : this.dropDowns.selectedFiltersModel.domicile;
    this.dropDowns.selectedFiltersModel.religion = this.profile.Religion_Id != 0 ? { Name: this.profile.Religion_Name, Id: this.profile.Religion_Id } : this.dropDowns.selectedFiltersModel.religion;
    this.dropDowns.selectedFiltersModel.language = this.profile.Religion_Id != 0 ? { Name: this.profile.Language_Name, Id: this.profile.Language_Id } : this.dropDowns.selectedFiltersModel.language;
    this.dropDowns.selectedFiltersModel.departmentDefault = this.profile.Department_Id != 0 ? { Name: this.profile.Department_Name, Id: this.profile.Department_Id } : this.dropDowns.selectedFiltersModel.department;

    if (this.profile.HealthFacility_Id != 0 && this.profile.HealthFacility) {
      this._rootService.searchHealthFacilities(this.profile.HealthFacility).subscribe((data) => {
        this.dropDowns.selectedFiltersModel.actualHealthFacility = data[0] && data[0].FullName != 0 ? data[0].FullName : this.dropDowns.selectedFiltersModel.actualHealthFacility;
      });
    }
    if (this.profile.WorkingHealthFacility_Id != 0 && this.profile.WorkingHealthFacility) {
      this._rootService.searchHealthFacilities(this.profile.WorkingHealthFacility).subscribe((data) => {
        this.dropDowns.selectedFiltersModel.workingHealthFacility = data[0] && data[0].FullName != 0 ? data[0].FullName : this.dropDowns.selectedFiltersModel.workingHealthFacility;
      });
    }
    this.healthFacilityFullName = this.profile.HealthFacility ? this.profile.HealthFacility + ', ' + this.profile.Tehsil + ', ' + this.profile.District : '';
    this.workingHealthFacilityFullName = this.profile.WorkingHealthFacility ? this.profile.WorkingHealthFacility + ', ' + this.profile.WorkingTehsil + ', ' + this.profile.WorkingDistrict : '';
    this.dropDowns.selectedFiltersModel.actualDesignation = this.profile.Designation_Id != 0 ? { Name: this.profile.Designation_Name, Id: this.profile.Designation_Id } : this.dropDowns.selectedFiltersModel.actualDesignation;
    this.dropDowns.selectedFiltersModel.workingDesignation = this.profile.WDesignation_Id != 0 ? { Name: this.profile.WDesignation_Name, Id: this.profile.WDesignation_Id } : this.dropDowns.selectedFiltersModel.workingDesignation;
    this.dropDowns.selectedFiltersModel.qualification = this.profile.Qualification_Id != 0 ? { qualificationname: this.profile.QualificationName, Id: this.profile.Qualification_Id } : this.dropDowns.selectedFiltersModel.qualification;
    this.dropDowns.selectedFiltersModel.specialization = this.profile.Specialization_Id != 0 ? { Name: this.profile.SpecializationName, Id: this.profile.Specialization_Id } : this.dropDowns.selectedFiltersModel.specialization;
    this.dropDowns.selectedFiltersModel.employementMode = this.profile.EmpMode_Id != 0 ? { Name: this.profile.EmpMode_Name, Id: this.profile.EmpMode_Id } : this.dropDowns.selectedFiltersModel.employementMode;
    this.dropDowns.selectedFiltersModel.postType = this.profile.Posttype_Id != 0 ? { Name: this.profile.PostType_Name, Id: this.profile.Posttype_Id } : this.dropDowns.selectedFiltersModel.postType;
    this.dropDowns.selectedFiltersModel.profileStatus = this.profile.Status_Id != 0 ? { Name: this.profile.StatusName, Id: this.profile.Status_Id } : this.dropDowns.selectedFiltersModel.profileStatus;
    /* this.dropDowns.selectedFiltersModel.headOfDepartment =  */

    if (this.profile.Hfac) {
      let hfacs = this.dropDowns.hfacData as any[];
      let hfac = hfacs.find(x => x.Name == this.profile.Hfac);
      if (hfac) {
        this.dropDowns.selectedFiltersModel.hfac = { Name: this.profile.Hfac, Id: hfac.Id };
      }
    }
    this.oldCNIC = this.profile.CNIC;
    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg?v=' + this.radnom;
    /*    this.photoSrc = 'https://hrmis.pshealthpunjab.gov.pk/Uploads/ProfilePhotos/' + this.profile.CNIC + '_23.jpg?v=' + this.radnom; */
    this.editProfile = true;
    this.loading = false;
  }

  private compareDateTo1900(actualDate) {
    let defaultDBDate = new Date(1900, 0, 1);
    let dateComparable = new Date(actualDate);
    return dateComparable.getTime() == defaultDBDate.getTime();
  }
  private handleError(err: any) {
    this.loading = false;
    this.loadingCNIC = false;
    this.changingPassword = false;
    if (err.status == 403) {
      this._authenticationService.logout();
    }
  }
  public dropdownValueChanged = (value, filter) => {
     
    if (!value || !value.Code) {
      return;
      
    }
     
    this.hfmisCodeParam = undefined;
    this.hfmisCode = value.Code;

    if (filter == 'division') {
       
      this.divisionCode = null;
      this.divisionCode = value.Code;
      this.selectedFiltersModel.district.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getDistricts();
      // this.getTehsils();
    }
    if (filter == 'district') {
      this.districtCode = null;
      this.districtCode = value.Code;
      this.selectedFiltersModel.tehsil.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getTehsils();
    }
    if (filter == 'tehsil') {
      this.tehsilCode = null;
      this.tehsilCode = value.Code;
      this.selectedFiltersModel.hf.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getHealthFacilities();
      this.getUCs();
    }
    if (filter == 'hf') {
     
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
    if (filter == 'uc') {
       
      
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
    if (filter == 'bank') {
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
    if (filter == 'designation') {
     
      this.selectedFiltersModel.uc.Code = this.hfmisCode;
      this.resetDrops(filter);
      this.getUCs();
    }
  }
  public handleFilter = (value, filter) => {
    if (filter == 'division') {
      this.divisionsData = this.divisions.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'district') {
      this.districtsData = this.districts.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'tehsil') {
      this.tehsilsData = this.tehsils.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'hf') {
      this.hfData = this.hf.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'uc') {
      this.ucData = this.uc.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
    if (filter == 'bank') {
      this.bankData = this.bank.filter((s: any) => s.Name.toLowerCase().indexOf(value.toLowerCase()) !== -1);
    }
  }
  private resetDrops = (filter: string) => {
    if (filter == 'division') {
      this.selectedFiltersModel.district = { Name: 'Select District', Code: this.currentUser.HfmisCode };
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
    }
    if (filter == 'district') {
      this.selectedFiltersModel.tehsil = { Name: 'Select Tehsil', Code: this.currentUser.HfmisCode };
    }
  }
  private loadDropdownValues = () => {
    this.getDivisions();
    this.getDistricts();
    this.getTehsils();
  }
  private getDistricts = () => {
    this.districts = [];
    this.districtsData = [];
     
    this._rootService.getDistricts(this.hfmisCode).subscribe((res: any) => {
       
      this.districts = res;
      this.districtsData = this.districts.slice();
      if (this.districts.length == 1) {
        this.selectedFiltersModel.district = this.districtsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }

  
  private getTehsils = () => {
    this.tehsils = [];
    this.tehsilsData = [];
    this._rootService.getTehsils(this.hfmisCode).subscribe((res: any) => {
      this.tehsils = res;
      this.tehsilsData = this.tehsils.slice();
      if (this.tehsils.length == 1) {
        this.selectedFiltersModel.tehsil = this.tehsilsData[0];
      }
    },
      err => { this.handleError(err); }
    );
  }

  public uploadBtn(filter: string) {
    if (filter == 'pic') {
      this.photoRef.nativeElement.click();
    } else
      if (filter == 'cnicFront') {
        this.cnicFrontRef.nativeElement.click();
      } else
        if (filter == 'cnicBack') {
          
          this.cnicBackRef.nativeElement.click();
        }
  }

 
  private getUCs(){
    this.uc = [];
    this.ucData = [];
    this._rootService.GetHFUCsForDailyWages(this.hfmisCode).subscribe((res: any) => {
      this.uc = res;
      this.ucData = this.uc.slice();

    },
      err => { this.handleError(err); }
    );
  }

  private getUCByTehsilCode(tehsilCode:any){
    this.uc = [];
    this.ucData = [];
    this._rootService.GetHFUCsForDailyWages(tehsilCode).subscribe((res: any) => {
      this.uc = res;
      this.ucData = this.uc.slice();
    },
      err => { this.handleError(err); }
    );
  }
  
  private GetAllHFUCsForDailyWages(){
    this.uc = [];
    this.ucData = [];
    this._rootService.GetAllHFUCsForDailyWages().subscribe((res: any) => {
      this.allUCData = res;
    },
      err => { this.handleError(err); }
    );
  }

  private GetBankForDailyWages(){
    this.bank = [];
    this.bankData = [];
    this._rootService.GetBankForDailyWages().subscribe((res: any) => {
    this.loading = false;
      this.bank = res;
      this.bankData = this.bank.slice();

    },
      err => { this.handleError(err); }
    );
  }

  private getHealthFacilities = () => {
    this._rootService.getHealthFacilities(this.hfmisCode).subscribe((res: any) => {
       
      this.hf = res;
      this.hfData = this.hf.slice();
      // this.dropDowns.healthFacilities = res;
      // this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();

    },
      err => { this.handleError(err); }
    );
    
  }

  private getHealthFacilitiesOnLoad = (tehsilCode:any) => {
    this._rootService.getHealthFacilities(tehsilCode).subscribe((res: any) => {
       
      this.hf = res;
      this.hfData = this.hf.slice();
      // this.dropDowns.healthFacilities = res;
      // this.dropDowns.healthFacilitiesData = this.dropDowns.healthFacilities.slice();

    },
      err => { this.handleError(err); }
    );
    
  }



  public saveData(){
  }



  ngOnDestroy() {

  }
}
