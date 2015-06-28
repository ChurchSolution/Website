// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BibleExtensions.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// <summary>
//   Default the Bible extensions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System.Linq;

    /// <summary>
    /// Provides the extensions for Bible.
    /// </summary>
    public static class BibleExtensions
    {
        /// <summary>
        /// Gets the verse pattern.
        /// </summary>
        /// <param name="bible">The bible.</param>
        /// <returns>The <see cref="string"/> for the verse pattern.</returns>
        public static string GetAbbreviations(this Bible bible)
        {
            var bookNames = bible.Books.SelectMany(b => new[] { b.Name, b.Abbreviation });

            // TODO Considering to move to Bible handler for multiple languages.
            return "(" + string.Join("|", bookNames)
                   + ")[ ]*([0-9]+)[ ]*[:：]([ ]*[0-9]+(-[ ]*[0-9]+)?([，,][ ]*[0-9]+(-[ ]*[0-9]+)?)*)";
        }
    }
}
