USE [db43a269ccada04e03bdb2a83100ad06e2]
GO

/****** Object: Table [dbo].[CityAverage] Script Date: 18/12/2017 11:10:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

DROP TABLE [dbo].[CityAverage];


GO
CREATE TABLE [dbo].[CityAverage] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Year]     SMALLINT       NULL,
    [MONTH]    SMALLINT       NULL,
    [WEEK]     SMALLINT       NULL,
    [cityId]   INT            NOT NULL,
    [particle] NCHAR (5)      NOT NULL,
    [sum]      BIGINT         NULL,
    [count]    INT            NULL,
    [average]  DECIMAL (6, 2) NULL,
    [min]      DECIMAL (6, 2) NULL,
    [max]      DECIMAL (6, 2) NULL
);


