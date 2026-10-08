namespace RepositorioRemoto.Models;

/// <summary>Evento emitido por el servicio de notificaciones cuando se crea, actualiza o elimina un usuario.</summary>
public record Notification(Notification.NotificationType Type, string Message, DateTime Timestamp) {
    /// <summary>Tipo de operación que ha generado una notificación.</summary>
    public enum NotificationType { Create, Update, Delete }
}