using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace SwitchHosts
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(HostConfiguration))] 
    internal partial class JsonSourceGenerationContext : JsonSerializerContext
    {
    }
}
