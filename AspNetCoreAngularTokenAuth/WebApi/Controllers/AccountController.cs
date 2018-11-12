using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Services;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AppSettings _appSettings { get; set; }
        private readonly IAuthenticationService _authenticationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSenderService _emailSenderService;

        public AccountController(IAuthenticationService authenticationService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSenderService emailSenderService,
            IOptions<AppSettings> settings)
        {
            _authenticationService = authenticationService;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSenderService = emailSenderService;
            _appSettings = settings.Value;
        }

        [AllowAnonymous]
        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok(true);
        }

        // This is the authentication method for the API. If the username and password is correct, we will send an auth token to the user to use on subsuquent requests (until it expires)
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody]LoginDto login)
        {
            var loginResultDto = new LoginResultDto();

            var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, true, lockoutOnFailure: false); // Check the user's email and password
            if (result.Succeeded)
            {
                var token = _authenticationService.GenerateAuthToken(login.Email, _appSettings.Secret); // The login was successful so generate a auth token and send it to the user along with a refresh token

                if (string.IsNullOrEmpty(token))
                    return BadRequest(new { message = "Username or password is incorrect" });

                var user = await _userManager.FindByEmailAsync(login.Email);

                _authenticationService.RemoveRefreshTokenByUser(user.Id, "providername", "refresh", "phone"); // Remove any existing refresh tokens for the user so that we can add the new one
                var newRefreshToken = _authenticationService.GenerateRefreshToken(); // Generate a new refresh token
                _authenticationService.AddRefreshToken(user.Id, "providername", "refresh", newRefreshToken, "phone"); // Save the new refresh token

                loginResultDto.WasSuccessful = true;
                loginResultDto.Token = token;
                loginResultDto.RefreshToken = newRefreshToken;

                return Ok(loginResultDto);
            }

            loginResultDto.WasSuccessful = false;

            return Ok(loginResultDto);
        }

        // This method is called then the user's token is expired and they need a new one. They send up a refresh token to prove who they are. If the refresh token looks good, we'll send them a new auth token
        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody]RefreshTokenModelDto refreshTokenModelDto)
        {
            var refreshTokenResultDto = new RefreshTokenResultDto();

            if (string.IsNullOrEmpty(refreshTokenModelDto.Token) || string.IsNullOrEmpty(refreshTokenModelDto.RefreshToken)) // Make sure they have a refresh token
            {
                refreshTokenResultDto.WasSuccessful = false;
                return Ok(refreshTokenResultDto);
            }

            string loginProvier = "providername";

            var savedRefreshToken = _authenticationService.GetRefreshTokenByToken(loginProvier, "refresh", refreshTokenModelDto.RefreshToken, "phone"); // Retrieve the refresh token  fromt he database AspNetToken table

            if (savedRefreshToken == null || savedRefreshToken.Value != refreshTokenModelDto.RefreshToken) // If there's no matching refresh token in the database we have a problem
            {
                refreshTokenResultDto.WasSuccessful = false;
                return Ok(refreshTokenResultDto);
            }

            var newJwtToken = _authenticationService.GenerateAuthToken(savedRefreshToken.UserId, _appSettings.Secret); // The refresh token looks good, so let's generate a new auth token
            var newRefreshToken = _authenticationService.GenerateRefreshToken(); // Generate a new reresh token as well
            _authenticationService.RemoveRefreshToken(loginProvier, "refresh", refreshTokenModelDto.RefreshToken, "phone"); // Remove any existing refresh tokens
            _authenticationService.AddRefreshToken(savedRefreshToken.UserId, loginProvier, "refresh", newRefreshToken, "phone"); // Add the new refresh token

            refreshTokenResultDto.Token = newJwtToken;
            refreshTokenResultDto.RefreshToken = newRefreshToken;
            refreshTokenResultDto.WasSuccessful = true;

            return Ok(refreshTokenResultDto);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {
            var user = new ApplicationUser { UserName = register.Email, Email = register.Email };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                register.WasSuccessful = true;

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail", //Not built yet
                    pageHandler: null,
                    values: new { userId = user.Id, code = code },
                    protocol: Request.Scheme);

                await _emailSenderService.SendEmail(register.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }
            else
            {
                register.WasSuccessful = false;
            }

            return Ok(register);
        }

        [Authorize]
        [HttpGet("list")]
        public IActionResult list()
        {
            return Ok(new { Message = "You got an authorized response!" });
        }

        [HttpGet("GetUnAuthorizedRequest")]
        public IActionResult GetUnAuthorizedRequest()
        {
            return Ok(new { Message = "You got a public response!" });
        }
    }
}