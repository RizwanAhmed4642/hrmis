export class DBColumn {
    constructor() {
    }
    public name: string;
    public value: boolean;
}

export class FilterModel {
    constructor() {
    }
    public _id: Property;
    public name: Property;
}

export class Property {
    public operatorAndValues: any[];
}

/* export const tcs = () => {
    let obj = new FilterModel();
    obj._id = {
        '$in': [
            ObjectId('5bfceda3b41f84596c100518'),
            ObjectId('5bfceda3b41f84596c10052d')
        ]
    }
} 

export class ObjectId {

} */