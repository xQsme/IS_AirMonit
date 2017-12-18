USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[Sensors] Script Date: 18/12/2017 11:11:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Sensors];


GO
CREATE TABLE [dbo].[Sensors] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [name]   NVARCHAR (20)  NOT NULL,
    [value]  DECIMAL (6, 2) NOT NULL,
    [date]   DATETIME       NOT NULL,
    [cityId] INT            NOT NULL
);


