namespace backend.Models;
public class Wall {
    public int Id {get; set;}
    public string URI {get; set;}
    public string Name {get; set;}
    public DateTime Uploaded {get; set;}
    public User UploadedBy { get; set; }
    public bool IsOutDated { get; set; } = false;
    public List<ClimbingHold> ClimbingHolds {get; set;} = new();
    public List<Boulder> Boulders {get; set;} = new();
    public Wall(string uri, string name, User uploadedBy)
    {
        URI = uri;
        Name = name;
        Uploaded = DateTime.Now;
        UploadedBy = uploadedBy;
    }
    private Wall(){}
}

public record Neighbour {
    public int Id {get; set;}
    public Wall Wall {get; set;}

    public int  Index {get; set;}
    public int NeighbourIndex {get; set;}
    public DIRECTION Direction {get; set;}
}

public enum DIRECTION
{
    NORTH,
    SOUTH,
    WEST,
    EAST,
}