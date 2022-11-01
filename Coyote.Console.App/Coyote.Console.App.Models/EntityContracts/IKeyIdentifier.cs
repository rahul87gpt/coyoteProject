using System.ComponentModel.DataAnnotations;

namespace Coyote.Console.App.Models.EntityContracts
{
    interface IKeyIdentifier<T>
    {
        [Key]
        public T Id { get; set; }
    }
    interface IKeyIdentifierField<T>
    {
        [Key]
        public T ID { get; set; }
    }
}
