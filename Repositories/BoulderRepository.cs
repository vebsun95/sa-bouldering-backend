using backend.Contracts.v1.Requests;
using backend.Data;
using backend.Models;

namespace backend.Repositories;

public interface IBoulderRepository {
    Task<backend.Models.Boulder> AddRoute(User user, NewBoulderRequest request, Wall wall);

}

public class BoulderRepository : IBoulderRepository {
    private readonly ILogger<BoulderRepository> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private List<backend.Models.Boulder>? _routes;
    public BoulderRepository(ILogger<BoulderRepository> logger, IServiceScopeFactory scopeFactory) {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }
    public async Task<backend.Models.Boulder> AddRoute(User user, NewBoulderRequest request, Wall wall) {
        using(var scope = _scopeFactory.CreateScope()) {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return new Boulder(1, "qwe", user, request.Grade);
        }
    }
}
