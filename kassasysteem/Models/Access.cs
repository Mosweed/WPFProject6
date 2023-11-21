using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kassasysteem.Models
{
    public class Access
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        public string token_type { get; set; }


        [JsonProperty("user")]
        public Employee _employee { get; set; }

    }
}
