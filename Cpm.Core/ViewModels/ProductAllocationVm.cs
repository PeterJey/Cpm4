using System.Collections.Generic;
using System.Linq;
using Cpm.Core.Extensions;
using Cpm.Core.Services.Allocations;
using Optional;

namespace Cpm.Core.ViewModels
{
    public class ProductAllocationVm
    {
        public Product Product { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Punnets { get; set; }
        public decimal? Trays { get; set; }

        public static ProductAllocationVm Create(Product product, decimal? weight)
        {
            var punnets = weight
                .ToOption()
                .Map(v => v / product.PerPunnet * 1000)
                .ToNullable();
            var trays = punnets
                    .ToOption()
                    .Map(v => v / product.PerTray)
                    .ToNullable();
            return new ProductAllocationVm
            {
                Product = product,
                Weight = weight,
                Punnets = punnets,
                Trays = trays
            };
        }

        public static ProductAllocationVm Create(Product product, ICollection<FieldAllocationVm> fields)
            =>  Create(
                product, 
                fields
                    .Select(x => x.Products
                        .SingleOrDefault(p => p?.Product == product)
                        ?.Weight
                    )
                    .NullableSum()
                );
    }
}