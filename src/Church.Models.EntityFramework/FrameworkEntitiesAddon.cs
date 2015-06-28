//-----------------------------------------------------------------------------
// <copyright file="FrameworkEntitiesAddon.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public partial class FrameworkEntities
    {
        public FrameworkEntities(string connectionString)
            : base(connectionString)
        {
        }
    }
}
