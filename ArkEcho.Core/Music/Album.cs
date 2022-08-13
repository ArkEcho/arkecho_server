﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Album : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        [JsonProperty]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public int TrackCount { get; set; } = 0;

        [JsonProperty]
        public int DiscCount { get; set; } = 0;

        [JsonProperty]
        public int Year { get; set; } = 0;

        public string Cover64 { get; set; } = null;

        public Album() : base() { }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
