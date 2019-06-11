CREATE TABLE [Library].[TrackArtist]
(
	[TrackId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Track(Id)
	,[ArtistId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Artist(Id)
	CONSTRAINT PK_TrackArtists PRIMARY KEY (TrackId, ArtistId)
)
