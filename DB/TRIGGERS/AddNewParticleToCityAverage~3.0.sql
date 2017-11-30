CREATE TRIGGER AddNewParticleToCityAverage
	ON [dbo].[ENTRIES]
	AFTER INSERT
	AS
	IF NOT EXISTS (SELECT particle
           FROM CityAverage, inserted WHERE particle = inserted.name
          )
	BEGIN TRY
		INSERT INTO CityAverage (cityId, particle)
			SELECT DISTINCT ca.cityId, inserted.name
			FROM inserted, CityAverage ca
		UPDATE CityAverage
	SET SUM = (SUM + ins.value),
		COUNT = (COUNT + 1)
	FROM inserted ins
	WHERE ins.cityId = (SELECT cityId 
						FROM CityAverage ca
						WHERE YEAR(ins.date) = ca.Year AND
							  MONTH(ins.date) = ca.Month AND
							  DATEPART( wk,ins.date) = ca.Week AND
							  ca.cityId = ins.cityId AND
							  ca.particle = ins.name)
	END TRY
BEGIN CATCH
    PRINT 'Error on line ' + CAST(ERROR_LINE() AS VARCHAR(10))
    PRINT ERROR_MESSAGE()
END CATCH
	ELSE
	BEGIN TRY
	UPDATE CityAverage
	SET SUM = (SUM + ins.value),
		COUNT = (COUNT + 1)
	FROM inserted ins
	WHERE ins.cityId = (SELECT cityId 
						FROM CityAverage ca
						WHERE YEAR(ins.date) = ca.Year AND
							  MONTH(ins.date) = ca.Month AND
							  DATEPART( wk,ins.date) = ca.Week AND
							  ca.cityId = ins.cityId AND
							  ca.particle = ins.name)
	END TRY
BEGIN CATCH
    PRINT 'Error on line ' + CAST(ERROR_LINE() AS VARCHAR(10))
    PRINT ERROR_MESSAGE()
END CATCH
