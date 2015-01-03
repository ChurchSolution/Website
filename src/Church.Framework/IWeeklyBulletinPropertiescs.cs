//-----------------------------------------------------------------------------
// <copyright file="IWeeklyBulletinProperties.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Model
{
    using System.Collections.Generic;

    public interface IWeeklyBulletinProperties
    {
        Dictionary<string, decimal> LastWeekData { get; }

        bool Verify();
    }
}
