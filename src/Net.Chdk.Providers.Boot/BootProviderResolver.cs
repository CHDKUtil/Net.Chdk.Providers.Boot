using Microsoft.Extensions.Logging;
using Net.Chdk.Providers.Category;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Providers.Boot
{
    sealed class BootProviderResolver : IBootProviderResolver
    {
        #region Fields

        private ICategoryProvider CategoryProvider { get; }
        private ILoggerFactory LoggerFactory { get; }

        #endregion

        #region Constructor

        public BootProviderResolver(ICategoryProvider categoryProvider, ILoggerFactory loggerFactory)
        {
            CategoryProvider = categoryProvider;
            LoggerFactory = loggerFactory;

            providers = new Lazy<Dictionary<string, IBootProvider>>(GetProviders);
        }

        #endregion

        #region IBootProviderResolver Members

        public IEnumerable<KeyValuePair<string, IBootProvider>> GetBootProviders()
        {
            return Providers;
        }

        public IBootProvider GetBootProvider(string categoryName)
        {
            IBootProvider bootProvider;
            Providers.TryGetValue(categoryName, out bootProvider);
            return bootProvider;
        }

        #endregion

        #region Providers

        private readonly Lazy<Dictionary<string, IBootProvider>> providers;

        private Dictionary<string, IBootProvider> Providers => providers.Value;

        private Dictionary<string, IBootProvider> GetProviders()
        {
            return CategoryProvider.GetCategories()
                .ToDictionary(c => c, CreateBootProvider);
        }

        private IBootProvider CreateBootProvider(string categoryName)
        {
            return new BootProvider(categoryName, LoggerFactory);
        }

        #endregion
    }
}
