namespace BaltaWeb.Interfaces
{
    public interface ISeniorAuthenticationService
    {
        Task DetectGoogleTranslatorAsync();

        Task PostUserAsync();
    }
}
