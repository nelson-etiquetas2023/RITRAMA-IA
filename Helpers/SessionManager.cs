namespace Ritrama2025.Helpers
{
    public class SessionManager
    {
        private DateTime _lastActivity;
        private readonly TimeSpan _timeout = TimeSpan.FromMinutes(15);

        public event Action? SessionExpired;
        public bool IsExpired => (DateTime.Now - _lastActivity) > _timeout;

        public void ResetActivity() => _lastActivity = DateTime.Now;
        public void Start() => _lastActivity = DateTime.Now;

        public bool CheckExpiration()
        {
            if (IsExpired)
            {
                SessionExpired?.Invoke();
                return true;
            }
            return false;
        }
    }
}
