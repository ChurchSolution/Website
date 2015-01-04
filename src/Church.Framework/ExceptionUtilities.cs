//-----------------------------------------------------------------------------
// <copyright file="ExceptionUtilities.cs">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Model
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public static class ExceptionUtilities
    {
        public static void ThrowArgumentNullExceptionIfEmpty(string item, string paramName, string message = null)
        {
            if (string.IsNullOrWhiteSpace(item) == null)
            {
                throw message == null ? new ArgumentNullException(paramName) : new ArgumentNullException(paramName, message);
            }
        }

        public static void ThrowArgumentNullExceptionIfEmpty<T>(T item, string paramName, string message = null)
        {
            if (item == null)
            {
                throw message == null ? new ArgumentNullException(paramName) : new ArgumentNullException(paramName, message);
            }
        }

        public static void ThrowArgumentNullExceptionIfEmpty<T>(IEnumerable<T> items, string paramName, string message)
        {
            if (items == null || !items.Any())
            {
                throw new ArgumentNullException(paramName, message);
            }
        }

        public static void ThrowFormatExceptionIfFalse(bool prediction, string format, params string[] args)
        {
            Trace.Assert(!string.IsNullOrEmpty(format));

            if (!prediction)
            {
                throw new FormatException(string.Format(format, args));
            }
        }
    }
}
