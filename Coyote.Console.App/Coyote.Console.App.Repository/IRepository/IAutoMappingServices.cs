namespace Coyote.Console.App.Repository
{
    public interface IAutoMappingServices
    {
        public TDest Mapping<TSource, TDest>(TSource Mapper);
    }
}
