namespace ProductKeyManager.Configuration
{
    public sealed class SecuritySettings
    {
        public bool IsEnabled { get; set; }
        
        public string SharedSecretKey { get; set; }
    }
}
