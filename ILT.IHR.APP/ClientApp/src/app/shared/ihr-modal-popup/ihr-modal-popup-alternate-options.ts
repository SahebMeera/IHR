export interface IModalPopupAlternateOptions {
    footerActions?: IAction[];
}

export interface IAction {
    actionText?: string;
    actionMethod?: Function;
    styleClass?: string;
    iconClass?: string;
    disabled?: boolean;
    isHidden?: boolean;
    isConfirm?: boolean;
} 
