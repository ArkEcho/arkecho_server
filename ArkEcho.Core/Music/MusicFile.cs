﻿using System;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MusicFile : TransferFileBase
    {
        [JsonProperty]
        public Guid Album { get; set; } = Guid.Empty;

        [JsonProperty]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonProperty]
        public string Title { get; set; } = string.Empty;

        [JsonProperty]
        public string Performer { get; set; } = string.Empty;

        [JsonProperty]
        public int Disc { get; set; } = 0;

        [JsonProperty]
        public int Track { get; set; } = 0;

        [JsonProperty]
        public int Year { get; set; } = 0;

        /// <summary>
        /// Duration of the Musicfile in Milliseconds.
        /// </summary>
        [JsonProperty]
        public int Duration { get; set; } = 0;

        /// <summary>
        /// For Serialization and Unit Tests
        /// </summary>
        public MusicFile() : base() { }

        public MusicFile(string filePath) : base(filePath)
        {
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Performer} - {Title}";
            }
        }
    }
}
