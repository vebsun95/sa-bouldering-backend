using backend.Contracts.v1.DTO;

namespace backend.Models;

public interface IHold
{
    int Id { get; set; }
    Wall Wall {get; set;}
    int Index { get; set; }
}


public class Point
{
    public int Id {get; set;}
    public float X {get; set;}
    public float Y {get; set;}
}

public class EllipseHold : IHold
{
    public int Id {get; set;}
    public Wall Wall {get; set;}
    public int Index { get; set; }
    public float Cx {get; set; }
    public float Cy {get; set;}
    public float Rx {get; set;}
    public float Ry {get; set;}
    public int   Rotation {get; set;}

    public EllipseHold(Wall wall, int index, float cx, float cy, float rx, float ry, int rotation)
    {
        Wall = wall;
        Index = index;
        Cx = cx;
        Cy = cy;
        Rx = rx;
        Ry = ry;
        Rotation = rotation;
    }
    private EllipseHold() { }
}

public class PolygonHold : IHold
{
    public int Id {get; set;}
    public Wall Wall {get; set;}
    public int Index { get; set; }
    private List<Point> _points = new();
    public IEnumerable<Point> Points => _points.AsReadOnly();
    public PolygonHold(Wall wall, int index)
    {
        Wall = wall;
        Index = index;
    }
    private PolygonHold() { }
    public void AddPoint(Point point)
    {
        this._points.Add(point);
    }
    public void AddPoint(float x, float y)
    {
        //this._points.Add(new Point(x, y));
    }
}

public class ClimbingHold
{
    public int Id {get; set;}
    public int Index { get; set; }
    public TYPE Type {get; set;}
    public int NeighbourNorth {get; set;} = -1;
    public int NeighbourSouth{get; set;} = -1;
    public int NeighbourWest {get; set;} = -1;
    public int NeighbourEast {get; set;} = -1;
    public List<Point> Points {get; set;} = new();
    public float Cx {get; set; } = 0;
    public float Cy {get; set;} = 0;
    public float Rx {get; set;} = 0;
    public float Ry {get; set;} = 0;
    public int   Rotation {get; set;} = 0;

    public static ClimbingHold GenerateEllipseHold(int index, float cx, float cy, float rx, float ry, int rotation)
    {
        return new ClimbingHold()
        {
            Index = index,
            Type  = TYPE.ELLIPSE,
            Cx    = cx,
            Cy    = cy,
            Rx    = rx,
            Ry    = ry,
            Rotation = rotation,
        };
    }
    public static ClimbingHold GeneratePolygonHold(int index, Point[] points)
    {
        var ch = new ClimbingHold()
        {
            Index = index,
            Type  = TYPE.POLYGON
        };
        ch.Points.AddRange(points);
        return ch;
    }
    public static ClimbingHold Generate(int index, ClimbingHoldDto climbingHoldDto)
    {
        if(climbingHoldDto.Type == TYPE.ELLIPSE.ToString())
            return GenerateEllipseHold(index: index, cx: climbingHoldDto.Cx, cy: climbingHoldDto.Cy, rx: climbingHoldDto.Rx, ry: climbingHoldDto.Ry, rotation: climbingHoldDto.Rotation);
        return GeneratePolygonHold(index: index, climbingHoldDto.Points);
    }
    public ClimbingHoldDto TransfromToDTO()
    {
        return new ClimbingHoldDto
            (
                points: Points.ToArray(),
                type: Type.ToString(),
                cx: Cx,
                cy: Cy,
                rx: Rx,
                ry: Ry,
                rotation: Rotation
            );
    }
    private ClimbingHold() { }
    
    public enum TYPE 
    {
        ELLIPSE,
        POLYGON,
    }
}
