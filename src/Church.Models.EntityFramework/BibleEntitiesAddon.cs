﻿//-----------------------------------------------------------------------------
// <copyright file="BibleEntitiesAddon.cs">
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

    public partial class BibleEntities
    {
        public BibleEntities(string connectionString)
            : base(connectionString)
        {
        }
    }
}
