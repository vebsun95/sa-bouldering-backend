using backend.Models;

namespace backend.Contracts.v1.DTO;

public struct ClimbingHoldDto
{
    public string Type {get; set;} = "";
    public Point[] Points {get; set;}
    public float Cx {get; set; } = 0;
    public float Cy {get; set;} = 0;
    public float Rx {get; set;} = 0;
    public float Ry {get; set;} = 0;
    public int   Rotation {get; set;} = 0;
    public ClimbingHoldDto(Point[] points, string type, float cx, float cy, float rx, float ry, int rotation)
    {
        Points = points;
        Type = type;
        Cx = cx;
        Cy = cy;
        Rx = rx;
        Ry = ry;
        Rotation = rotation;
    }
}