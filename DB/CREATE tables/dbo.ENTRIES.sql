USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[ENTRIES] Script Date: 18/12/2017 11:10:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[ENTRIES];


GO
CREATE TABLE [dbo].[ENTRIES] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [name]   NCHAR (5)      NOT NULL,
    [value]  NUMERIC (6, 2) NOT NULL,
    [date]   DATETIME       NOT NULL,
    [cityId] INT            NOT NULL
);


