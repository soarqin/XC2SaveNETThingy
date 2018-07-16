DROP VIEW IF EXISTS [viewWeaponNames];
CREATE VIEW [viewWeaponNames]
AS
SELECT
ID as ID,
(SELECT CASE(ID)
WHEN 0 THEN (Name)
ELSE (Name || " [" || (SELECT Name FROM Items WHERE ID = CoreChipID) || "]")
END) AS FriendlyName
FROM WeaponNames;