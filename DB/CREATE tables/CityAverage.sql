CREATE TABLE [dbo].[CityAverage] (
    [Id]       INT        IDENTITY (1, 1) NOT NULL,
	[Year]     SMALLINT DEFAULT YEAR(GETDATE()),
	[MONTH]    SMALLINT DEFAULT MONTH(GETDATE()),
	[WEEK]     SMALLINT DEFAULT DATEPART( wk,GETDATE()),
	[cityId]   INT        NOT NULL,
    [particle] NCHAR (5)  NOT NULL,
    [sum]      BIGINT     DEFAULT ((0)) NULL,
    [count]    INT        DEFAULT ((0)) NULL,
    [average]  FLOAT (53) DEFAULT ((0)) NULL,
    
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([cityId]) REFERENCES [dbo].[CITIES] ([Id])
);