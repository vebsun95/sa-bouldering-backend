using backend.Contracts.v1.DTO;

namespace backend.Contracts.v1.Responses;

public class LatestWallResponse {
    public string URI {get; set;}
    public ClimbingHoldDto [] Holds {get; set;}
    public LatestWallResponse(string uri, ClimbingHoldDto[] holds)
    {
        URI = uri;
        Holds = holds;
    }
}


