// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientResult.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class HttpClientResult
    {
        [DataMember]
        public bool Success { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}
