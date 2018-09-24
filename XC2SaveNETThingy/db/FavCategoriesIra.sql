DROP TABLE IF EXISTS [FavCategoriesIra];
CREATE TABLE [FavCategoriesIra] (
  [ID] INTEGER  NOT NULL UNIQUE
, [Name_EN] text NOT NULL
, [Name_CN] text NOT NULL
, CONSTRAINT [sqlite_master_PK_FavCategoriesIra] PRIMARY KEY ([ID])
);
