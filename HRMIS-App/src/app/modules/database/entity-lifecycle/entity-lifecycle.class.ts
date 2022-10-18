export class EntityLifecycle {
    constructor() {

    }
    public _id: string;
    public sq_Id: Number;
    public name: String;
    public cadre: any;
    public scale: any;
    public scale2: any;
    public scale3: any;
    public entity_lifecycle: any;
}

export class EntityLifecycleFilters {
    constructor() {

    }
    public allColumns: boolean;
    public columns: any[];
    public allLogColumns: boolean;
    public logColumns: any[];
    public totalRecords: number;
    public _id: string;
    public _idOperator: string;
    public dsgName: string;
    public dsgNameOperator: string;
    public cadre: string[];
    public cadreOperator: string[];
    public scale: string[];
    public scaleOperator: string;
    public gazatted: string;
    public gazattedOperator: string;
}