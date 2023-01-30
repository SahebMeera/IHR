CREATE TYPE [dbo].[TypeEmployeeAddress] AS TABLE (
    [EmployeeAddressID] INT           NOT NULL,
    [EmployeeID]        INT           NOT NULL,
    [AddressTypeID]     INT           NOT NULL,
    [Address1]          VARCHAR (100) NOT NULL,
    [Address2]          VARCHAR (100) NULL,
    [City]              VARCHAR (50)  NOT NULL,
    [State]             VARCHAR (50)  NOT NULL,
    [Country]           VARCHAR (50)  NOT NULL,
    [ZipCode]           VARCHAR (10)  NOT NULL,
    [StartDate]         DATE          NOT NULL,
    [EndDate]           DATE          NULL);

