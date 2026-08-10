DROP TABLE IF EXISTS [ItemStatus];
CREATE TABLE [ItemStatus] (
  [ItemID] INTEGER NOT NULL UNIQUE
, [Status] text NOT NULL
, [Reason_EN] text NOT NULL
, [Reason_CN] text NOT NULL
, [Evidence] text NOT NULL
, CONSTRAINT [PK_ItemStatus] PRIMARY KEY ([ItemID])
, CONSTRAINT [CK_ItemStatus_Status] CHECK ([Status] = 'Reserved')
, FOREIGN KEY ([ItemID]) REFERENCES [Items] ([ID]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
