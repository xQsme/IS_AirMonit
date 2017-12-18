USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[Incidents] Script Date: 18/12/2017 11:10:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[Incidents];


GO
CREATE TABLE [dbo].[Incidents] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [message]    NVARCHAR (160) NOT NULL,
    [otherEvent] NCHAR (30)     NULL,
    [eventId]    INT            NULL,
    [publisher]  NCHAR (30)     NOT NULL,
    [date]       DATETIME       NOT NULL,
    [cityId]     INT            NOT NULL
);


