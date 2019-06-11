CREATE TABLE [Library].[Artist]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[Name] NVARCHAR(2000) UNIQUE, 
    CONSTRAINT [PK_Artists] PRIMARY KEY ([Id])
)

GO

CREATE UNIQUE INDEX [IX_Artists_Name_Unique] ON [Library].[Artist] ([Name])

GO