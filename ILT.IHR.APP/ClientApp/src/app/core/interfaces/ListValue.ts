export class IListValue {
    ListValueID: number;
    ListTypeID: number;
    Value: string;
    ValueDesc: string;
    IsActive: boolean;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: string;
}
export class IListValueDisplay {
    listValueID: number;
    listTypeID: number;
    type: string;
    typeDesc: string;
    value: string;
    valueDesc: string;
    isActive: boolean
}
