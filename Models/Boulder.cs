namespace backend.Models;

public class Boulder {
    public int Id {get; set;}
    public int Index {get; set;}
    public string Name {get; set;}
    public User FA {get; set;}
    public int Grade {get; set;}
    public List<ClimbingHoldBoulder> Holds {get; set;} = new();
    public List<Ascent> Ascents {get; set;} = new();
    public Boulder(int index,string name, User fa, int grade) {
        Index = index;
        Name = name;
        FA = fa;
        Grade = grade;
    }
    private Boulder(){}
}

public class ClimbingHoldBoulder{
    public int BoulderId { get; set; }
    public int ClimbingHoldId { get; set; }
    public virtual ClimbingHold ClimbingHold { get; set; }
    public TYPES Type {get; set;}
    public ClimbingHoldBoulder(){}

    public enum TYPES
    {
        START,
        HAND,
        FOOT,
        FINISH,
    }
}