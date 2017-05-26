using Net.Chdk.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Net.Chdk.Providers.Boot
{
    sealed class ScriptProvider : IScriptProvider
    {
        #region Constructor

        public ScriptProvider()
        {
            bytes = new Lazy<Dictionary<string, Dictionary<int, byte[]>>>(GetBytes);
            files = new Lazy<Dictionary<string, byte[]>>(DoGetFiles);
        }

        #endregion

        #region IScriptProvider Members

        public IDictionary<int, byte[]> GetBytes(string fileSystem)
        {
            Dictionary<int, byte[]> bytes;
            Bytes.TryGetValue(fileSystem, out bytes);
            return bytes;
        }

        public IDictionary<string, byte[]> GetFiles()
        {
            return Files;
        }

        #endregion

        #region Script

        private const string DataPath = "Data";
        private const string ScriptDataFileName = "script.json";

        private sealed class ScriptData
        {
            public Dictionary<string, Dictionary<string, string>> Strings { get; set; }
            public Dictionary<string, string> Files { get; set; }
        }

        private readonly Lazy<ScriptData> boot = new Lazy<ScriptData>(GetScript);

        private ScriptData Script => boot.Value;

        private static ScriptData GetScript()
        {
            var filePath = Path.Combine(DataPath, ScriptDataFileName);
            using (var reader = File.OpenText(filePath))
            using (var jsonReader = new JsonTextReader(reader))
            {
                return JsonSerializer.CreateDefault(Settings).Deserialize<ScriptData>(jsonReader);
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
            return Script.Strings.ToDictionary(kvp => kvp.Key, GetBytes);
        }

        private static Dictionary<int, byte[]> GetBytes(KeyValuePair<string, Dictionary<string, string>> kvp)
        {
            return kvp.Value.ToDictionary(GetStartIndex, GetBytes);
        }

        #endregion

        #region Files

        private Lazy<Dictionary<string, byte[]>> files;

        private Dictionary<string, byte[]> Files => files.Value;

        private Dictionary<string, byte[]> DoGetFiles()
        {
            return Script.Files.ToDictionary(kvp => kvp.Key, GetBytes);
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
