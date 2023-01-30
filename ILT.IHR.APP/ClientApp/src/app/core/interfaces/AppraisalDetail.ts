export class IAppraisalDetail {
    AppraisalDetailID: number;
    AppraisalID: number;
    AppraisalQualityID: number;
    Quality: string;
    ResponseTypeID?: number;
    ResponseType?: string;
    ResponseTypeDescription?: string;
    EmpResponse?: number;
    EmpComment?: string;
    MgrResponse?: number;
    MgrComment?: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']

}
export class IAppraisalDetailDisplay {
    appraisalDetailID: number;
    appraisalID: number;
    appraisalQualityID: number;
    quality: string;
    responseTypeID?: number;
    responseType?: string;
    responseTypeDescription?: string;
    empResponse?: number;
    empComment?: string;
    mgrResponse?: number;
    mgrComment?: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
