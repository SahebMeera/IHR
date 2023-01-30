export class IEmployeeSkill {
    EmployeeSkillID: number;
    SkillTypeID: number;
    SkillType?: string;
    EmployeeID: number;
    EmployeeName: string;
    Skill: string;
    Experience: number;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']
}
export class IEmployeeSkillDisplay {
    employeeSkillID: number;
    skillTypeID: number;
    skillType: string;
    employeeID: number;
    employeeName: string;
    skill: string;
    experience: number;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
}
