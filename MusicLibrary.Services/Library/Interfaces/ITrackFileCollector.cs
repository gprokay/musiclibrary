using GP.Tools.DriveService;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public interface ITrackFileCollector
    {
        Task<IEnumerable<IFile>> FindTracksAsync(string startPath, CancellationToken? cancellationToken = null, IProgress<int> searchProgress = null);
    }
}