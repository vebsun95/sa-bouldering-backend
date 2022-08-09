using backend.Contracts.v1.DTO;
using backend.Contracts.v1.Requests;
using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Repositories;

public interface IWallRepository {
    bool CheckForExisting();
    Wall GetLatestWall();
    Task<bool> AddNewWall(string imgURI, string name, EllipseHoldDto[] eHolds, PolygonHoldDto[] pHolds);
}

public class WallRepository : IWallRepository {
    private readonly ILogger<WallRepository> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private Wall? _wall;

    public WallRepository(ILogger<WallRepository> logger, IServiceScopeFactory scopeFactory) {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    private async void setWall() {
        using(var scope = _scopeFactory.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _wall = await _context.Walls
                .Include(w => w.PolygonHolds).ThenInclude(p => p.Points)
                .Include(w => w.EllipseHolds)
                .Include(w => w.Neighbours)
                .OrderByDescending(w => w.Id).FirstAsync();
        }
    }
    public bool CheckForExisting() {
        if(_wall is null) {
            setWall();
        }
        return _wall is not null;
    }
    public Wall GetLatestWall() {
        if (_wall is null) {
            setWall();
        }
        return _wall!;
    }
    public async Task<bool> AddNewWall(string imgURI, string name, EllipseHoldDto[] eHoldsDtos, PolygonHoldDto[] pHoldsDtos) {
        using(var scope = _scopeFactory.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var wall = new Wall(imgURI, name); 
            int indexCounter = 0;
            var grid = new List<(float x,float y, int idx)>[20];
            grid = grid.Select(l => new List<(float x, float y, int idx)>()).ToArray();
            foreach(var eHoldDto in eHoldsDtos) {
                var gridRow = (int)(eHoldDto.Cy/0.05);
                grid[gridRow].Add((eHoldDto.Cx, eHoldDto.Cy, indexCounter));
                var eHold = new EllipseHold(
                    wall: wall,
                    index: indexCounter,
                    cx: eHoldDto.Cx,
                    cy: eHoldDto.Cy,
                    rx: eHoldDto.Rx,
                    ry: eHoldDto.Ry,
                    rotation: eHoldDto.Rotation % 360
                );
                await _context.EllipseHolds.AddAsync(eHold);
                indexCounter++;
                wall.AddEllipseHold(eHold);
            }
            foreach(var pHoldDto in pHoldsDtos) {
                (float avgX, float avgY) = getAvgOfPoints(pHoldDto.Points);
                var gridRow = (int)(avgY/0.05);
                grid[gridRow].Add((avgX, avgY, indexCounter));
                var pHold = new PolygonHold(wall: wall, index: indexCounter);
                foreach(var point in pHoldDto.Points) {
                    pHold.AddPoint(point.X ,point.Y);
                }
                await _context.PolygonHolds.AddAsync(pHold);
                indexCounter++;
                wall.AddPolygonHold(pHold);
            }
            foreach(var row in grid) {
                row.Sort((r1, r2) => r1.x.CompareTo(r2.x));
            }
            var NGTC = new NeighbourGridTupleComparer();
            for(var row=0; row < grid.Length; row++) {
                for(var col=0; col < grid[row].Count; col++) {
                    // West
                    if(col > 0) {
                        wall.AddNeighbour(new Neighbour(){
                            Index=grid[row][col].idx,
                            NeighbourIndex=grid[row][col - 1].idx,
                            Direction="W",
                        });
                    }
                    // East
                    if(col < grid[row].Count - 2) {
                        wall.AddNeighbour(new Neighbour(){
                            Index=grid[row][col].idx,
                            NeighbourIndex=grid[row][col + 1].idx,
                            Direction="E",
                        });
                    }
                    // North
                    if(row > 0 && grid[row-1].Any()) {
                        var closestIndex = grid[row-1].BinarySearch(grid[row][col], NGTC);
                            closestIndex = closestIndex >= 0 ? closestIndex : Math.Max(~closestIndex - 1, 0);
                        wall.AddNeighbour(new Neighbour()
                        {
                            Index = grid[row][col].idx,
                            NeighbourIndex = grid[row - 1][closestIndex].idx,
                            Direction = "N",
                        });
                    }
                    // South
                    if(row < grid.Length - 2 && grid[row + 1].Any()) {
                        var closestIndex = grid[row+1].BinarySearch(grid[row][col], NGTC);
                            closestIndex = closestIndex >= 0 ? closestIndex : Math.Max(~closestIndex - 1, 0);
                        wall.AddNeighbour(new Neighbour()
                        {
                            Index=grid[row][col].idx,
                            NeighbourIndex=grid[row + 1][closestIndex].idx,
                            Direction="S",
                        });
                    }
                }
            }
            await _context.Walls.AddAsync(wall);
            await _context.SaveChangesAsync();
        }
        return true;
    }
    
    private (float, float) getAvgOfPoints(PointDto[] points) {
        var n = points.Count();
        float sumX = 0;
        float sumY = 0;
        foreach(var point in points) {
            sumX += point.X;
            sumY += point.Y;
        }
        return (sumX / n, sumY / n);
    }
}

public class NeighbourGridTupleComparer : IComparer<(float x, float y, int idx)>
{
    public int Compare((float x, float y, int idx) a, (float x, float y, int idx) b)
    {
        if(a.x == b.x) {
            return 0;
        }
        if(a.x < b.x) {
            return 1;
        }
        return -1;
    }
}