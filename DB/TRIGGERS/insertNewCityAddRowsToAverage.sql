CREATE TRIGGER SetUpCityAverage_Trigger
ON [dbo].[CITIES]
AFTER INSERT
AS
BEGIN
	INSERT INTO CityAverage (particle, cityId)
	SELECT DISTINCT name, (SELECT inserted.id FROM inserted)
	FROM Entries;
	
END;