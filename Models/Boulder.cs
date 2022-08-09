namespace backend.Models;

public class Boulder {
    public int Id {get; set;}
    public int Index {get; set;}
    public Wall Wall {get; set;}
    public string Name {get; set;}
    public ApplicationUser FA {get; set;}
    public int Grade {get; set;}
    public List<HoldIndex> Holds {get; set;} = new();
    public Boulder(int index, Wall wall,string name, ApplicationUser fa, int grade) {
        Index = index;
        Name = name;
        Wall = wall;
        FA = fa;
        Grade = grade;
    }
    private Boulder(){}
}

public class HoldIndex{
    public int Id { get; set; }
    public int Index {get; set;}
    public int Type {get; set;}
    
    public HoldIndex(int index, int type) {
        Index = index;
        Type  = type;
    }
    
    private HoldIndex(){}
}