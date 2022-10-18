export class VpPack {
    public vpMaster: VpMProfileView;
    public vpDetails: VpDProfileView[];
    public vpMasterLog: any;
    public vpHolder: VPHolderView = new VPHolderView();
    public vpMasterLogs: any[];
}

export class VpMProfileView {
    constructor() { }
    public Id: number;
    public TotalSanctioned: number;
    public TotalWorking: number;
    public TotalApprovals: number;
    public Vacant: number;
    public Profiles: number;
    public WorkingProfiles: number;
    public HFMISCode: string;
    public HF_Id: number;
    public HFName: string;
    public PostType_Id: number;
    public PostTypeName: string;
    public Desg_Id: number;
    public DsgName: string;
    public BPSWorking: number;
    public Cadre_Id: number;
    public CadreName: string;
    public BPS: number;
    public BPS2: number;
    public HFTypeId: number;
    public HFTypeCode: string;
    public HFTypeName: string;
    public EntityLifeCycle_Id: number;
    public Created_By: string;
    public Created_Date: Date;
    public Modified_By: string;
    public Modified_Date: Date;
    public Users_Id: string;
    public VPDetails: VpDProfileView[] = [];
}
export class VpDProfileView {
    constructor() { }
    public Id: number;
    public Master_Id: number;
    public TotalWorking: number;
    public TotalApprovals: number;
    public Profiles: number;
    public WorkingProfiles: number;
    public EmpMode_Id: number;
    public EmpModeName: string;
    public EntityLifeCycle_Id: number;
    public Created_By: string;
    public Created_Date: Date;
    public Modified_By: string;
    public Modified_Date: Date;
    public Users_Id: string;
}
export class VPDetail {
    public Id: number;
    public Master_Id: number;
    public EmpMode_Id: number;
    public TotalWorking: number;
    public TotalApprovals: number;
    public EntityLifecycle_Id: number;
}


export class VPHolder {
    public Id: number;
    public TotalSeats: number;
    public TotalSeatsVacant: number;
    public TotalSeatsHold: number;
    public VpMaster_Id: number;
    public VpDetail_Id: number;
    public DateTime: Date;
    public User_Id: string;
    public CreatedBy: string;
    public IsActive: boolean;
    public FileNumber: string;
    public TrackingNumber: number;
    public EmployeeName: string;
    public Profile_Id: number;
    public Application_Id: number;
    public ESR: string;
    public ELR: string;
    public OrderNumber: string;
    public OrderAttachment_Id: number;
    public Elc_Id: number;
}

export class VPHolderView {
    public Id: number;
    public TotalSeats: number;
    public TotalSeatsVacant: number;
    public TotalSeatsHold: number;
    public VpMaster_Id: number;
    public VpDetail_Id: number;
    public Elc_Id: number;
    public DateTime: Date;
    public User_Id: string;
    public CreatedBy: string;
    public IsActive: boolean;
    public FileNumber: string;
    public TrackingNumber: number;
    public EmployeeName: string;
    public Profile_Id: number;
    public Application_Id: number;
    public ESR: string;
    public ELR: string;
    public OrderNumber: string;
    public OrderAttachment_Id: number;
    public TotalSanctioned: number;
    public TotalWorking: number;
    public Profiles: number;
    public WorkingProfiles: number;
    public Vacant: number;
    public HFMISCode: string;
    public HF_Id: number;
    public DivisionCode: string;
    public DivisionName: string;
    public DistrictCode: string;
    public DistrictName: string;
    public TehsilCode: string;
    public TehsilName: string;
    public HFName: string;
    public PostType_Id: number;
    public PostTypeName: string;
    public Desg_Id: number;
    public DsgName: string;
    public BPSWorking: number;
    public Cadre_Id: number;
    public CadreName: string;
    public BPS: number;
    public BPS2: number;
    public HFTypeId: number;
    public HFTypeCode: string;
    public HFTypeName: string;
    public EntityLifeCycle_IdM: number;
    public Created_By: string;
    public Created_Date: Date;
    public Last_Modified_By: string;
    public Modified_By: string;
    public Modified_Date: Date;
    public Users_Id: string;
    public ActingCharge: number;
    public ActiveCharge: number;
    public Adhoc: number;
    public Adjusted: number;
    public AdjustedForPayPurpose: number;
    public Contract: number;
    public CurrentCharge: number;
    public DailyWages: number;
    public Deputation: number;
    public GeneralDuty: number;
    public OnPayScale: number;
    public ProjectContract: number;
    public Regular: number;
    public Reinstated: number;
    public PHFMC: number;
    public Consultancy: number;
    public TotalWorkingD: number;
    public ProfilesD: number;
    public WorkingProfilesD: number;
    public EmpMode_Id: number;
    public EmpModeName: string;
    public EntityLifeCycle_IdD: number;
    public Created_ByD: string;
    public Created_DateD: Date;
    public Modified_ByD: number;
    public Modified_DateD: number;
    public Users_IdD: string;
}