using Microsoft.AspNetCore.Mvc;
using Common.Objects;
using BackOfficeRpcService;

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
        public async Task<IActionResult> AccountRegist([FromBody] Common.Objects.Account account)
        {
            if (account.Validate())
                return BadRequest("유효하지 않은 요청");

            var request = new RegisterReq
            {
                Id = Guid.NewGuid().ToString(),
                Username = account.Username,
                Email = account.Email,
                Password = account.Password,
                Emailverified = account.EmailVerified,
            };

            var response = await _channel.RegistAccount(request);
            return Ok(response);
        }
    }
}
