using System.Configuration;

namespace CIM.Common
{
    public static class ConfigHelper
    {
        public static string GetKey(string key)
        {
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }
}