using System;
using Coyote.Console.Common;

namespace Coyote.Console.App.Models.EntityContracts
{
    public interface IAuditable<T>
    {
        public T CreatedById { get; set; }
        public T UpdatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface IAuditableField<T>
    {
        public T CreatedBy { get; set; }
        public T ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public Status IsActive { get; set; }
    }
}
