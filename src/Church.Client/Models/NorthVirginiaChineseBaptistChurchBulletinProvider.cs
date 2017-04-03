// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NorthVirginiaChineseBaptistChurchBulletinProvider.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public class NorthVirginiaChineseBaptistChurchBulletinProvider : AbstractBulletinProvider
    {
        public Dictionary<string, string> TextMap { get; internal set; }

        public NorthVirginiaChineseBaptistChurchBulletinProvider()
        {
        }

        protected string InitializeFromWord(string filename)
        {
            Microsoft.Office.Interop.Word._Application wordApp = null;
            try
            {
                //TODO test
                wordApp = new Microsoft.Office.Interop.Word.Application();
                wordApp.Application.Visible = false;
                //wordApp.WindowState = Microsoft.Office.Interop.Word.WdWindowState.wdWindowStateMinimize;
                //object missing = Type.Missing;

                object source = filename;
                wordApp.Documents.Open(ref source);
                Microsoft.Office.Interop.Word._Document document = wordApp.ActiveDocument;
                //this.PlainText = document.Content.Text;

                string txtFilename = Path.ChangeExtension(filename, ".txt");
                object fileName = txtFilename;
                object fileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatUnicodeText;
                //object LockComments = false;
                //object AddToRecentFiles = false;
                //object ReadOnlyRecommended = false;
                //object EmbedTrueTypeFonts = true;
                //object SaveNativePictureFormat = false;
                //object SaveFormsData = false;
                //object SaveAsAOCELetter = false;
                object encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
                //object InsertLineBreaks = false;
                //object AllowSubstitutions = false;
                object lineEnding = Microsoft.Office.Interop.Word.WdLineEndingType.wdCRLF;
                //object AddBiDiMarks = false;

                //if (!this._existsTextFile)
                {
                    //document.SaveAs(ref fileName, ref FileFormat, ref LockComments,
                    //    ref missing, ref AddToRecentFiles, ref missing,
                    //    ref ReadOnlyRecommended, ref EmbedTrueTypeFonts,
                    //    ref SaveNativePictureFormat, ref SaveFormsData,
                    //    ref SaveAsAOCELetter, ref Encoding, ref InsertLineBreaks,
                    //    ref AllowSubstitutions, ref LineEnding, ref AddBiDiMarks);
                    document.SaveAs(FileName: ref fileName,
                        FileFormat: ref fileFormat,
                        Encoding: ref encoding,
                        LineEnding: ref lineEnding);
                }

                //FileName = printableFilename;
                //FileFormat = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatPDF;

                //if (!this._existsPrintableFile)
                //{
                //    wordApp.ActiveDocument.SaveAs(ref FileName, ref FileFormat, ref LockComments,
                //        ref missing, ref AddToRecentFiles, ref missing,
                //        ref ReadOnlyRecommended, ref EmbedTrueTypeFonts,
                //        ref SaveNativePictureFormat, ref SaveFormsData,
                //        ref SaveAsAOCELetter, ref /*Encoding*/missing, ref InsertLineBreaks,
                //        ref AllowSubstitutions, ref LineEnding, ref AddBiDiMarks);
                //}

                document.Close();

                return txtFilename;
            }
            finally
            {
                //wordApp.Documents.Close();
                wordApp.Quit();
            }
        }

        public override void Initialize(string filename)
        {
            var txtFilename = this.InitializeFromWord(filename);
            this.TextMap = new Dictionary<string, string>();
            var plainText = this.Read(txtFilename);
            this.TextMap.Add("zh-CN", Utilities.ToSChinese(plainText));
            this.TextMap.Add("zh-TW", Utilities.ToTChinese(plainText));
        }

        private string Read(string filename)
        {
            using (var sr = new StreamReader(filename))
            {
                return sr.ReadToEnd();
            }
        }

        public override IEnumerable<Bulletin> Make(DateTime date)
        {
            Trace.Assert(null != this.TextMap);
            var lstBulletin = new List<Bulletin>();
            foreach (var m in this.TextMap)
            {
                BulletinTextBuilder builder = null;
                if ("zh-TW".Equals(m.Key))
                {
                    builder = new NorthVirginiaChineseBaptistChurchTwBulletinBuilder();
                }
                else
                {
                    builder = new NorthVirginiaChineseBaptistChurchCnBulletinBuilder();
                }
                var bulletin = builder.Make<NorthVirginiaChineseBaptistChurchBulletin>(m.Value);
                lstBulletin.Add(bulletin);
            }

            return lstBulletin;
        }

        public override void MakePresentations(DataTransfer transfer, IEnumerable<Bulletin> bulletins, string path)
        {
            foreach (var b in bulletins)
            {
                if ("zh-CN".Equals(b.Culture))
                {
                    var builder = new NorthVirginiaChineseBaptistChurchPresentationBuilder(transfer, b, path);
                    builder.Make();
                }
            }
        }

        public override bool Upload(DataTransfer transfer, string uploader, System.Security.SecureString securePassword, DateTime date)
        {
            bool res = true;
            foreach (var m in this.TextMap)
            {
                res &= transfer.UploadBulletin(uploader, securePassword, date, m.Value, m.Key);
            }

            return res;
        }
    }
}
