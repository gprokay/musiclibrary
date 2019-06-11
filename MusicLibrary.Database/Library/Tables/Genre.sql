CREATE TABLE [Library].[Genre]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[Name] NVARCHAR(500) NOT NULL UNIQUE
    ,CONSTRAINT [PK_Genres] PRIMARY KEY ([Id])
)

GO

CREATE UNIQUE INDEX [IX_Genres_Name_Unique] ON [Library].[Genre] ([Name])

GO

