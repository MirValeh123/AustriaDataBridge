using Shared.Models;

namespace Shared.Services
{
    public interface IAppSettingsService
    {
        public AppSettings? AppSettingsInstance { get; set; }

        T BindSection<T>(string Key) where T : class, new();
        T Setting<T>(string name);
    }
}
