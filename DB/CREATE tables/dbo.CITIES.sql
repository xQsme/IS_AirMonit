USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[CITIES] Script Date: 18/12/2017 11:09:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[CITIES];


GO
CREATE TABLE [dbo].[CITIES] (
    [Id]        INT        IDENTITY (1, 1) NOT NULL,
    [name]      NCHAR (50) NOT NULL,
    [latitude]  FLOAT (53) NOT NULL,
    [longitude] FLOAT (53) NOT NULL
);


