namespace backend.Contracts.v1.DTO;

public class PolygonHoldDto{
    public int Index {get; set;}
    public PointDto[] Points {get; set;}
}

public class PointDto {
    public float X {get; set;}
    public float Y {get; set;}
}