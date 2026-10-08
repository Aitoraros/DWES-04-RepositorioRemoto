using FluentAssertions;
using RepositorioRemoto.Models;
using RepositorioRemoto.Services.Notifications;

namespace RepositorioRemoto.Tests.Services;

[TestFixture]
public class NotificationServiceTest
{
    [TestFixture]
    public class CasosValidos
    {
        private NotificationService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _service = new NotificationService();
        }

        [TearDown]
        public void TearDown()
        {
            _service.Dispose();
        }

        [Test]
        public void Notificar_ConSuscriptorActivo_LeLlegaLaNotificacion()
        {
            // Arrange
            Notification? recibida = null;
            _service.Observable.Subscribe(n => recibida = n);

            var notificacion = new Notification(Notification.NotificationType.Create, "Usuario creado", DateTime.UtcNow);

            // Act
            _service.Notificar(notificacion);

            // Assert
            recibida.Should().Be(notificacion);
        }

        [Test]
        public void Notificar_ConVariosSuscriptores_LesLlegaATodos()
        {
            // Arrange
            var recibidas1 = new List<Notification>();
            var recibidas2 = new List<Notification>();
            _service.Observable.Subscribe(recibidas1.Add);
            _service.Observable.Subscribe(recibidas2.Add);

            var notificacion = new Notification(Notification.NotificationType.Update, "Usuario actualizado", DateTime.UtcNow);

            // Act
            _service.Notificar(notificacion);

            // Assert
            recibidas1.Should().ContainSingle().Which.Should().Be(notificacion);
            recibidas2.Should().ContainSingle().Which.Should().Be(notificacion);
        }

        [Test]
        public void Notificar_SinSuscriptores_NoLanzaExcepcion()
        {
            // Arrange
            var notificacion = new Notification(Notification.NotificationType.Delete, "Usuario eliminado", DateTime.UtcNow);

            // Act
            var act = () => _service.Notificar(notificacion);

            // Assert
            act.Should().NotThrow();
        }

        [Test]
        public void Observable_SuscriptorTardio_NoRecibeNotificacionesAnteriores() {
            // Arrange: es un flujo CALIENTE (Subject): quien se suscribe tarde no ve lo que ya pasó
            _service.Notificar(new Notification(Notification.NotificationType.Create, "Antes de suscribirse",
                DateTime.UtcNow));

            var recibidas = new List<Notification>();
            _service.Observable.Subscribe(recibidas.Add);

            // Act
            _service.Notificar(new Notification(Notification.NotificationType.Update, "Después de suscribirse",
                DateTime.UtcNow));

            // Assert
            recibidas.Should().ContainSingle().Which.Message.Should().Be("Después de suscribirse");
        }
    }
}