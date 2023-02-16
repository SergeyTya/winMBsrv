using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public class CustomControlsReader
    {

        [JsonProperty("Device")]
        public string Device { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Options")]
        public List<List<string>> Options { get; set; }

        [JsonProperty("Control")]
        public bool Control { get; set; }

        [JsonProperty("Max")]
        public int Max { get; set; }

        [JsonProperty("Min")]
        public int Min { get; set; }

        [JsonProperty("Scl")]
        public int Scl { get; set; }

        [JsonProperty("Adr")]
        public int Adr { get; set; }

        [JsonProperty("Descript")]
        public string Descript { get; set; }

        [JsonProperty("Dim")]
        public string Dim { get; set; }
    }

    public class CustomControlTyple
    {
        public TextBox textboxIndicator { get; set; }
        public TrackBar trackBarController { get; set; }
        public UInt16 index { get; set; }

    }

}
