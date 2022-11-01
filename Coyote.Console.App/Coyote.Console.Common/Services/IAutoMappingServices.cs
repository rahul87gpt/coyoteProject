
namespace Coyote.Console.Common.Services
{
    public interface IAutoMappingServices
    {
        public TDest Mapping<TSource, TDest>(TSource Mapper);
    }
}
