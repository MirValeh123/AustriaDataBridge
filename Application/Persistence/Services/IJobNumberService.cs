namespace Application.Persistence.Services
{
    public interface IJobNumberService
    {
        Task<string> GetNextJobNumberAsync();
    }
}
