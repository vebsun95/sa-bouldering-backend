using backend.Contracts.v1.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using backend.Models;
using backend.Repositories;
using backend.Data;
using backend.Contracts.v1.DTO;
using backend.SharedKernel;

namespace backend.Controllers.v1.RoutesController;

[ApiController]
[Route("v1/boulders")]
public class BoulderController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<BoulderController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWallRepository _wallRepo;
    private readonly IBoulderRepository _routesRepo;
    private readonly IEnumerable<IValidator<NewBoulderRequest>> _validators;

    public BoulderController(ILogger<BoulderController> logger,
                             UserManager<ApplicationUser> userManager,
                             IWallRepository wallRepo,
                             ApplicationDbContext context,
                             IBoulderRepository routesRepo,
                             IEnumerable<IValidator<NewBoulderRequest>> validators)
    {
        _logger = logger;
        _userManager = userManager;
        _wallRepo = wallRepo;
        _context = context;
        _routesRepo = routesRepo;
        _validators = validators;
    }

    [HttpGet]
    [Route("get/all")]
    public async Task<ActionResult<BoulderDto[]>> GetAll()
    {
        var wall = _context.Walls.OrderByDescending(w => w.Uploaded).First();
        var boulders = _context.Boulders
        .Where(b => b.Wall == wall)
        .Select(b => new BoulderDto(
            b.Name,
            b.FA.UserName,
            b.Grade,
            _context.Ascents.Where(a => a.Route == b).Count(),
            false,
            b.Holds.Select(h => new HoldIndexDto(h.Index, h.Type)).ToArray()
            ));
        return Ok(boulders);
    }

    [HttpPost]
    [Route("upload")]
    [Authorize]
    public async Task<ActionResult> Upload(NewBoulderRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        var errors = _validators.Select(v => v.isValid(request))
        .Where(result => !result.IsValid)
        .Select(result => result.Error).ToArray();

        if(errors.Length > 1) {
            return BadRequest(String.Join(", ", errors));
        }

        var wall = _context.Walls.OrderByDescending(w => w.Uploaded).First();
        var index = _context.Boulders.Where(b => b.Wall == wall).Count();
        var boulder = new Boulder(
            index, wall, request.name, user, request.grade
        );
        boulder.Holds.AddRange(request.holds.Select(h => new HoldIndex(h.Index, h.Type)));
        await _context.AddAsync(boulder);
        await _context.SaveChangesAsync();
        return Ok(boulder.Index);
    }

}