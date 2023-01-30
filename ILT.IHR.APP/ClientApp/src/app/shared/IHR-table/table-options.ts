export class ITableOptions {

}

export class ITableRowAction {
    actionMethod?: Function;
    iconClass?: string;
    styleClass?: string;
    toolTip?: string;
    isHidden?: boolean;
    disabled?: boolean;
}

export class ITableHeaderAction {
    actionMethod?: Function;
    hasIcon?: boolean;
    styleClass?: string;
    iconClass?: string;
    actionText?: string;
    isHidden?: boolean;
    isDisabled?: boolean;
    isToggle?: boolean;
    toggleItems?: IToggleButtonItems[];
    toolTip?: string;
}

export interface IToggleButtonItems {
    actionMethod?: Function;
    actionText?: string;
}
