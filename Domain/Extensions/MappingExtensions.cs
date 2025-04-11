using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    public static TDest MapTo<TDest>(this object source)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        TDest dest = Activator.CreateInstance<TDest>()!;

        var sourceProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destProps = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var dp in destProps)
        {
            var sp = sourceProps.FirstOrDefault(p => p.Name == dp.Name && p.PropertyType == dp.PropertyType);

            if (sp != null)
            {
                var value = sp.GetValue(source);
                dp.SetValue(dest, value);
            }
        }
        return dest;
    }
}
