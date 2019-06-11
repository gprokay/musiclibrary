using GP.Tools.DriveService;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MusicLibrary.Services.Library
{
    public interface ITrackInfoCollector
    {
        Task<List<TrackInfoResult>> GetTrackInfosAsync(IEnumerable<IFile> files, CancellationToken? cancellationToken = null, IProgress<int> progress = null);
    }
}
