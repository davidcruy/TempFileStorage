CREATE TABLE [TempFileStorage] (
   [Key] nvarchar(10) NOT NULL,
   [Filename] nvarchar(max) NOT NULL,
   [Filesize] bigint NOT NULL,
   [CacheTimeout] datetime2 NOT NULL,
   [Content] varbinary(max) NOT NULL,

   CONSTRAINT [PK_Key] PRIMARY KEY CLUSTERED ([Key] ASC) 
);