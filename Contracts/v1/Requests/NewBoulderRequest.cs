using backend.Contracts.v1.DTO;
using backend.SharedKernel;

namespace backend.Contracts.v1.Requests;

public class NewBoulderRequest {
    public string wall {get; set;}
    public string name {get; set;}
    public int grade {get; set;}
    public HoldIndexDto[] holds {get; set;}
}

public class BoulderRequestWallValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if (string.IsNullOrWhiteSpace(item.wall)) return (false, $"{nameof(item.wall)} cannot be be empty");
        return (true, "");
    }
}

public class BoulderRequestNameValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if (string.IsNullOrWhiteSpace(item.name) || item.name.Length > 32) return (false, $"{nameof(item.name)} cannot be be empty or larger than 32 chars");
        return (true, "");
    }
}

public class BoulderRequestGradeValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.grade < 0 || item.grade > 10) return (false, $"{nameof(item.grade)} is invalid");
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
        if(item.holds.Select(h => h.Type == 0).Count() > 1) return (false, "A boulder needs a start hold");
        return (true, "");
    }
}

public class BoulderRequestFinishHoldsValidator : IValidator<NewBoulderRequest>
{
    public (bool IsValid, string Error) isValid(NewBoulderRequest item)
    {
        if(item.holds.Select(h => h.Type == 3).Count() > 1) return (false, "A boulder needs a finish hold");
        return (true, "");
    }
}