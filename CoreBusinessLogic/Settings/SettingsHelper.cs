using System;
using System.Configuration;
using System.Web.Mvc;

namespace SCMS.CoreBusinessLogic.Settings
{   
    public class SettingsHelper<T> where T : ISystemSettings, new()
    {
        private static SettingConfigurationProvider<T> s_TConfigurationProvider;

        public static T Settings
        {
            get
            {
                if(s_TConfigurationProvider == null)
                {
                    s_TConfigurationProvider = DependencyResolver.Current.Resolve<SettingConfigurationProvider<T>>();
                }

                return s_TConfigurationProvider.GetFromCache();
            }
        }
    }
}
