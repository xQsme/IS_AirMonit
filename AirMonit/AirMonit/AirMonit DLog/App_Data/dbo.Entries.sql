CREATE TABLE [dbo].[Entries] (
    [Id]        INT             IDENTITY (1, 1) NOT NULL,
    [NO2] INT   NULL,
    [CO] INT   NULL,
	[O3] INT   NULL,
    [DateTime]  DATE            NULL,
    [City]      NVARCHAR (50)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

