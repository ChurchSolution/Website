// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Verse.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class Verse
    {
        [DataMember]
        public int Chapter { get; set; }
        [DataMember]
        public int Order { get; set; }
        [DataMember]
        public string Text { get; set; }
    }
}
