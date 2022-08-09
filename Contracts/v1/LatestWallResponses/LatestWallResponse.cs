using backend.Contracts.v1.DTO;
using backend.Models;

namespace backend.Contracts.v1.Responses;

public class LatestWallResponse {
    public string URI {get; set;}
    public PolygonHoldDto[] PolygonHolds {get; set;}
    public EllipseHoldDto[] EllipseHolds {get; set;}
    public NeighbourDto[]   Neighbours {get; set;}
    public LatestWallResponse(Wall wall) {
        URI = wall.URI;
        PolygonHolds = wall.PolygonHolds.Select(hold => new PolygonHoldDto(){
            Points=hold.Points.Select(point => new PointDto(){X=point.X, Y=point.Y}).ToArray(),
            Index=hold.Index
        }).ToArray();
        EllipseHolds = wall.EllipseHolds.Select(hold => new EllipseHoldDto(){
            Rx=hold.Rx, Ry=hold.Ry, Cx=hold.Cx, Cy=hold.Cy, 
            Rotation=hold.Rotation, Index=hold.Index}).ToArray();
        Neighbours   = wall.Neighbours.Select(hold => new NeighbourDto(){Index=hold.Index, NeighbourIndex=hold.NeighbourIndex, Direction=hold.Direction} ).ToArray();
    }
}


