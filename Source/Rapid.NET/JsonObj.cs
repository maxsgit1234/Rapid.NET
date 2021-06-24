using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace Rapid.NET
{
    public class JsonObj
    {
        public JsonElement Element;

        public JsonObj(JsonElement element)
        {
            Element = element;
        }

        public JsonObj this[string key]
        {
            get
            {
                var items = Objects();
                if (items == null || !items.ContainsKey(key))
                    return null;

                return items[key];
            }
        }

        public JsonObj GetChild(params string[] keys)
        {
            JsonObj ret = this;
            foreach (string key in keys)
            {
                var items = ret.Objects();
                if (items == null || !items.ContainsKey(key))
                    return null;

                ret = items[key];
            }
            return ret;
        }

        public JsonObj this[int index]
        {
            get
            {
                var arr = Array();
                if (arr == null || index < 0 || index >= arr.Length)
                    return null;

                return arr[index];
            }
        }

        public JsonValueKind Kind { get { return Element.ValueKind; } }

        public Dictionary<string, JsonObj> Objects()
        {
            if (Kind != JsonValueKind.Object)
                return null;

            var ret = new Dictionary<string, JsonObj>();
            foreach (JsonProperty item in Element.EnumerateObject())
                ret.Add(item.Name, new JsonObj(item.Value));
            return ret;
        }

        public JsonObj[] Array()
        {
            if (Kind != JsonValueKind.Array)
                return null;

            return Element.EnumerateArray().Select(i => new JsonObj(i)).ToArray();
        }

        public string String()
        {
            if (Kind != JsonValueKind.String)
                return null;

            return Element.GetString();
        }

        public double Number()
        {
            if (Kind != JsonValueKind.Number)
                return double.NaN;

            return Element.GetDouble();
        }

        public bool? Boolean()
        {
            if (Kind == JsonValueKind.False)
                return false;
            else if (Kind == JsonValueKind.True)
                return true;
            else
                return null;
        }

        public static JsonSerializerOptions Options()
        {
            var opts = new JsonSerializerOptions();
            opts.IncludeFields = true;
            opts.WriteIndented = true;
            opts.IgnoreReadOnlyProperties = true;
            return opts;
        }

        public static T Deserialize<T>(string text)
        {
            return JsonSerializer.Deserialize<T>(text, Options());
        }

        public static object Deserialize(string text, Type t)
        {
            return JsonSerializer.Deserialize(text, t, Options());
        }

        public static string Serialize<T>(T item)
        {
            return JsonSerializer.Serialize(item, Options());
        }

        public static JsonObj Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            JsonElement element = (JsonElement)JsonSerializer
                .Deserialize<object>(text, Options());

            return new JsonObj(element);
        }

        public override string ToString()
        {
            return Serialize(Element);
        }
    }
}
