using backend.Contracts.v1.DTO;
using backend.Contracts.v1.Requests;
using backend.Contracts.v1.Responses;
using backend.Data;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace backend.Controllers.v1.WallController;

[ApiController]
[Route("v1/walls")]
public class WallController : ControllerBase
{
    private string _wallDir = Path.Combine("Content", "walls");
    private readonly ILogger<WallController> _logger;
    private readonly IWallRepository _wallRepo;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public WallController(ILogger<WallController> logger, ApplicationDbContext context, IWallRepository wallRepo, UserManager<User> userManager)
    {
        _logger = logger;
        _wallRepo = wallRepo;
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("get")]
    public async Task<ActionResult<LatestWallResponse>> Get(string wallUri)
    {
        /*
        Console.WriteLine(img);
        Console.WriteLine(img is null);
        Wall? wall;
        if (img is null)
        {
            wall = _context.Walls
            .Include(w => w.PolygonHolds).ThenInclude(p => p.Points)
            .Include(w => w.EllipseHolds)
            .Include(w => w.Neighbours)
            .AsSplitQuery()
            .OrderByDescending(w => w.Uploaded)
            .FirstOrDefault();
        }
        else
        {
            wall = _context.Walls
            .Include(w => w.PolygonHolds).ThenInclude(p => p.Points)
            .Include(w => w.EllipseHolds)
            .Include(w => w.Neighbours)
            .Where(w => w.URI == img)
            .FirstOrDefault();
        }
        if (wall is null)
        {
            return BadRequest("NO WALL");
        }
        _logger.LogInformation("Fetched wall succesfull");
        return Ok(new LatestWallResponse(wall));
        */
        var wall = await _context.Walls.Include(w => w.ClimbingHolds).ThenInclude(ch => ch.Points).FirstAsync(w => w.URI == wallUri);
        if(wall is null)
            return BadRequest($"Could not find wall {wallUri}");
        return Ok(new LatestWallResponse(uri: wall.URI, holds: wall.ClimbingHolds.Select(ch => ch.TransfromToDTO()).ToArray()));
    }

    [HttpGet]
    [Route("all")]
    public IActionResult GetAllWalls()
    {
        return Ok(_context.Walls.Select(w => new SimpleWallDto() {Name=w.Name, URI=w.URI,}));
    }

    [HttpPost]
    [Route("upload")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Upload([FromForm] string JsonData, IFormFile wallImage)
    {
        UploadWallRequest? request = JsonConvert.DeserializeObject<UploadWallRequest>(JsonData);
        if (request is null) return BadRequest("Could not parse");
        var fingerPrint = new Random().Next().ToString("X") + ".jpg";
        var filePath = Path.Combine(_wallDir, fingerPrint);
        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await wallImage.CopyToAsync(fileStream);
        }
        var user = await _userManager.GetUserAsync(User);
        var wall = new Wall(filePath, request.Name, user);
        wall.ClimbingHolds.AddRange(request.ClimbingHolds.Select((ch, i) => ClimbingHold.Generate(i, ch)));
        await _context.AddAsync(wall);
        await _context.SaveChangesAsync();
        //var result = await _wallRepo.AddNewWall(fingerPrint, request.Name, request.ClimbingHolds);
        return Ok();
    }
}