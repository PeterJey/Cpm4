namespace Cpm.Web.Security
{
    public class ClaimTypes
    {
        public const string AdministeringApplication = ClaimIssuer.Issuer + "app";


        public const string AdministeringUsers = ClaimIssuer.Issuer + "u";


        public const string Viewing = ClaimIssuer.Issuer + "v";


        public const string ForecastingSite = ClaimIssuer.Issuer + "fcs";
        public const string ForecastingField = ClaimIssuer.Issuer + "fcf";


        public const string AllocatingForSite = ClaimIssuer.Issuer + "als";
        public const string AllocatingForField = ClaimIssuer.Issuer + "alf";


        public const string UpdatingTheFieldDiary = ClaimIssuer.Issuer + "ufd";
        public const string UpdatingTheSiteDiary = ClaimIssuer.Issuer + "usd";


        public const string ActualSiteDataLogging = ClaimIssuer.Issuer + "lds";
        public const string ActualFieldDataLogging = ClaimIssuer.Issuer + "ldf";


        public const string DailySitePlanning = ClaimIssuer.Issuer + "dsp";
        public const string DailyFieldPlanning = ClaimIssuer.Issuer + "dfp";


        public const string ShortName = ClaimIssuer.Issuer + "sn";
        public const string LongName = ClaimIssuer.Issuer + "ln";
    }
}