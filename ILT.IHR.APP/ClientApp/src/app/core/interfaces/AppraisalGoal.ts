export class IAppraisalGoal {
    AppraisalGoalID: number;
    AppraisalID: number;
    ReviewYear?: number;
    Goal?: string;
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
export class IAppraisalGoalDisplay {
    appraisalGoalID: number;
    appraisalID: number;
    reviewYear?: number;
    goal?: string;
    empResponse?: number;
    empComment?: string;
    mgrResponse?: number;
    mgrComment?: string;
    createdBy?: string;
    createdDate?: Date;
    modifiedBy?: string;
    modifiedDate?: Date;
    timeStamp?: ['']
}
