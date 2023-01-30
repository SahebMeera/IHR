export class IAsset {
    AssetID: number;
    AssetTypeID: number;
    AssetType: string;
    Tag: string;
    Make?: string;
    Model?: string;
    Configuration: string;
    WiFiMAC: string;
    LANMAC: string;
    OS: string;
    PurchaseDate: Date;
    WarantyExpDate: Date;
    AssignedToID: number;
    AssignedTo: string;
    StatusID: number;
    Status: string;
    Comment: string;
    CreatedBy: string;
    CreatedDate: Date;
    ModifiedBy: string;
    ModifiedDate: Date;
    TimeStamp: ['']

}
export class IAssetDisplay {
    assetID: number;
    assetTypeID: number;
    assetType: string;
    tag: string;
    make?: string;
    model?: string;
    configuration: string;
    wiFiMAC: string;
    lanmac: string;
    os: string;
    purchaseDate: Date;
    warantyExpDate: Date;
    assignedToID: number;
    assignedTo: string;
    statusID: number;
    status: string;
    comment: string;
    createdBy: string;
    createdDate: Date;
    modifiedBy: string;
    modifiedDate: Date;
    timeStamp: ['']
}
