CREATE TABLE [UserLibrary].[UserTrack]
(
	[Id] INT NOT NULL IDENTITY(1,1)
	,[TrackId] INT NOT NULL FOREIGN KEY REFERENCES [Library].[Track](Id)
	,[UserId] INT NOT NULL FOREIGN KEY REFERENCES [Auth].[User](Id)
	,CONSTRAINT [PK_UserTrack] PRIMARY KEY NONCLUSTERED ([UserId],[TrackId])	
)

GO

CREATE CLUSTERED INDEX [IX_UserTrack_ClusteredId] ON [UserLibrary].[UserTrack] ([Id])

GO