using RepositorioRemoto.Models;

namespace RepositorioRemoto.Services.Notificactions;

/// <summary>Expone un flujo observable de eventos de usuario (crear/actualizar/eliminar).</summary>
public interface INotificationService {
    
    /// <summary>Solo se reciben eventos desde el momento de la suscripción (flujo caliente)</summary>
    IObservable<Notification> Observable { get; }

    /// <summary>Emite una notificación a todos los suscriptores activos en este momento.</summary>
    void Notificar(Notification notificacion);
}