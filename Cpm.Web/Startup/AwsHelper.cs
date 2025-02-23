using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace Cpm.Web.Startup
{
    public static class AwsHelper
    {
        public static AWSCredentials CreateAwsCredentialsFromConfig(IConfiguration configuration)
        {
            return new BasicAWSCredentials(
                configuration["AWS:Credentials:AccessKey"],
                configuration["AWS:Credentials:SecretKey"]);
        }
    }
}