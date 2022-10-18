import { EsrForwardToOfficer } from "./EsrForwardToOfficer.class";

export class ESR {
  public Id: number;
  public VDT: Date;
  public Profile_Id: number;
  public CNIC: string;
  public BPSFrom: number;
  public BPSTo: number;
  public CurrentDoJ: Date;
  public CurrentDoT: Date;
  public DesignationFrom: number;
  public DesignationTo: number;
  public DepartmentFrom: number;
  public DepartmentTo: number;
  public HF_Id_From: number;
  public HF_Id_To: number;
  public HfmisCodeFrom: string;
  public HfmisCodeTo: string;
  public PostingOrderNo: string;
  public PostingOrderDate: Date;
  public LengthOfService: string;
  public JoiningStatus: string;
  public EmployeeFileNO: string;
  public IsActive: boolean;
  public EntityLifecycle_Id: number;
  public Remarks: string;
  public COMMENTS: string;
  public PostingOrderPhoto: number[];
  public TargetUser: string;
  public VPot: string;
  public CurrentUser: string;
  public WDesigFrom: string;
  public OrderDetail: string;
  public OrderNumer: string;
  public TransferOrderType: string;
  public TranferOrderStatus: string;
  public VerbalOrderByDesignation: string;
  public VerbalOrderByName: string;
  public NotingFile: string;
  public EmbossedFile: string;
  public SectionOfficer: string;
  public ResponsibleUser: string;

  public DesigFrom: string;
  public DesigTo: string;
  public HF_From: string;
  public HF_TO: string;


  public DistrictFrom?: string;
  public DistrictTo?: string;
  public TehsilFrom?: string;
  public TehsilTo?: string;

  public TransferTypeID: number;
  public DisposalofID: number;
  public Disposalof: string;

  public esrSectionOfficerName: string;

  public EsrSectionOfficerID: number;
  public EsrForwardToOfficers: EsrForwardToOfficer[];
  public MutualESR_Id: number;
  public Created_Date: Date;
  public Created_By: string;
  public Last_Modified_By: string;
  public Users_Id: string;
  public elcIsActive: boolean;
  
  public officerStamp: string;

}
export class ESRView {
  public Id: number;
  public VDT: Date;
  public Profile_Id: number;
  public CNIC: string;
  public BPSFrom: number;
  public BPSTo: number;
  public CurrentDoJ: Date;
  public CurrentDoT: Date;
  public DesignationFrom: number;
  public DesignationTo: number;
  public DepartmentFrom: number;
  public DepartmentTo: number;
  public HF_Id_From: number;
  public HF_Id_To: number;
  public HfmisCodeFrom: string;
  public HfmisCodeTo: string;
  public PostingOrderNo: string;
  public PostingOrderDate: Date;
  public LengthOfService: string;
  public JoiningStatus: string;
  public EmployeeFileNO: string;
  public IsActive: boolean;
  public EntityLifecycle_Id: number;
  public Remarks: string;
  public COMMENTS: string;
  public PostingOrderPhoto: number[];
  public TargetUser: string;
  public VPot: string;
  public CurrentUser: string;
  public WDesigFrom: string;
  public OrderDetail: string;
  public OrderNumer: string;
  public TransferOrderType: string;
  public TranferOrderStatus: string;
  public VerbalOrderByDesignation: string;
  public VerbalOrderByName: string;
  public NotingFile: string;
  public EmbossedFile: string;
  public SectionOfficer: string;
  public ResponsibleUser: string;
  public TransferTypeID: number;
  public DisposalofID: number;
  public Disposalof: string;
  public EsrSectionOfficerID: number;
  public MutualESR_Id: number;
  public OrderHTML: string;
  public ModuleSource: string;
  public EmployeeName: string;
  public DesigFrom: string;
  public DesigTo: string;
  public HF_From: string;
  public HF_TO: string;
  public EsrSectionOfficerName: string;
  public Created_Date: Date;
  public Created_By: string;
  public Last_Modified_By: string;
  public Users_Id: string;
  public elcIsActive: boolean;
  public AppointmentEffect: boolean;
  public AppointmentDate: boolean;
}

export class ESRDetail {
  public Id: number;
  public Master_Id: number;
  public OrderDate: string;
  public Salary: number;
  public StartDate: string;
  public PeriodDuration: string;
  public Reason_Id: number;
}

export class EmpProfile {
  public Id: number;
  public Srno: number;
  public EmployeeName: string;
  public FatherName: string;
  public CNIC: string;
  public DateOfBirth: Date;
  public Gender: string;
  public Province: string;
  public MaritalStatus: string;
  public BloodGroup: string;
  public SeniorityNo: string;
  public PersonnelNo: string;
  public JoiningGradeBPS: number;
  public CurrentGradeBPS: number;
  public PresentPostingOrderNo: string;
  public PresentPostingDate: Date;
  public Specialization: string;
  public AdditionalQualification: string;
  public Status: string;
  public DateOfFirstAppointment: Date;
  public LengthOfService: number;
  public SuperAnnuationDate: Date;
  public ContractStartDate: Date;
  public ContractEndDate: Date;
  public LastPromotionDate: Date;
  public PermanentAddress: string;
  public CorrespondenceAddress: string;
  public LandlineNo: string;
  public MobileNo: string;
  public EMaiL: string;
  public PrivatePractice: string;
  public PresentStationLengthOfService: string;
  public HfmisCode: string;
  public Tenure: string;
  public AdditionalCharge: string;
  public Remarks: string;
  public Photo: number[];
  public HighestQualification: string;
  public MobileNoOfficial: string;
  public Postaanctionedwithscale: string;
  public Faxno: string;
  public HoD: string;
  public Fp: string;
  public Hfac: string;
  public DateOfRegularization: Date;
  public Tbydeo: string;
  public DateOfCourse: string;
  public RtmcNo: string;
  public PmdcNo: string;
  public CourseDuration: string;
  public PgSpecialization: string;
  public Category: string;
  public RemunerationStatus: string;
  public PgFlag: string;
  public CourseName: string;
  public AddToEmployeePool: boolean;
  public Division: string;
  public District: string;
  public Tehsil: string;
  public HealthFacility: string;
  public HealthFacility_Id: number;
  public Designation_Id: number;
  public Designation_Name: string;
  public Cadre_Name: string;
  public WDesignation_Id: number;
  public WDesignation_Name: string;
  public Department_Id: number;
  public Department_Name: string;
  public EmpMode_Id: number;
  public EmpMode_Name: string;
  public Domicile_Id: number;
  public Domicile_Name: string;
  public PostType_Id: number;
  public PostType_Name: string;
  public Religion_Id: number;
  public Religion_Name: string;
  public Language_Id: number;
  public Language_Name: string;

  public StatusName: string;
  public QualificationName: string;
  public SpecializationName: string;


}

export class LeaveOrder {
  public Id: number;
  public Profile_Id: number;
  public LeaveType_Id: number;
  public FromDate: Date;
  public ToDate: Date;
  public TotalDays: number;
  public FileNumber: string;
  public OrderDate: Date;
  public SignedBy: string;
  public Officer_Id: number;
  public Application_Id: number;
  public OrderHTML: string;
  public EntityLifecycle_Id: number;
}