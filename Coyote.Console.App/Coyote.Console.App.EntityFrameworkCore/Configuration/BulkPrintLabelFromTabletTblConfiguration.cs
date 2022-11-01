using Coyote.Console.App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coyote.Console.App.EntityFrameworkCore.Configuration
{
    public class BulkPrintLabelFromTabletTblConfiguration : IEntityTypeConfiguration<BulkPrintLabelFromTabletTbl>
    {
        public void Configure(EntityTypeBuilder<BulkPrintLabelFromTabletTbl> builder)
        {
            builder?.HasNoKey();

        }
    }
}
