using AutoMapper;
using System.Collections.Generic;

namespace Coyote.Console.App.WebAPI.Helper
{
    /// <summary>
    /// AutoMapperExtensions
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// MapList
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> MapList<TSource, TDestination>(this IMapper mapper, List<TSource> source)
        {
            return mapper?.Map<List<TDestination>>(source);
        }
    }
}
