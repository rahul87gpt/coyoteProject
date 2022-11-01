using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.Services.IServices
{
    public interface IAutoMappingServices
    {
        public TDest Mapping<TSource, TDest>(TSource Mapper);
    }
}
