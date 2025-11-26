using Newtonsoft.Json;
using Shared.Constants;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using Shared.Helpers;
using Shared.Models;

namespace Shared.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly IConfiguration _config;

        public AppSettingsService(IConfiguration config)
        {
            _config = config;

            var json = TextFileReader.ReadAsStringAsync(AppSettingsValues.AppsettingsFileDirectory).Result;
            AppSettingsInstance = JsonConvert.DeserializeObject<AppSettings>(json);
        }

        public AppSettings? AppSettingsInstance { get; set; }

        public T BindSection<T>(string Key) where T : class, new()
        {
            var json = TextFileReader.ReadAsStringAsync(AppSettingsValues.AppsettingsFileDirectory).Result;

            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(json);
            JObject wantedObject = jsonObject[Key] as JObject;

            if (wantedObject != null)
            {
                T instance = wantedObject.ToObject<T>();
                return instance;
            }

            return new T();
        }

        public T Setting<T>(string name)
        {
            string value = _config.GetSection(name.Split(".")[0]).GetSection(name.Split(".")[1]).Value;

            if (value == null)
            {
                throw new KeyNotFoundException(string.Format("Could not find setting '{0}',", name));
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
        }
    }
}
