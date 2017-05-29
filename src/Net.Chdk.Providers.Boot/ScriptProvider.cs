using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Net.Chdk.Providers.Boot
{
    sealed class ScriptProvider : BootProvider<ScriptProvider.ScriptData>, IScriptProvider
    {
        #region Constructor

        public ScriptProvider(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<ScriptProvider>())
        {
        }

        #endregion

        #region IScriptProvider Members

        public IDictionary<string, byte[]> GetFiles()
        {
            return Files;
        }

        #endregion

        #region Data

        internal sealed class ScriptData : DataBase
        {
        }

        protected override string DataFileName => "script.json";

        #endregion
    }
}
