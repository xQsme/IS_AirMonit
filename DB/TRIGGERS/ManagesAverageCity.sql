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
			average = ((SUM + @value) / (COUNT + 1))
		WHERE Id = (SELECT ca.Id 
							FROM CityAverage ca
							WHERE @year = ca.Year AND
								  @month = ca.Month AND
								  @week = ca.Week AND
								  ca.cityId = @cityId AND
								  @particle = ca.particle)
	END;