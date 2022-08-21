namespace backend.Contracts.v1.DTO;

public record BoulderDto {
    public string Name {get; set;}
    public string FA {get; set;}
    public int Grade {get; set;}
    public int Ascents {get; set;}
    public bool Checked {get; set;}
    public BoulderClimbingHoldDto[] Holds {get; set;}

    public BoulderDto(string name, string fA, int grade, int ascents, bool @checked, BoulderClimbingHoldDto[] holds)
    {
        Name = name;
        FA = fA;
        Grade = grade;
        Ascents = ascents;
        Checked = @checked;
        Holds = holds;
    }
}

public record HoldIndexDto {
    public int Index {get; set;}
    public int Type  {get; set;}
    public HoldIndexDto(int index, int type)
    {
        Index = index;
        Type = type;
    }
}