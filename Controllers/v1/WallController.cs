using backend.Contracts.v1.Requests;
using backend.Contracts.v1.Responses;
using backend.Data;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Authorization;
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

    public WallController(ILogger<WallController> logger, ApplicationDbContext context, IWallRepository wallRepo)
    {
        _logger = logger;
        _wallRepo = wallRepo;
        _context = context;
    }

    [HttpGet]
    [Route("get")]
    public ActionResult<LatestWallResponse> Get(string? img)
    {
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
        foreach (var eHold in request.Ellipseholds)
        {
            Console.WriteLine(eHold.Cy);
            Console.WriteLine(eHold.Cx);
        }
        var result = await _wallRepo.AddNewWall(fingerPrint, request.Name, request.Ellipseholds, request.PolygonHolds);
        return result ? Ok() : BadRequest("Failed to add new wall");
    }
}