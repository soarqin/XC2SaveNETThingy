DROP TABLE IF EXISTS [FavCategories];
CREATE TABLE [FavCategories] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FavCategories] PRIMARY KEY ([ID])
);
