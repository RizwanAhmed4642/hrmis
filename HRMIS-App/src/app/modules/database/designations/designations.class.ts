export class Designation {
  constructor() {}
  public _id: string;
  public sq_Id: Number;
  public name: String;
  public cadre: any;
  public scale: any;
  public scale2: any;
  public scale3: any;
  public entity_lifecycle: any;
  public HrScale: any;
  public Cadre: any;
  public Cadre_Id: any;
  public HrScale_Id: any;
  public IsActive: any;
}
export class HrDesignation {
  public Id: string;
  public Name: String;
  public Cadre_Id: any;
  public HrScale_Id: any;
  public Remarks: string;
}
export class DesignationFilters {
  constructor() {}
  //generic
  public allColumns: boolean;
  public columns: any[];
  public allLogColumns: boolean;
  public logColumns: any[];
  public totalRecords: number;

  //dedicated props
  public _id: string;
  public _idOperator: any;
  public name: string;
  public nameOperator: any;
  public _cadre: string[];
  public _cadreOperator: any;
  public _scale: string[];
  public _scaleOperator: any;
  public gazatted: string;
  public gazattedOperator: any;
}
