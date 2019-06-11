CREATE TABLE [Library].[TrackFile]
(
	[TrackId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Track(Id)
	,[MusicFileId] INT NOT NULL FOREIGN KEY REFERENCES [Library].MusicFile(Id)
	CONSTRAINT PK_TrackFiles PRIMARY KEY (TrackId, MusicFileId)
)
