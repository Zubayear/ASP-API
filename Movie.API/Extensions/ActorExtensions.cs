using System.Dynamic;
using System.Reflection;

namespace Movie.API.Extensions;

public static class ActorExtensions
{
    public static ExpandoObject ShapeDataForActor<TSource>(this TSource source, string fields)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        var response = new ExpandoObject();
        if (string.IsNullOrWhiteSpace(fields))
        {
            var propertyInfos = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(source);
                ((IDictionary<string, object>)response).Add(propertyInfo.Name, value);
            }
            return response;
        }

        var fieldsAfterSplit = fields.Split(",");
        foreach (var field in fieldsAfterSplit)
        {
            var propertyName = field.Trim();
            // Get the propertyInfo with id, title
            var propertyInfo = typeof(TSource).GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            var value = propertyInfo?.GetValue(source);
            ((IDictionary<string, object>)response).Add(propertyInfo.Name, value);
        }

        return response;
    }
}