CREATE TRIGGER UpdateAverage
	ON [dbo].[ENTRIES]
	AFTER INSERT
	AS
	BEGIN
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
	END
