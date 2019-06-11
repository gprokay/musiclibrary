using System;
using System.IO;
using File = TagLib.File;

namespace MusicLibrary.Services.Library
{
    public class StreamedFileAbstraction : File.IFileAbstraction
    {
        public StreamedFileAbstraction(string name, Stream readStream)
        {
            Name = name;
            ReadStream = readStream;
        }

        public void CloseStream(Stream stream)
        {
            stream.Position = 0;
        }

        public string Name { get; }

        public Stream ReadStream { get; }

        public Stream WriteStream
        {
            get { throw new NotSupportedException(); }
        }
    }
}
