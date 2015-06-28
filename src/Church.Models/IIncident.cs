//-----------------------------------------------------------------------------
// <copyright file="IIncident.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Models
{
    using System;

    public interface IIncident
    {
        DateTime Time { get; }

        string Username { get; }

        string Fullname { get; }

        string Description { get; }
    }
}
