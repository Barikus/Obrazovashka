namespace Obrazovashka.Results
{
    public abstract class BaseResult
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
    }
}
