USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[Alarms] Script Date: 18/12/2017 11:08:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Alarms];


GO
CREATE TABLE [dbo].[Alarms] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Particle]        NVARCHAR (50)  NOT NULL,
    [Condition]       NVARCHAR (10)  NOT NULL,
    [ConditionValue1] NUMERIC (6, 2) NOT NULL,
    [ConditionValue2] NUMERIC (6, 2) NULL,
    [EntryValue]      NUMERIC (6, 2) NOT NULL,
    [Message]         NVARCHAR (100) NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [CityId]          INT            NOT NULL
);


