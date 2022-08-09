namespace backend.Models;

public interface IHold
{
    int Id {get; set;}
    Wall Wall {get; set;}
    int Index { get; set; }
}


public record Point
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
        this._points.Add(new Point() { X = x, Y = y });
    }
}
