using backend.Contracts.v1.DTO;

namespace backend.Contracts.v1.Requests;

public class UploadWallRequest {
    public string Name {get; set;}
    public EllipseHoldDto[] Ellipseholds {get; set;}
    public PolygonHoldDto[] PolygonHolds {get; set;}
}
