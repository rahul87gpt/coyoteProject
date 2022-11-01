using AutoMapper;
using System.Collections.Generic;

namespace Coyote.Console.Common.Services
{
    public class AutoMappingServices : IAutoMappingServices
    {
        IMapper iMapper;
        public T_Dest Mapping<T_Source, T_Dest>(T_Source Mapper)
        {

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T_Source, T_Dest>();
            });

            iMapper = config.CreateMapper();
            var AfterMap = iMapper.Map<T_Source, T_Dest>(Mapper);
            return AfterMap;
        }

        public List<TDestination> MapList<TSource, TDestination>(List<TSource> source)
        {
            return iMapper.Map<List<TDestination>>(source);
        }
    }
}
