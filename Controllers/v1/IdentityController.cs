using backend.Data;
using backend.Models;
using backend.Contracts.v1.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.v1.IdentityController;

[ApiController]
[Route("v1/identity")]
public class IdentityController : ControllerBase
{
    private readonly ILogger<IdentityController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleMangager;
    private readonly ApplicationDbContext _context;

    public IdentityController(ILogger<IdentityController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _roleMangager = roleManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;        
    }
    [HttpGet]
    [Route("create_test")]
    public  ActionResult CreateTest() {
        var user = new ApplicationUser() {UserName="vebsun", Email="vebsun@mail.no"};
         _userManager.CreateAsync(user, "passord").Wait();
         _roleMangager.CreateAsync(new IdentityRole() {Name="admin"}).Wait();
         _userManager.AddToRoleAsync(user, "admin").Wait();
        return Ok();
    }
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult> Login(LoginRequest input)
    {
        Console.WriteLine(input.Username);
        Console.WriteLine(input.Password);
        var result = await _signInManager.PasswordSignInAsync(userName: input.Username, password: input.Password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register(LoginRequest input)
    {
        var user = new ApplicationUser();
        user.UserName = input.Username;
        var result = await _userManager.CreateAsync(user, input.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }
    [HttpGet]
    [Route("logout")]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}