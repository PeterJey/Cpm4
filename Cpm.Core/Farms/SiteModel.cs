using System.Collections.Generic;

namespace Cpm.Core.Farms
{
    public class SiteModel : ModelBase
    {
        public string Name { get; set; }

        public string Postcode { get; set; }

        public FieldModel[] Fields { get; set; }

        public override ICollection<string> GetErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(Name))
            {
                errors.Add("The site name is empty.");
            }
            CheckCollection(errors, Fields, "Field", "There are no fields.");
            return errors;
        }
    }
}