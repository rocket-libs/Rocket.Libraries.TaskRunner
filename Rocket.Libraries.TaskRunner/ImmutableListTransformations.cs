namespace Rocket.Libraries.TaskRunner
{
    using Newtonsoft.Json;
    using System.Collections.Immutable;
    using System.Linq;

    public static class ImmutableListTransformations
    {
        public static ImmutableList<TDestination> Transform<TSource, TDestination>(this ImmutableList<TSource> sourceList)
            where TSource : TDestination
        {
            return sourceList.Select(a => (TDestination)a).ToImmutableList();
        }

        public static ImmutableList<TDestination> SerializeTransform<TSource, TDestination>(this ImmutableList<TSource> sourceList)
        {
            return sourceList.Select(a => JsonConvert.DeserializeObject<TDestination>(JsonConvert.SerializeObject(a)))
                    .ToImmutableList();
        }
    }
}