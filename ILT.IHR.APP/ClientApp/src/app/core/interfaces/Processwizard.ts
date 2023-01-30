import { IProcessDataTicketDisplay } from "./ProcessDataTicket";

export class IProcessWizard {
    ProcessWizardID: number;
    Process?: string;
    Fields: IFields[];

}
export class IProcessWizardDisplay {
    processWizardID?: number;
    process?: string;
    name?: string;
    Fields?: IFields[];
}

export class IFields {
    Position?: string;
    ElementType?: string;
    Name?: string;
    Label?: string;
    GridDisplay?: boolean;
    Required?: boolean;
    FullWidth?: boolean;
    Value?: string;
}

//    public string Position { get; set; }
//        public string ElementType { get; set; }
//        public string Name { get; set; }
//        public string Label { get; set; }
//        public string GridDisplay { get; set; }
//        public string Required { get; set; }
//        public string FullWidth { get; set; }
//[Required]
//        public string Value { get; set; }
