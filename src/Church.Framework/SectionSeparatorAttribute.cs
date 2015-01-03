//-----------------------------------------------------------------------------
// <copyright file="SectionSeparatorAttribute.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class SectionSeparatorAttribute : Attribute
    {
        public string[] StartingStrings { get; private set; }

        public SectionSeparatorAttribute(params string[] startingStrings)
        {
            this.StartingStrings = startingStrings;
        }
    }

}
