using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Frends.Kungsbacka.Json
{
    internal class Mapping
    {
        private object _default;
        private bool _defaultPresent;
        private List<string> _transformations;

        [JsonProperty("from", Required = Required.Always)]
        public object From { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("def")]
        public object Default
        {
            get => _default;
            set
            {
                _defaultPresent = true;
                _default = value;
            }
        }

        [JsonProperty("trans")]
        public List<string> Transformations
        {
            get
            {
                if (_transformations == null)
                {
                    _transformations = new List<string>();
                }
                return _transformations;
            }
            set => _transformations = value;
        }

        public bool HasTransformation(string transformation)
        {
            return _transformations?.Contains(transformation, StringComparer.OrdinalIgnoreCase) ?? false;
        }

        public bool DefaultPresent => _defaultPresent;
    }
}
