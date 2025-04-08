using System.Reflection;

namespace Domain.Extensions;

public static class MappingExtensions
{
    public static TDest MapTo<TDest>(this object source)
    {
        // Check if null
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        //Create instance, suppress null
        TDest dest = Activator.CreateInstance<TDest>()!;

        // Create lists of properties
        var sourceProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destProps = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var dp in destProps)
        {
            // Return matching property if any
            var sp = sourceProps.FirstOrDefault(p => p.Name == dp.Name && p.PropertyType == dp.PropertyType);

            //Copy value from source to dest if match found
            if (sp != null)
            {
                var value = sp.GetValue(source);
                dp.SetValue(dest, value);
            }
        }
        return dest;
    }
}
