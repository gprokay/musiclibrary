using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicLibrary.Model;
using MusicLibrary.Services.Library;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MusicLibrary.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("track")]
    public class TrackController : Controller
    {
        private readonly TrackService service;

        public TrackController(TrackService trackService)
        {
            service = trackService;
        }

        [HttpPost("search")]
        public Task<ListResult<Track>> SearchTracks([FromBody]SearchTrackRequest request)
        {
            return service.SearchTracks(request.Filter, request.Page, HttpContext.User);
        }

        [HttpPost("library/{trackId}")]
        public Task AddTrackToLibrary(int trackId)
        {
            return service.ToggleLibrary(trackId, true, HttpContext.User);
        }

        [HttpDelete("library/{trackId}")]
        public Task RemoveTrackFromLibrary(int trackId)
        {
            return service.ToggleLibrary(trackId, false, HttpContext.User);
        }

        [HttpGet("{trackId}")]
        public Task<Track> Get(int trackId)
        {
            return service.GetById(trackId, HttpContext.User);
        }
    }
}
