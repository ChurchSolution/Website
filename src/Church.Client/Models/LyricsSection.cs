// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LyricsSection.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class LyricsSection
    {
        [DataMember]
        public LyricsSectionType Type { get; set; }
        [DataMember]
        public IEnumerable<string> Text { get; set; }
    }
}
