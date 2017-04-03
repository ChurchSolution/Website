// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Scripture.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Scripture
    {
        [DataMember]
        public string Language { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string Book { get; set; }
        [DataMember]
        public IEnumerable<Verse> Verses { get; set; }
    }
}
