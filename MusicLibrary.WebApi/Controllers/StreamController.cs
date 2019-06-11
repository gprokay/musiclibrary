using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicLibrary.Services;
using System.Threading.Tasks;

namespace MusicLibrary.WebApi.Controllers
{
    [ApiController]
    [Route("stream")]
    public class StreamController : Controller
    {
        private readonly TrackStreamService service;

        public StreamController(TrackStreamService service)
        {
            this.service = service;
        }

        [HttpGet("{token}")]
        public async Task<FileStreamResult> GetTrackStream(string token)
        {
            var stream = await service.GetTrackAsStream(token, Request.HttpContext.Connection.RemoteIpAddress.ToString());
            return File(stream, "text/plain", enableRangeProcessing: true);
        }

        [HttpGet("token/{trackId}")]
        [Authorize]
        public string GetStreamingToken(int trackId)
        {
            return service.GetStreamingToken(trackId, HttpContext.User, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }
    }
}
