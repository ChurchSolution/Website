//-----------------------------------------------------------------------------
// <copyright file="ExceptionUtilities.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Church.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides an exception utilities for checking parameters.
    /// </summary>
    public static class ExceptionUtilities
    {
        /// <summary>
        /// Throws an argument null exception if an argument is null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="errorMessage">The errorMessage.</param>
        public static void ThrowArgumentNullExceptionIfEmpty(string argument, string argumentName, string errorMessage = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw errorMessage == null ? new ArgumentNullException(argumentName) : new ArgumentNullException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Throws an argument null exception if an argument is null.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="errorMessage">The errorMessage.</param>
        public static void ThrowArgumentNullExceptionIfNull(object argument, string argumentName, string errorMessage = null)
        {
            if (argument == null)
            {
                throw errorMessage == null ? new ArgumentNullException(argumentName) : new ArgumentNullException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Throws an argument null exception for <see cref="IEnumerable{T}" /> instances.
        /// </summary>
        /// <typeparam name="T">The type of instances.</typeparam>
        /// <param name="argument">The argument.</param>
        /// <param name="argumentName">The argument name.</param>
        /// <param name="errorMessage">The errorMessage.</param>
        public static void ThrowArgumentNullExceptionIfEmpty<T>(IEnumerable<T> argument, string argumentName, string errorMessage)
        {
            if (argument == null || !argument.Any())
            {
                throw new ArgumentNullException(argumentName, errorMessage);
            }
        }

        /// <summary>
        /// Throws an invalid operation exception if the predication is false.
        /// </summary>
        /// <param name="prediction">The predication</param>
        /// <param name="errorFormat">The error format.</param>
        /// <param name="args">The error arguments.</param>
        public static void ThrowFormatExceptionIfFalse(bool prediction, string errorFormat, params object[] args)
        {
            if (!prediction)
            {
                throw new InvalidOperationException(string.Format(errorFormat, args));
            }
        }
    }
}
