using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UntisNotifier.WebUntis.Models
{
    public class LoginResult
    {
        [JsonProperty("reload")]
        public bool Reload { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
    }
}
