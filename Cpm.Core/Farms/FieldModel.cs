using System.Collections.Generic;

namespace Cpm.Core.Farms
{
    public class FieldModel : ModelBase
    {
        public string Name { get; set; }
        public string Variety { get; set; }
        public decimal AreaInHectares { get; set; }

        public override ICollection<string> GetErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(Name))
            {
                errors.Add("The field name is empty.");
            }
            if (AreaInHectares <= 0)
            {
                errors.Add("The area should be > 0.");
            }
            return errors;
        }
    }
}