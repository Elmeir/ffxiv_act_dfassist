﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace DFAssist
{
    internal class Localization
    {
        private static Dictionary<string, string> _localizedMap;

        public static void Initialize(string language)
        {
            var json = WebInteractions.DownloadString($"https://raw.githubusercontent.com/easly1989/ffxiv_act_dfassist/master/localization/{language}.json");
            _localizedMap = string.IsNullOrWhiteSpace(json) ? new Dictionary<string, string>() : JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        }

        public static string GetText(string key, params object[] args)
        {
            return _localizedMap == null || !_localizedMap.TryGetValue(key, out var value) ? $"<{key}>" : string.Format(value, args);
        }
    }
}
