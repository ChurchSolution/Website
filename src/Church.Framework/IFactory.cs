//-----------------------------------------------------------------------------
// <copyright file="IBulletinFactory.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IFactory
    {
        WeeklyBulletin CreateBulletin(DateTime date, string fileUri, string plainText);
    }
}
