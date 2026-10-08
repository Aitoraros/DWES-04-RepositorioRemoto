namespace RepositorioRemoto.Services.Background;

public interface IBackgroundService {
    Task StartAsync(CancellationToken cancellationToken = default);
}