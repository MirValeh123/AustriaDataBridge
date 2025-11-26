using AutoMapper;
using System.Reflection;

namespace Shared.MappingProfile
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public MappingProfile(List<Assembly> assemblies) : this()
        {
            foreach (Assembly assembly in assemblies)
            {
                ApplyMappingsFromAssembly(assembly);
            }
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            var types = GetMappedTypes(assembly);

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod("Mapping") ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { this });
            }
        }

        private static List<Type> GetMappedTypes(Assembly assembly)
        {
            return assembly.GetExportedTypes()
                  .Where(t => t.GetInterfaces().Any(i =>
                      i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                  .ToList();

        }
    }

}
