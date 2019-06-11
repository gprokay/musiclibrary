CREATE TABLE [UserLibrary].[PlaylistTrack]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[TrackId] INT NOT NULL FOREIGN KEY REFERENCES [Library].[Track](Id)
	,[PlaylistId] INT NOT NULL FOREIGN KEY REFERENCES [UserLibrary].[Playlist](Id)
	,CONSTRAINT [PK_PlaylistTrack] PRIMARY KEY NONCLUSTERED ([PlaylistId],[TrackId])	
)


GO

CREATE CLUSTERED INDEX [IX_PlaylistTrack_ClusteredId] ON [UserLibrary].[PlaylistTrack] ([Id])

GO