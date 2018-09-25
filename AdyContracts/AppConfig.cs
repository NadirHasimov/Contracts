using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AdyContracts
{
    public static class AppConfig
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Local_ADY_ContractsDB"].ConnectionString;
            }
        }

        public static string Path
        {
            get
            {
                return ConfigurationManager.AppSettings["path"].ToString();
            }
        }
    }
}