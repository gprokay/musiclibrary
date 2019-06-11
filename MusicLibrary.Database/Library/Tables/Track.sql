CREATE TABLE [Library].[Track]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1)
	,[Title] NVARCHAR(2000) NOT NULL
	,[AlbumId] INT NULL FOREIGN KEY REFERENCES [Library].Album(Id)
	,[DiscNumber] INT NOT NULL
	,[TrackNumber] INT NOT NULL
	,[Duration] TIME NOT NULL
	,[CreatedOn] DATETIMEOFFSET(2) NOT NULL
)

GO

CREATE INDEX [IX_Tracks_Title] ON [Library].[Track] ([Title])

GO