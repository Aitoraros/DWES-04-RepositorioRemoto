using System.Reactive.Linq;
using System.Reactive.Subjects;
using RepositorioRemoto.Models;
using RepositorioRemoto.Services.Notificactions;

namespace RepositorioRemoto.Services.Notifications;

public class ConsoleNotificationService : INotificationService, IDisposable {
    
    private readonly Subject<Notification> _subject = new();

    /// <inheritdoc />
    public IObservable<Notification> Observable => _subject.AsObservable();

    /// <inheritdoc />
    public void Notificar(Notification notificacion) {
        _subject.OnNext(notificacion);
    }

    public void Dispose() {
        _subject.OnCompleted();
        _subject.Dispose();
    }
}