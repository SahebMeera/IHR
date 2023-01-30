import { IAppraisalDetail, IAppraisalDetailDisplay } from "./AppraisalDetail";
import { IAppraisalGoal, IAppraisalGoalDisplay } from "./AppraisalGoal";

export class IAppraisal {
    AppraisalID: number;
    ReviewYear: number;
    EmployeeID?: number;
    EmployeeName?: string;
    FinalReviewerID?: number;
    Manager?: string;
    FinalReviewer?: string;
    ReviewerID?: number;
    Reviewer?: string;
    AssignedDate?: Date;
    SubmitDate?: Date;
    ReviewDate?: Date;
    FinalReviewDate?: Date;
    StatusID: number;
    Status?: string;
    StatusValue?: string;
    MgrFeedback: string;
    Comment: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: [''];
    //AppraisalDetails: IAppraisalDetail[];
    //AppraisalGoal: IAppraisalGoal[];
}
export class IAppraisalDisplay {
    appraisalID: number;
    reviewYear: number;
    employeeID?: number;
    employeeName?: string;
    finalReviewerID?: number;
    manager?: string;
    finalReviewer?: string;
    reviewerID?: number;
    reviewer?: string;
    assignedDate?: Date;
    submitDate?: Date;
    reviewDate?: Date;
    finalReviewDate?: Date;
    statusID: number;
    status?: string;
    statusValue?: string;
    mgrFeedback: string;
    comment: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    rimeStamp: ['']
    appraisalDetails: IAppraisalDetailDisplay[]
    appraisalGoals: IAppraisalGoalDisplay[]
}
