CREATE TRIGGER AddNewParticleToCityAverage
	ON [dbo].[ENTRIES]
	AFTER INSERT
	AS
	IF NOT EXISTS (SELECT particle
           FROM CityAverage, inserted WHERE particle = inserted.name
          )  
	BEGIN
		INSERT INTO CityAverage (cityId, particle)
			SELECT DISTINCT ca.cityId, inserted.name
			FROM inserted, CityAverage ca
	END
