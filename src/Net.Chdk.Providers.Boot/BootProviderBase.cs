using Microsoft.Extensions.Logging;
using Net.Chdk.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Net.Chdk.Providers.Boot
{
    abstract class BootProvider<TData>
        where TData : BootProvider<TData>.DataBase
    {
        #region Fields

        private ILogger Logger { get; }

        #endregion

        #region Constructor

        protected BootProvider(ILogger logger)
        {
            Logger = logger;

            data = new Lazy<TData>(GetData);
            bytes = new Lazy<Dictionary<string, Dictionary<int, byte[]>>>(GetBytes);
            files = new Lazy<Dictionary<string, byte[]>>(DoGetFiles);
        }

        #endregion

        #region Public Methods

        public IDictionary<int, byte[]> GetBytes(string fileSystem)
        {
            Dictionary<int, byte[]> bytes;
            Bytes.TryGetValue(fileSystem, out bytes);
            return bytes;
        }

        #endregion

        #region Serializer

        private static readonly Lazy<JsonSerializer> serializer = new Lazy<JsonSerializer>(GetSerializer);

        private static JsonSerializer Serializer => serializer.Value;

        private static JsonSerializer GetSerializer()
        {
            return JsonSerializer.CreateDefault();
        }

        #endregion

        #region Data

        internal abstract class DataBase
        {
            public Dictionary<string, Dictionary<string, string>> Strings { get; set; }
            public Dictionary<string, string> Files { get; set; }
        }

        private readonly Lazy<TData> data;

        protected TData Data => data.Value;

        protected virtual string DataPath => "Data";

        protected abstract string DataFileName { get; }

        private TData GetData()
        {
            var filePath = Path.Combine(DataPath, DataFileName);
            using (var reader = File.OpenText(filePath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return Serializer.Deserialize<TData>(jsonReader);
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
            return Data.Strings.ToDictionary(kvp => kvp.Key, GetBytes);
        }

        private static Dictionary<int, byte[]> GetBytes(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            return kvp.Value.ToDictionary(GetStartIndex, GetBytes);
        }

        #endregion

        #region Files

        private Lazy<Dictionary<string, byte[]>> files;

        protected Dictionary<string, byte[]> Files => files.Value;

        private Dictionary<string, byte[]> DoGetFiles()
        {
            return Data.Files.ToDictionary(kvp => kvp.Key, GetBytes);
        }

        #endregion

        #region Helper Methods

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
