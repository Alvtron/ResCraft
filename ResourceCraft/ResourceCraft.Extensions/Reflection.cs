using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace ResourceCraft.Extensions
{
    public static class Reflection
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(this T source)
        {
            if (source == null)
            {
                return default(T);
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The destination.</param>
        public static void CopyProperties(this object source, object target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("Source object is null");
            }

            if (target == null)
            {
                throw new ArgumentNullException("Target Object is null");
            }

            var typeTarget = target.GetType();
            var typeSource = source.GetType();
            
            var sourceProperties = typeSource.GetProperties();
            foreach (var sourceProperty in sourceProperties)
            {
                if (!sourceProperty.CanRead)
                {
                    continue;
                }

                var targetProperty = typeTarget.GetProperty(sourceProperty.Name);

                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(sourceProperty.PropertyType))
                {
                    continue;
                }
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
        }
    }
}
