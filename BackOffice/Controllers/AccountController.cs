using Microsoft.AspNetCore.Mvc;
using Common.Objects;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private Service.BackOfficeChannel _channel;

        public AccountController(Service.BackOfficeChannel channel)
        {
            _channel = channel;
        }

        [HttpPost("regist")]
        public async Task<IActionResult> AccountRegist([FromBody] Account account)
        {
            if (account.Validate())
                return BadRequest("유효하지 않은 요청");

            var response = await _channel.Register(account);
            return Ok(response);
        }
    }
}
