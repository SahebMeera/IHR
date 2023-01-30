import { IListValueDisplay } from "./ListValue";

export class IListTypeDisplay {
    listTypeID: number;
    type: string;
    typeDesc: string;
    ListValues: IListValueDisplay[];
}

