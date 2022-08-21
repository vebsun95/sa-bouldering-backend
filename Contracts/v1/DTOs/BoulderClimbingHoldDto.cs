namespace backend.Contracts.v1.DTO;

public struct BoulderClimbingHoldDto
{
    public int HoldIndex {get; set;}
    public int HoldType {get; set;}

    public BoulderClimbingHoldDto(int holdIndex, int holdType) : this()
    {
        HoldType = holdType;
        HoldIndex = holdIndex;
    }
}
    