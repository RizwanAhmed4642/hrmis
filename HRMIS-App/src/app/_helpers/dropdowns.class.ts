import { RootService } from "../_services/root.service";
import {
  cstmdummyHFAC,
  cstmdummyModes,
  cstmdummyStatus,
  cstmdummyOperatorDropDown,
  cstmdummyOperatorString,
  cstmdummyOperatorNumber,
  cstmdummyOperatorDate,
  cstmdummyOperatorBoolean,
  cstmdummyCadre,
  cstmdummyScale,
  cstmdummyGazatted,
  cstmdummyRetirementTypes,
  cstmdummyReligion,
  cstmdummyLanguages,
  cstmdummyActiveStatus,
  cstmdummyOrderReasons,
  maritalStatusItems,
  bloodGroupItems,
  privatePractiveItems,
  hodItems,
  cstmdummyGender
} from "../_models/cstmdummydata";

export class DropDownsHR {

  public divisions: Array<any>;
  public divisionsData: Array<any>;

  public districts: Array<any>;
  public districtsData: Array<any>;

  public tehsils: Array<any>;
  public tehsilsData: Array<any>;

  public hfCategory: Array<any>;
  public hfCategoryData: Array<any>;

  public hfTypes: Array<any>;
  public hfTypesData: Array<any>;

  public ucs: Array<any>;
  public ucsData: Array<any>;

  public orderTypes: Array<any>;
  public orderTypesData: Array<any>;

  public leaveTypes: Array<any>;
  public leaveTypesData: Array<any>;

  public designations: Array<any>;
  public designationsData: Array<any>;

  public healthFacilities: Array<any>;
  public healthFacilitiesData: Array<any>;


  public profileAttachments: Array<any>;
  public profileAttachmentsData: Array<any>;

  public postingModes: Array<any>;


  public hfacs: Array<any>;
  public hfacsData: Array<any>;

  public covidFacilityTypes: Array<any>;
  public covidFacilities: Array<any>;
  public covidFacilitiesData: Array<any>;


  public dhqs: Array<any>;
  public thqs: Array<any>;


  public departments: Array<any>;
  public departmentsData: Array<any>;

  public domicile: Array<any>;
  public domicileData: Array<any>;

  public qualifications: Array<any>;
  public qualificationsData: Array<any>;

  public qualificationTypes: any[] = [];
  public degrees: any[] = [];
  public appTypePendancy: any[] = [];
  public employementModes: any[] = [];
  public employementModesData: any[] = [];

  public applicationPurposes: any[] = [];
  public disposalOf: any[] = [];

  public postTypes: Array<any>;
  public postTypesData: Array<any>;

  public profileStatuses: Array<any>;
  public profileStatusesData: Array<any>;

  public specialization: Array<any>;
  public specializationData: Array<any>;

  public adhocJobs: Array<any>;
  public adhocJobsData: Array<any>;

  public adhocApplicationStatuses: Array<any>;
  public adhocApplicationStatusesData: Array<any>;

  public hfacData: Array<any> = cstmdummyHFAC.slice();
  public maritalStatusItems: Array<any> = maritalStatusItems.slice();
  public bloodGroupItems: Array<any> = bloodGroupItems.slice();
  public hodItems: Array<any> = hodItems.slice();
  public privatePractiveItems: Array<any> = privatePractiveItems.slice();
  public modesData: Array<any> = cstmdummyModes.slice();
  public statusData: Array<any> = cstmdummyStatus.slice();
  public cadreData: Array<any> = cstmdummyCadre.slice();
  public scaleData: Array<any> = cstmdummyScale.slice();
  public gazattedData: Array<any> = cstmdummyGazatted.slice();
  public retirementTypes: Array<any> = cstmdummyRetirementTypes.slice();
  public religions: Array<any> = cstmdummyReligion.slice();
  public languages: Array<any> = cstmdummyLanguages.slice(); cstmdummyGender
  public genders: Array<any> = cstmdummyGender.slice();
  public inboxOfficers: Array<any>;

  public applicationSources: Array<any>;
  public applicationTypes: Array<any>;
  public applicationDocuments: Array<any>;
  public applicationTypesData: Array<any>;

  public applicationStatus: Array<any>;
  public applicationStatusData: Array<any>;

  public filterOperatorString: Array<any> = cstmdummyOperatorString.slice();
  public filterOperatorNumber: Array<any> = cstmdummyOperatorNumber.slice();
  public filterOperatorDate: Array<any> = cstmdummyOperatorDate.slice();
  public filterOperatorBoolean: Array<any> = cstmdummyOperatorBoolean.slice();
  public filterOperatorDropdown: Array<any> = cstmdummyOperatorDropDown.slice();

  public wards: Array<any>;
  public inquiryStatuses: Array<any>;

  public cadres: Array<any>;
  public cadresData: Array<any>;

  public scales: Array<any>;
  public scalesData: Array<any>;


  public Activestatus: Array<any> = cstmdummyActiveStatus.slice();
  public disabilityTypes: any[] = [];

  // attandance
  public candidateStatus: Array<any>;
  public EmpStatus: Array<any>;
  public EmpStatusData: Array<any>;

  public subDepartments: Array<any>;
  public subDepartmentsData: Array<any>;

  public years: Array<any>;
  public yearsData: Array<any>;

  public months: Array<any>;
  public monthsData: Array<any>;
  // end attandance
  //selected object for drop downs
  public selectedFiltersModel: any = {
    postingMode:{Name: "Select Working Mode", Id: 0},
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' },
    hfType: { Name: 'Select Type of Health Facility', Code: '0' },
    wards: { Name: 'Select Ward', Id: null },
    covidFacility: { Name: 'Select Type', Id: null },
    orderType: { Name: 'Select Order Type', Id: 0 },
    retirementType: { Name: 'Select Retirement Type', Id: 0 },
    healthFacility: { Name: 'Select Health Facility', Id: 0 },
    healthFacilityPref: { FullName: 'Select Health Facility', Id: null },
    vpPref: { HFName: 'Select Health Facility', Id: null },
    complaintType: { Name: 'Select Complaint Type', Id: null },
    actualHealthFacility: 'Type Health Facility',
    workingHealthFacility: 'Type Working Health Facility',
    department: { Name: 'Select Department', Id: null },
    departmentDefault: { Name: 'Primary & Secondary Healthcare Department', Id: 25 },
    signingOfficer: { DesignationName: 'Select Officer', Id: 0 },
    sectionOfficer: { DesignationName: 'Select Section', Id: 0 },
    signedByOfficer: { DesignationName: 'Select Signed by', Id: 0 },
    designation: { Name: 'Select Designation', Id: 0 },
    qualificationType: { QualificationType1: 'Select Qualification', Id: 0 },
    degree: { DegreeName: 'Select Degree', Id: 0 },
    post: { Name: 'Select Post', Id: 0 },
    actualDesignation: { Name: 'Select Actual Designation', Id: 0 },
    joiningDesignation: { Name: 'Select Joining Designation', Id: 0 },
    workingDesignation: { Name: 'Select Working Designation', Id: 0 },
    leaveType: { LeaveType1: 'Select Leave Type', Id: 0 },
    applicationType: { Name: 'Select Type', Id: 0 },
    applicationDocument: { Name: 'Select Document', Id: 0 },
    applicationStatus: { Name: 'Select Status', Id: 0 },
    hfCategory: { Name: 'Select Health Facility Category', Code: '0' },
    hfac: { Name: 'Select Administrative Control', Id: 0 },
    inboxOfficers: { Name: 'Select Office', Id: 0 },
    mode: { Name: 'Select Mode', Id: 0 },
    disability: { Name: 'Select Disability Type', Id: null },
    status: { Name: 'Select Health Facility Status', Id: 0 },
    currentStatus: { Name: 'Select Status', Id: 0 },
    profileStatus: { Name: 'Select Employee Status', Id: 0 },
    domicile: { DistrictName: 'Select Domicile', Id: null },
    religion: { Name: 'Select Religion', Id: null },
    language: { Name: 'Select Mother Tounge', Id: null },
    specialization: { Name: 'Select Specialization', Id: null },
    employementMode: { Name: 'Select Employment Mode', Id: null },
    disposalOf: { Name: 'Select Disposal Of', Id: null },
    postType: { Name: 'Select Post Type', Id: null },
    adhocJob: { DesignationName: 'Select Post', Designation_Id: 0 },
    adhocApplicationStatus: { Name: 'Select Status', Id: 0 },
    qualification: { qualificationname: 'Select Highest Qualification', Id: null },
    additionalQualification: { qualificationname: 'Select Additional Qualification', Id: null },
    scale: 0,
    hod: 0,
    headOfDepartment: { Name: "Select Yes / No", Id: null },
    ucDrp: { UC: 'Select UC', Id: null },
    designationInterview: { DesignationName: 'Select Designation', Designation_Id: 0 },
    interviewBatch: { BatchNo: 'Select Batch', Id: 0 },
    designationForTransfer: { Name: 'Select Designation', Id: 0 },
    vaccinationType: { Name: 'Select Vaccination Type', Id: 0 },
    designationForOrderTransfer: { DsgName: 'Select Designation', Desg_Id: null },
    divisionForTransfer: { Name: 'Select Division', Code: '0' },
    districtForTransfer: { Name: 'Select District', Code: '0' },
    tehsilForTransfer: { Name: 'Select Tehsil', Code: '0' },
    candidateStatus: { Name: 'Select Candidate Status', Id: 0 },
    healthFacilityForTransfer: { Name: 'Select Health Facility', Id: 0 },
    officer: { DesignationName: "Select Officer", Id: 0 },
    applicationSource: { Name: "Select Source", Id: 0 },
    program: "Select Program",
    cadre: { name: "Select Cadre", id: 0 },
    inquiryStatus: { Name: 'Select Inquiry Status', Id: null },
    applicationPurpose: { Purpose: "Select Purpose", Id: null },

    ScaleDefault: { Name: "Select Scale", Id: null },
    fileRequestStatus: { ReuestStatus: "Select Request Status", Id: 0 },

    orderReasons: { Name: "Select reason or mention in other", Id: 0 },
    healthFacilitiesAtDisposal: { Name: "Select Department / Office", Id: 0 },

    //attandance
    EmpStatus: { Name: "Select Status", Id: null },
    subDepartments: { SubDept_Name: "Select Sub Department", SubDept_ID: null }
    //attandance
  };

  public programs: string[] = [
    "Admin",
    "Development Wing",
    "Drug Control",
    "Technical",
    "Vertical Program"
  ];
  public officers: any[] = [];
  public officersData: any[] = [];
  public healthFacilitiesAtDisposal: any[] = [];
  public orderReasons: any[] = cstmdummyOrderReasons;

  public fileRequestStatus: any[];


  public defultFiltersModel: any = {
    postingMode:{Name: "Select Working Mode", Id: 0},
    division: { Name: 'Select Division', Code: '0' },
    district: { Name: 'Select District', Code: '0' },
    tehsil: { Name: 'Select Tehsil', Code: '0' },
    hfType: { Name: 'Select Type of Health Facility', Code: '0' },
    wards: { Name: 'Select Ward', Id: null },
    orderType: { Name: 'Select Order Type', Id: 0 },
    retirementType: { Name: 'Select Retirement Type', Id: 0 },
    ucDrp: { UC: 'Select UC', Id: null },
    healthFacility: { Name: 'Select Health Facility', Id: 0 },
    healthFacilityPref: { FullName: 'Select Health Facility', Id: null },
    vpPref: { HFName: 'Select Health Facility', Id: null },
    actualHealthFacility: { FullName: 'Type Health Facility', Id: 0 },
    workingHealthFacility: { FullName: 'Type Working Health Facility', Id: 0 },
    department: { Name: 'Select Department', Id: null },
    departmentDefault: { Name: 'Primary & Secondary Healthcare Department', Id: 25 },
    sectionOfficer: { DesignationName: 'Select Section', Id: 0 },
    signedByOfficer: { DesignationName: 'Select Signed by', Id: 0 },
    designation: { Name: 'Select Designation', Id: 0 },
    designationInterview: { DesignationName: 'Select Designation', Designation_Id: 0 },
    post: { Name: 'Select Post', Id: 0 },
    officer: { DesignationName: "Select Officer", Id: 0 },
    actualDesignation: { Name: 'Select Actual Designation', Id: 0 },
    currentStatus: { Name: 'Select Status', Id: 0 },
    joiningDesignation: { Name: 'Select Joining Designation', Id: 0 },
    workingDesignation: { Name: 'Select Working Designation', Id: 0 },
    qualificationType: { QualificationType1: 'Select Qualification', Id: 0 },
    degree: { DegreeName: 'Select Degree', Id: 0 },
    leaveType: { LeaveType1: 'Select Leave Type', Id: 0 },
    applicationType: { Name: 'Select Type', Id: 0 },
    applicationDocument: { Name: 'Select Document', Id: 0 },
    applicationSource: { Name: "Select Source", Id: 0 },
    applicationStatus: { Name: 'Select Status', Id: 0 },
    hfCategory: { Name: 'Select Health Facility Category', Code: '0' },
    hfac: { Name: 'Select Administrative Control', Id: 0 },
    mode: { Name: 'Select Mode', Id: 0 },
    interviewBatch: { BatchNo: 'Select Batch', Id: 0 },
    status: { Name: 'Select Health Facility Status', Id: 0 },
    profileStatus: { Name: 'Select Employee Status', Id: 0 },
    inboxOfficers: { Name: 'Select Office', Id: 0 },
    domicile: { DistrictName: 'Select Domicile', Id: null },
    religion: { Name: 'Select Religion', Id: null },
    covidFacility: { Name: 'Select Type', Id: null },
    language: { Name: 'Select Mother Tounge', Id: null },
    vaccinationType: { Name: 'Select Vaccination Type', Id: 0 },
    specialization: { Name: 'Select Specialization', Id: null },
    employementMode: { Name: 'Select Employment Mode', Id: null },
    disposalOf: { Name: 'Select Disposal Of', Id: null },
    adhocJob: { DesignationName: 'Select Post', Designation_Id: 0 },
    adhocApplicationStatus: { Name: 'Select Status', Id: 0 },
    qualification: { qualificationname: 'Select Highest Qualification', Id: null },
    postType: { Name: 'Select Post Type', Id: null },
    additionalQualification: { qualificationname: 'Select Additional Qualification', Id: null },
    scale: 0,
    hod: 0,
    designationForTransfer: { Name: 'Select Designation', Id: 0 },
    inquiryStatus: { Name: 'Select Inquiry Status', Id: null },
    designationForOrderTransfer: { DsgName: 'Select Designation', Desg_Id: null },
    divisionForTransfer: { Name: 'Select Division', Code: '0' },
    districtForTransfer: { Name: 'Select District', Code: '0' },
    tehsilForTransfer: { Name: 'Select Tehsil', Code: '0' },
    healthFacilityForTransfer: { Name: 'Select Health Facility', Id: 0 },
    candidateStatus: { Name: 'Select Candidate Status', Id: 0 },
    disability: { Name: 'Select Disability Type', Id: null },

    cadre: { Name: "Select Cadre", Id: null },
    Scale: { Name: "Select Scale", Id: null },

    complaintType: { Name: 'Select Complaint Type', Id: null },


    cadreVal: 0,
    orderReasons: { Name: "Select reason or mention in other", Id: 0 },
    healthFacilitiesAtDisposal: { Name: "Select Department / Office", Id: 0 },
    fileRequestStatus: { ReuestStatus: "Select Request Status", Id: 0 },

    gender: { text: "Select Gender", value: null },
    maritalStatus: { text: "Select Marital Status", value: null },
    bloodGroup: { text: "Select Blood Group", value: null },
    headOfDepartment: { Name: "Select Yes / No", Id: null },
    privatePractive: { text: "Select Yes / No", value: null },
    joiningScale: { text: "Select Joining Grade (BPS)", value: null },
    currentScale: { text: "Select Current Grade (BPS)", value: null },

    ScaleDefault: { Name: "Select Scale", Id: null },
    applicationPurpose: { Purpose: "Select Purpose", Id: null },


    //attandance
    months: { MonthName: 'Select Month', Month: '0' },
    EmpStatus: { Name: "Select Status", Id: null },
    years: { Name: 'Select Year', Id: '0' },
    subDepartments: { SubDept_Name: "Select Sub Department", SubDept_ID: null },
    //attandance
  };
}
