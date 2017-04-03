// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractBulletinProvider.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security;

    public abstract class AbstractBulletinProvider
    {
        public abstract void Initialize(string filename);
        public abstract IEnumerable<Bulletin> Make(DateTime date);
        public abstract void MakePresentations(DataTransfer transfer, IEnumerable<Bulletin> bulletins, string path);
        public abstract bool Upload(DataTransfer transfer, string uploader, SecureString securePassword, DateTime date);

        public static void Save(string plainText, string filename)
        {
            if (null == plainText)
            {
                throw new ArgumentNullException("plainText");
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            var lstFilename = new List<string>();
            using (var sw = new StreamWriter(filename))
            {
                sw.Write(plainText);
            }
            lstFilename.Add(filename);

        }
    }
}
