using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cpm.Core.Farms
{
    public class FarmModel : ModelBase
    {
        public string Name { get; set; }

        public SiteModel[] Sites { get; set; }

        public string FirstDayOfYear { get; set; }

        public static IModelResult Parse(string text)
        {
            var modelResult = new ModelResult();
            FarmModel model;
            try
            {
                model = JsonConvert.DeserializeObject<FarmModel>(text);
            }
            catch (JsonException ex)
            {
                modelResult.Errors.Add("Invalid JSON file: " + ex.Message);
                return modelResult;
            }

            if (model == null)
            {
                modelResult.Errors.Add("No farm defined.");
            }

            if (!modelResult.Success)
            {
                return modelResult;
            }

            modelResult.Errors = model.GetErrors();

            if (modelResult.Success)
            {
                modelResult.Model = model;
            }

            return modelResult;
        }

        public override ICollection<string> GetErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrEmpty(Name))
            {
                errors.Add("The farm name is empty.");
            }
            CheckCollection(errors, Sites, "Site", "There are no sites.");
            return errors;
        }
    }
}