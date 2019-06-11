CREATE TABLE [UserLibrary].[UserAlbum]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[AlbumId] INT NOT NULL FOREIGN KEY REFERENCES [Library].[Album](Id)
	,[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Auth].[User](Id)
	,CONSTRAINT [PK_UserAlbum] PRIMARY KEY NONCLUSTERED ([UserId],[AlbumId])	
)

GO

CREATE CLUSTERED INDEX [IX_UserAlbum_ClusteredId] ON [UserLibrary].[UserAlbum] ([Id])

GO