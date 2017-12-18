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

CREATE TRIGGER AddNewParticleToCityAverage
    ON [dbo].[ENTRIES]
    AFTER INSERT
    AS
    DECLARE @week int;
    DECLARE @month int;
    DECLARE @year int;
    DECLARE @particle nchar(5);
    DECLARE @value numeric(6,2);
    DECLARE @cityId int;
    SELECT @week = DATEDIFF(WEEK, DATEADD(MONTH, DATEDIFF(MONTH, 0, ins.date), 0), ins.date) +1, 
            @month = MONTH(ins.date), 
            @year = YEAR(ins.date), 
            @particle = ins.name,
            @value = ins.value,
            @cityId = ins.cityId
    FROM inserted ins;
    IF NOT EXISTS (SELECT particle
                  FROM CityAverage ca
                  WHERE @year = ca.Year AND
                    @month = ca.Month AND
                    @week = ca.Week AND
                    ca.cityId = @cityId AND
                    @particle = ca.particle)
    BEGIN
        INSERT INTO CityAverage (cityId, particle, Year, MONTH, WEEK, count, sum)
            VALUES (@cityId, @particle, @year, @month, @week, 1, @value)
    END
    ELSE
    BEGIN
        UPDATE CityAverage
        SET SUM = (SUM + @value),
            COUNT = (COUNT + 1),
            average = ((SUM + @value) / (COUNT + 1)),
            min = (CASE WHEN min > @value
                    THEN @value
                    ELSE min
                    END),
            max = (CASE WHEN max < @value
                    THEN @value
                    ELSE max
                    END)
        WHERE Id = (SELECT ca.Id 
                            FROM CityAverage ca
                            WHERE @year = ca.Year AND
                                  @month = ca.Month AND
                                  @week = ca.Week AND
                                  ca.cityId = @cityId AND
                                  @particle = ca.particle)
    END;