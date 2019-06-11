CREATE TABLE [UserLibrary].[PlayCount]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[UserId] INT NOT NULL REFERENCES [Auth].[User](Id),
	[TrackId] INT NOT NULL,
	[Count] INT NOT NULL,
	[Timestamp] DATETIMEOFFSET NOT NULL,
	CONSTRAINT [PK_PlayCount] PRIMARY KEY NONCLUSTERED ([UserId],[TrackId])	
)

GO

CREATE CLUSTERED INDEX [IX_PlayCount_ClusteredId] ON [UserLibrary].[PlayCount]([Id])

GO