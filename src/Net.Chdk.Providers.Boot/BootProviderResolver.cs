using Microsoft.Extensions.Logging;
using Net.Chdk.Providers.Category;
using System.Collections.Generic;

namespace Net.Chdk.Providers.Boot
{
    sealed class BootProviderResolver : ProviderResolver<IBootProvider>, IBootProviderResolver
    {
        #region Fields

        private ICategoryProvider CategoryProvider { get; }

        #endregion

        #region Constructor

        public BootProviderResolver(ICategoryProvider categoryProvider, ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
            CategoryProvider = categoryProvider;
        }

        #endregion

        #region IBootProviderResolver Members

        public IEnumerable<KeyValuePair<string, IBootProvider>> GetBootProviders()
        {
            return Providers;
        }

        public IBootProvider GetBootProvider(string categoryName)
        {
            return GetProvider(categoryName);
        }

        #endregion

        #region Providers

        protected override IEnumerable<string> GetNames()
        {
            return CategoryProvider.GetCategoryNames();
        }

        protected override IBootProvider CreateProvider(string categoryName)
        {
            return new BootProvider(categoryName, LoggerFactory);
        }

        #endregion
    }
}
