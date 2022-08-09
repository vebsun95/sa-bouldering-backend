namespace backend.Models;
public class Wall {
    public int Id {get; set;}
    public string URI {get; set;}
    public string Name {get; set;}
    public int NrOfHolds {get; set;}
    public DateTime Uploaded {get; set;}
    private List<EllipseHold> _ellipseHolds = new();
    public IEnumerable<EllipseHold> EllipseHolds => _ellipseHolds.AsReadOnly();
    private List<PolygonHold> _polygonHolds = new();
    public IEnumerable<PolygonHold> PolygonHolds => _polygonHolds.AsReadOnly();
    private List<Neighbour> _neighbours = new();
    public IEnumerable<Neighbour> Neighbours => _neighbours.AsReadOnly();
    public Wall(string uri, string name) {
        this.URI = uri;
        this.Name = name;
        this.Uploaded = DateTime.Now;
        this.NrOfHolds = 0;
    }
    private Wall(){}
    public void AddPolygonHold(PolygonHold hold) {
        _polygonHolds.Add(hold);
        NrOfHolds++;
    }
    public void AddEllipseHold(EllipseHold hold) {
        _ellipseHolds.Add(hold);
        NrOfHolds++;
    }
    public void AddNeighbour(Neighbour neighbour) {
        _neighbours.Add(neighbour);
    }
    public void AddNeighbour(int index, int neighbourIndex) {
        _neighbours.Add(new Neighbour() {Index=index, NeighbourIndex=neighbourIndex});
    }
}

public record Neighbour {
    public int Id {get; set;}
    public Wall Wall {get; set;}

    public int  Index {get; set;}
    public int NeighbourIndex {get; set;}
    public string Direction {get; set;}
}