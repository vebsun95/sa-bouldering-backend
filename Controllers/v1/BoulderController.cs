using backend.Contracts.v1.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using backend.Models;
using backend.Repositories;
using backend.Data;
using backend.Contracts.v1.DTO;
using backend.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers.v1.RoutesController;

[ApiController]
[Route("v1/boulders")]
public class BoulderController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<BoulderController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly IWallRepository _wallRepo;
    private readonly IBoulderRepository _routesRepo;
    private readonly IEnumerable<IValidator<NewBoulderRequest>> _validators;

    public BoulderController(ILogger<BoulderController> logger,
                             UserManager<User> userManager,
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
    public async Task<ActionResult<BoulderDto[]>> GetAll(string wallUri)
    {
        /*
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
        */
        var wall = await _context.Walls
            .Include(w => w.Boulders)
            .ThenInclude(b => b.Holds)
            .ThenInclude(bh => bh.ClimbingHold)
            .FirstAsync(w => w.URI == wallUri);
        if (wall is null) return BadRequest($"Could not find wall {wallUri}");
        return Ok(wall.Boulders.Select(b => new BoulderDto
        (
            name: b.Name,
            fA: b.FA.UserName,
            grade: b.Grade,
            ascents: b.Ascents.Count,
            @checked: false,
            holds: b.Holds.Select(h => new BoulderClimbingHoldDto(h.ClimbingHold.Index, (int)h.Type) ).ToArray()
        )));
    }

    [HttpPost]
    [Route("upload")]
    [Authorize]
    public async Task<ActionResult> Upload(NewBoulderRequest request)
    {
        /*
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
        */
        var wall = await _context.Walls
            .Include(w => w.Boulders)
            .Include(w => w.ClimbingHolds).ThenInclude(ch => ch.Points)
            .FirstAsync(w => w.URI == request.WallUri);
        if(wall is null) return BadRequest();
        var index = wall.Boulders.Count;
        var user = await _userManager.GetUserAsync(User);
        var boulder = new Boulder
        (
            index: index,
            name : request.Name,
            fa   : user,
            grade: request.Grade
        );
        var holds = wall.ClimbingHolds.OrderBy(h => h.Index).ToArray();
        boulder.Holds.AddRange
        (
            request.holds.Select(requestHold => 
                new ClimbingHoldBoulder() 
                    {
                        ClimbingHold = holds[requestHold.HoldIndex],
                        Type=(ClimbingHoldBoulder.TYPES)requestHold.HoldType
                    })
        );
        await _context.Boulders.AddAsync(boulder);
        await _context.SaveChangesAsync();
        return Ok();

    }

}