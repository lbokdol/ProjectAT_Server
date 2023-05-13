using Common.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    /*
     * 이곳엔 게임 내 맵에대한 설정과 게임 진행 중 발생하는 이벤트들을 처리하는 기능을 구현해야된다
     * 해당 맵에 있는 유저들에게만 공지를 보내는 것도 포함
     */
    [ApiController]
    [Route("[controller]")]
    public class WorldController : Controller
    {
        private Service.BackOfficeChannel _channel;

        public WorldController(Service.BackOfficeChannel channel)
        {
            _channel = channel;
        }

        [HttpPost("notice/{worldId}")]
        public async Task<IActionResult> Notice(int worldId)
        {
            // TODO: 월드 아이디를 통해 해당 월드에 있는 유저들에게 공지를 보내는 기능 구현해야됨
            return Ok();
        }

        // TODO: 월드 아이디로 보내는 것 뿐만아니라 같은 유형을 가진 월드들에 보낼 수 있도록 클래스 만들어야됨
    }
}
