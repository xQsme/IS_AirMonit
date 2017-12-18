USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[Events] Script Date: 18/12/2017 11:10:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Events];


GO
CREATE TABLE [dbo].[Events] (
    [Id]   INT        IDENTITY (1, 1) NOT NULL,
    [name] NCHAR (30) NOT NULL
);


