CREATE TABLE [dbo].[AirMonit] (
    [Id] INT NOT NULL IDENTITY,
    [Parameter] NVARCHAR(50) NULL, 
    [Value] DECIMAL NULL, 
    [DateTime] DATE NULL, 
    [City] NVARCHAR(50) NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

