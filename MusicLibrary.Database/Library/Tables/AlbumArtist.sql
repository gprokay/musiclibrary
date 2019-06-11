CREATE TABLE [Library].[AlbumArtist]
(
	[AlbumId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Album(Id)
	,[ArtistId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Artist(Id)
	CONSTRAINT PK_AlbumArtists PRIMARY KEY (AlbumId, ArtistId)
)
