CREATE TABLE [Library].[TrackGenre]
(
	[TrackId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Track(Id)
	,[GenreId] INT NOT NULL FOREIGN KEY REFERENCES [Library].Genre(Id)
	CONSTRAINT PK_TrackGenre PRIMARY KEY (TrackId, GenreId)
)
