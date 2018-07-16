DROP TABLE IF EXISTS [ItemTypes];
CREATE TABLE [ItemTypes] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_ItemTypes] PRIMARY KEY ([ID])
);
