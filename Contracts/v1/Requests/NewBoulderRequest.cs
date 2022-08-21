using backend.Contracts.v1.DTO;
using backend.SharedKernel;

namespace backend.Contracts.v1.Requests;

public struct NewBoulderRequest {
    public string WallUri {get; set;}
    public string Name {get; set;}
    public int Grade {get; set;}
    public BoulderClimbingHoldDto[] holds {get; set;}
}

public class BoulderRequestWallValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if (string.IsNullOrWhiteSpace(item.WallUri)) return (false, $"{nameof(item.WallUri)} cannot be be empty");
        return (true, "");
    }
}

public class BoulderRequestNameValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if (string.IsNullOrWhiteSpace(item.Name) || item.Name.Length > 32) return (false, $"{nameof(item.Name)} cannot be be empty or larger than 32 chars");
        return (true, "");
    }
}

public class BoulderRequestGradeValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.Grade < 0 || item.Grade > 10) return (false, $"{nameof(item.Grade)} is invalid");
        return (true, "");
    }
}

public class BoulderRequestHoldsValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.holds.Length < 3) return (false, "A boulder needs atleast 3 holds");
        return (true, "");
    }
}

public class BoulderRequestStartHoldsValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.holds.Select(h => h.HoldType== 0).Count() > 1) return (false, "A boulder needs a start hold");
        return (true, "");
    }
}

public class BoulderRequestFinishHoldsValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.holds.Select(h => h.HoldType == 3).Count() > 1) return (false, "A boulder needs a finish hold");
        return (true, "");
    }
}