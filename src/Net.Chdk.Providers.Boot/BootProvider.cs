using Net.Chdk.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Net.Chdk.Providers.Boot
{
    sealed class BootProvider : IBootProvider
    {
        #region Constructor

        public BootProvider()
        {
            bytes = new Lazy<Dictionary<string, Dictionary<int, byte[]>>>(GetBytes);
        }

        #endregion

        #region IBootProvider Members

        public IDictionary<int, byte[]> GetBytes(string fileSystem)
        {
            Dictionary<int, byte[]> bytes;
            Bytes.TryGetValue(fileSystem, out bytes);
            return bytes;
        }

        public string FileName => Boot.FileName;

        #endregion

        #region Boot

        private const string BootInfoFileName = "boot.json";

        private sealed class BootInfo
        {
            public string FileName { get; set; }
            public Dictionary<string, Dictionary<string, string>> Strings { get; set; }
        }

        private readonly Lazy<BootInfo> boot = new Lazy<BootInfo>(GetBoot);

        private BootInfo Boot => boot.Value;

        private static BootInfo GetBoot()
        {
            using (var reader = File.OpenText(BootInfoFileName))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return JsonSerializer.CreateDefault(Settings).Deserialize<BootInfo>(jsonReader);
            }
        }

        private static JsonSerializerSettings Settings => new JsonSerializerSettings
        {
            Converters = new[] { new HexStringJsonConverter() }
        };

        #endregion

        #region Bytes

        private readonly Lazy<Dictionary<string, Dictionary<int, byte[]>>> bytes;

        private Dictionary<string, Dictionary<int, byte[]>> Bytes => bytes.Value;

        private Dictionary<string, Dictionary<int, byte[]>> GetBytes()
        {
            return Boot.Strings.ToDictionary(kvp => kvp.Key, GetBytes);
        }

        private static Dictionary<int, byte[]> GetBytes(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            return kvp.Value.ToDictionary(GetStartIndex, GetBytes);
        }

        private static int GetStartIndex(KeyValuePair<string, string> kvp)
        {
            return Convert.ToInt32(kvp.Key, 16);
        }

        private static byte[] GetBytes(KeyValuePair<string, string> kvp)
        {
            return Encoding.ASCII.GetBytes(kvp.Value);
        }

        #endregion
    }
}
