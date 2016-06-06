using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SCMS.CoreBusinessLogic.Caching;

namespace SCMS.CoreBusinessLogic.Settings
{
    public class SettingConfigurationProvider<TSettings> where TSettings: ISystemSettings
    {
        readonly ISettingService m_SettingService;
        private readonly ICacheService m_CacheService;
        private readonly string m_SettingsCacheKey;

        public SettingConfigurationProvider(ISettingService settingService, ICacheService cacheService) 
        {
            m_SettingService = settingService;
            m_CacheService = cacheService;
            m_SettingsCacheKey = typeof (TSettings).AssemblyQualifiedName;

            BuildConfiguration();
        }

        public TSettings GetFromCache()
        {
            var settings = m_CacheService.Get<TSettings>(m_SettingsCacheKey);
            if (settings == null)
            {
                BuildConfiguration();
                settings = Settings;
            }
            return settings;
        }

        public TSettings Settings { get; protected set; }

        public void BuildConfiguration()
        {
            Settings = Activator.CreateInstance<TSettings>();

            // get properties we can write to
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             let setting = m_SettingService.GetSettingByKey<string>(typeof(TSettings).Name + "." + prop.Name)
                             where setting != null
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             let value = prop.PropertyType.GetCustomTypeConverter().ConvertFromInvariantString(setting)
                             select new { prop, value };

            // assign properties
            properties.ToList().ForEach(p => p.prop.SetValue(Settings, p.value, null));
            m_CacheService.Put(m_SettingsCacheKey, CacheTimeSpan.Infinite, Settings);
        }

        public void SaveSettings(TSettings settings)
        {
            var properties = from prop in typeof(TSettings).GetProperties()
                             where prop.CanWrite && prop.CanRead
                             where prop.PropertyType.GetCustomTypeConverter().CanConvertFrom(typeof(string))
                             select prop;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            foreach (var prop in properties)
            {
                string key = typeof(TSettings).Name + "." + prop.Name;
                //Duck typing is not supported in C#. That's why we're using dynamic type
                dynamic value = prop.GetValue(settings, null);
                if (value != null)
                    m_SettingService.SetSetting(key, value, false);
                else
                    m_SettingService.SetSetting(key, "", false);
            }

            //and now clear cache
            m_SettingService.ClearCache();

            Settings = settings;
            m_CacheService.Put(m_SettingsCacheKey, CacheTimeSpan.Infinite, Settings);
        }
    }
}
