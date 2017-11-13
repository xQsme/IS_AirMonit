CREATE TABLE [dbo].[Entries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Parameter] NVARCHAR(50) NULL, 
    [Value] DECIMAL NULL, 
    [DateTime] DATE NULL, 
    [City] NVARCHAR(50) NULL
)
