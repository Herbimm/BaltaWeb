namespace BaltaWeb.Interfaces
{
    public interface ITesteService
    {
        Task<T> GetAsync<T>(string address);
    }
}
