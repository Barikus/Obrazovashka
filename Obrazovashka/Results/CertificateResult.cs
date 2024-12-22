namespace Obrazovashka.Results
{
    public class CertificateResult
    {
        public bool Success { get; set; }
        public string CertificateUrl { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
