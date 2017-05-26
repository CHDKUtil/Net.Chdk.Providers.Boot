using System.Linq;

namespace Net.Chdk.Providers.Boot
{
    sealed class BootProvider : BootProvider<BootProvider.BootData>, IBootProvider
    {
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

        protected override string DataFileName => "boot.json";

        #endregion
    }
}
