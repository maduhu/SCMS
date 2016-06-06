using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SCMS.Model;
using SCMS.CoreBusinessLogic.Caching;
using System.Linq;
using System.Data.Entity.Infrastructure;

namespace SCMS.CoreBusinessLogic.Settings
{
    /// <summary>
    /// Setting manager
    /// </summary>
    public class SettingsService : ISettingService
    {
        #region Constants
        private const string SettingsAllKey = "SCMS.Setting.All";
        #endregion

        #region Fields

        private readonly ICacheService m_CacheService;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheService">Cache service</param>>
        public SettingsService(ICacheService cacheService)
        {
            m_CacheService = cacheService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Adds a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void InsertSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            using(var context = SCMSEntities.Define())
            {
                context.Settings.Add(setting);
                context.SaveChanges();
            }

            //cache
            if (clearCache)
                m_CacheService.RemoveByPattern(SettingsAllKey);
        }

        /// <summary>
        /// Updates a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void UpdateSetting(Setting setting, bool clearCache = true)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            using(var context = SCMSEntities.Define())
            {
                context.Settings.Attach(setting);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(setting, System.Data.EntityState.Modified);
                context.SaveChanges();
            }

            //cache
            if (clearCache)
                m_CacheService.RemoveByPattern(SettingsAllKey);
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Gets a setting by identifier
        /// </summary>
        /// <param name="settingId">Setting identifer</param>
        /// <returns>Setting</returns>
        public virtual Setting GetSettingById(Guid settingId)
        {
            if(settingId == Guid.Empty) return null;

            using(var context = SCMSEntities.Define())
            {
                var setting = context.Settings.Where(p => p.Id == settingId).FirstOrDefault();
                return setting;
            }
        }

        /// <summary>
        /// Get setting value by key
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="defaultValue">Default value</param>
        /// <returns>Setting value</returns>
        public virtual T GetSettingByKey<T>(string key, T defaultValue = default(T))
        {
            if (String.IsNullOrEmpty(key))
                return defaultValue;

            key = key.Trim().ToLowerInvariant();

            var settings = GetAllSettings();
            if (settings.ContainsKey(key)) {
                var setting = settings[key];
                return setting.Value.To<T>();
            }
            return defaultValue;
        }

        /// <summary>
        /// Set setting value
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="clearCache">A value indicating whether to clear cache after setting update</param>
        public virtual void SetSetting<T>(string key, T value, bool clearCache = true)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            key = key.Trim().ToLowerInvariant();
            
            var settings = GetAllSettings();
            
            Setting setting;
            string valueStr = typeof(T).GetCustomTypeConverter().ConvertToInvariantString(value);
            if (settings.ContainsKey(key))
            {
                //update
                setting = settings[key];
                //little hack here because of EF issue
                setting = GetSettingById(setting.Id);
                setting.Value = valueStr;
                UpdateSetting(setting, clearCache);
            }
            else
            {
                //insert
                setting = new Setting
                              {
                                  Id = Guid.NewGuid(),
                                  Name = key,
                                  Value = valueStr,
                                  Created = DateTime.UtcNow,
                                  Modified = DateTime.UtcNow
                              };
                InsertSetting(setting, clearCache);
            }
        }

        /// <summary>
        /// Deletes a setting
        /// </summary>
        /// <param name="setting">Setting</param>
        public virtual void DeleteSetting(Setting setting)
        {
            if (setting == null)
                throw new ArgumentNullException("setting");

            using (var context = SCMSEntities.Define())
            {
                context.Settings.Attach(setting);
                ((IObjectContextAdapter)context).ObjectContext.ObjectStateManager.ChangeObjectState(setting, System.Data.EntityState.Modified);
                context.Settings.Remove(setting);
                context.SaveChanges();
            }

            //cache
            m_CacheService.RemoveByPattern(SettingsAllKey);
        }

        /// <summary>
        /// Gets all settings
        /// </summary>
        /// <returns>Setting collection</returns>
        public virtual IDictionary<string, Setting> GetAllSettings()
        {
            return m_CacheService.Get(SettingsAllKey, CacheTimeSpan.Infinite,
                () =>
                {
                    var context = SCMSEntities.Define();
                    {
                        var query = (from s in context.Settings.ToArray()
                                     orderby s.Name
                                     select s).ToArray();

                        var settings = query.Cast<Setting>().ToDictionary(s => s.Name.ToLowerInvariant());

                        return settings;
                    }
                });
        }

        /// <summary>
        /// Save settings object
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="settingInstance">Setting instance</param>
        public virtual void SaveSetting<T>(T settingInstance) where T : ISystemSettings, new()
        {
            //We should be sure that an appropriate Settings object will not be cached in IoC tool after updating (by default cached per HTTP request)
            DependencyResolver.Current.Resolve<SettingConfigurationProvider<T>>().SaveSettings(settingInstance);
        }

        /// <summary>
        /// Clear cache
        /// </summary>
        public virtual void ClearCache()
        {
            m_CacheService.RemoveByPattern(SettingsAllKey);
        }
        #endregion
    }
}