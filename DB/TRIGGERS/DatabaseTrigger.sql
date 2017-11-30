CREATE TRIGGER SetUpCityAverage_Trigger
ON dbo.CITIES
AFTER INSERT
AS
BEGIN
	INSERT INTO CityAverage (particle, cityId)
	SELECT DISTINCT name, cityId 
	FROM Entries e 
	WHERE name NOT IN 
			(SELECT particle FROM CityAverage) 
		OR cityId NOT IN 
			(SELECT cityId FROM CityAverage WHERE name = particle);
END;
GO