namespace Ease.CodeChallenge.Application.Exceptions
{
    public class InvalidGuidException : ApplicationException
    {
        private const string message = "Guid does not exist";
        public InvalidGuidException() : base(message) { }
    }
}
