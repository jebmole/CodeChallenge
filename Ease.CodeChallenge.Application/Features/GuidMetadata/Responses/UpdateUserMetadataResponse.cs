namespace Ease.CodeChallenge.Application.Features.GuidMetadata.Responses
{
    public class UpdateUserMetadataResponse
    {
        public string Guid { get; set; }
        public DateTime Expires { get; set; }
        public string User { get; set; }
    }
}
