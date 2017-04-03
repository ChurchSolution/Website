// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionPatternsAttribute.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public sealed class SectionPatternsAttribute : Attribute
    {
        public string[] Patterns { get; private set; }
        public SectionPatternsAttribute(params string[] patterns)
        {
            this.Patterns = patterns;
        }
    }
}
