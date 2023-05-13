using Microsoft.AspNetCore.Mvc;
using Common.Objects;
using BackOfficeRpcService;

namespace BackOffice.Controllers
{
    /*
     * 이곳엔 계정과 관련된 정보를 얻거나 수정할 수 있도록 기능을 구현해야된다
     */
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
        public async Task<IActionResult> RegistAccount([FromBody] Common.Objects.Account account)
        {
            var validation = account.Validate();
            if (validation != ResultCode.SUCCESS)
                return BadRequest(validation);

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

        [HttpGet("payment/{id}")]
        public async Task<IActionResult> GetAccountPaymentInformation(int id)
        {
            // TODO: 특정 유저 결제 정보 조회 구현해야됨

            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeAccountInformantion(int id)
        {
            // TODO: 특저 유저의 닉네임이나 기타 정보들 수정하는 기능 구현해야됨

            return Ok();
        }
    }
}
