using System.IO;
using System.Linq;

namespace Net.Chdk.Providers.Boot
{
    sealed class BootProvider : BootProvider<BootProvider.BootData>, IBootProvider
    {
        #region Constants

        private const string CategoryPath = "Category";

        #endregion

        #region Constructor

        public BootProvider(string categoryName)
        {
            CategoryName = categoryName;
        }

        #endregion

        #region IBootProvider Members

        public string FileName => Data.Files.First().Key;
        public int[][] Offsets => Data.Offsets;
        public byte[] Prefix => Data.Prefix;

        #endregion

        #region Data

        internal sealed class BootData : DataBase
        {
            public int[][] Offsets { get; set; }
            public byte[] Prefix { get; set; }
        }

        private string CategoryName { get; }

        protected override string DataPath => Path.Combine(base.DataPath, CategoryPath, CategoryName);

        protected override string DataFileName => "boot.json";

        #endregion
    }
}
