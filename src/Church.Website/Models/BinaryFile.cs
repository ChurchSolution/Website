namespace Church.Website.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web;

    public class BinaryFile
    {
        public byte[] Content { get; private set; }
        public string Extension { get; private set; }
        public string Type { get; private set; }

        public string Filename { get; set; }

        public static BinaryFile FromBase64(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                return null;
            }

            var positionColon = encoded.IndexOf(':');
            var positionSemicolon = encoded.IndexOf(';');
            var positionComma = encoded.IndexOf(',');
            if (positionColon < 0 || positionSemicolon < positionColon || positionComma < positionSemicolon)
            {
                throw new FormatException("encoded");
            }

            var file = new BinaryFile
            {
                Type = encoded.Substring(positionColon + 1, positionSemicolon - positionColon - 1),
                Content = Convert.FromBase64String(encoded.Substring(positionComma + 1)),
            };
            switch(file.Type)
            {
                case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                    file.Extension = "docx";
                    break;
                case "application/pdf":
                    file.Extension = "pdf";
                    break;
                case "application/msword":
                    file.Extension = "doc";
                    break;
                default:
                    file.Extension = "unknown";
                    break;
            }

            return file;
        }

        public int Save(string path)
        {
            var filename = Path.Combine(path, this.Filename);
            using (var stream = File.Create(filename))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(this.Content);
                }
            }

            return this.Content.Length;
        }
    }
}
