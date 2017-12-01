using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Beeple_office
{
    public class MenuPageMenuItem
    {
        [JsonProperty(PropertyName = "success")]
        public string Success { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
    }
}
