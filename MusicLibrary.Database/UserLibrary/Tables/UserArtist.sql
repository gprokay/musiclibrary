CREATE TABLE [UserLibrary].[UserArtist]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[ArtistId] INT NOT NULL FOREIGN KEY REFERENCES [Library].[Artist](Id)
	,[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Auth].[User](Id)
	,CONSTRAINT [PK_UserArtist] PRIMARY KEY NONCLUSTERED ([UserId],[ArtistId])	
)

GO

CREATE CLUSTERED INDEX [IX_UserArtist_ClusteredId] ON [UserLibrary].[UserArtist] ([Id])

GO