DROP TABLE IF EXISTS [FavCategories];
CREATE TABLE [FavCategories] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FavCategories] PRIMARY KEY ([ID])
);
