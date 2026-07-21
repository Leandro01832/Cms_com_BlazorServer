using System;

namespace business.business.sistema
{
    public class UserFollow
    {
        // O usuário que toma a ação de seguir alguém (ex: Você)
        public string ObserverId { get; set; } = null!;
        public virtual UserModel Observer { get; set; } = null!;

        // O usuário que está recebendo o "follow" (ex: o perfil visitado)
        public string TargetId { get; set; } = null!;
        public virtual UserModel Target { get; set; } = null!;

        // Opcional: data em que começaram a se seguir
        public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
    }
}