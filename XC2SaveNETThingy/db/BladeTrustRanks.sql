DROP TABLE IF EXISTS [BladeTrustRanks];
CREATE TABLE [BladeTrustRanks] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_BladeTrustRanks] PRIMARY KEY ([ID])
);
