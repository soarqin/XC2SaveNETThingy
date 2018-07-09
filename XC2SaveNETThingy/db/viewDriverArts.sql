DROP VIEW IF EXISTS [viewDriverArts];
CREATE VIEW [viewDriverArts]
AS
SELECT
ID as ID,
(SELECT CASE(ID)
WHEN 0 THEN (Name)
ELSE (Name || " [" || (SELECT Name FROM Weapons WHERE ID = WeaponID) || "] (" || (SELECT Name FROM Drivers WHERE ID = DriverID) || ")")
END) AS FriendlyName
FROM DriverArts;
