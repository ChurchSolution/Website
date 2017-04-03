// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataTransfer.cs" company="Church">
//   Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    public class DataTransfer
    {
        public string HttpHost { get; internal set; }

        public DataTransfer(string httpHost)
        {
            this.HttpHost = httpHost;
        }

        private void LogOn(HttpClient client, string userName, SecureString securePassword)
        {
            Debug.Assert(null != client);
            Debug.Assert(!string.IsNullOrEmpty(userName));
            Debug.Assert(null != securePassword);

            var pairs = new Dictionary<string, string>();
            pairs.Add("UserName", userName);
            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                pairs.Add("Password", Marshal.PtrToStringUni(unmanagedString));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
            
            pairs.Add("RememberMe", false.ToString());
            string uri = this.HttpHost + @"/Account/LogOn";
            var content = new FormUrlEncodedContent(pairs);
            var responseResult = client.PostAsync(uri, content).Result;
            responseResult.EnsureSuccessStatusCode();
            if (!responseResult.RequestMessage.RequestUri.AbsoluteUri.EndsWith("/Member"))
            {
                throw new UnauthorizedAccessException("Could not log on the website.");
            }
        }

        public bool UploadBulletin(string userName, SecureString securePassword, DateTime date, string plainText, string culture, string printableFilename = null)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            if (null == securePassword)
            {
                throw new ArgumentNullException("securePassword");
            }
            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException("culture");
            }

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                CookieContainer cookies = new CookieContainer();
                handler.CookieContainer = cookies;
                cookies.Add(new Uri(this.HttpHost), new Cookie("culture", culture));

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    this.LogOn(client, userName, securePassword);

                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(date.ToShortDateString()), "\"Date\"");
                    // convert string to stream
                    byte[] byteArray = Encoding.UTF8.GetBytes(plainText);
                    MemoryStream stream = new MemoryStream(byteArray);
                    form.Add(new StreamContent(stream), "\"TextFile\"", "\"" + "Temporary.txt" + "\"");
                    if (!string.IsNullOrEmpty(printableFilename))
                    {
                        var fiPrintableFile = new FileInfo(printableFilename); 
                        form.Add(new StreamContent(fiPrintableFile.OpenRead()), "\"PrintableFile\"", "\"" + fiPrintableFile.Name + "\"");
                    }
                    var uri = this.HttpHost + @"/Library/UploadBulletin";

                    var responseResult = client.PostAsync(uri, form).Result;
                    responseResult.EnsureSuccessStatusCode();
                    var responseContent = responseResult.Content;
                    var ret = responseContent.ReadAsStringAsync().Result;

                    return ret.Contains("成功。");
                }
            }
        }

        public Scripture GetBibleVerses(string abbr)
        {
            using (var client = new HttpClient())
            {
                string uri = this.HttpHost + @"/Bible/Verses/" + abbr.Trim();
                var ret = client.GetStringAsync(uri).Result;
                return Utilities.ParseFromJson<Scripture>(ret);
            }
        }

        public bool UploadBible(string userName, SecureString securePassword, Bible bible, string culture)
        {
            if (null == bible)
            {
                throw new ArgumentNullException("bible");
            }

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                CookieContainer cookies = new CookieContainer();
                handler.CookieContainer = cookies;
                cookies.Add(new Uri(this.HttpHost), new Cookie("culture", culture));
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    this.LogOn(client, userName, securePassword);

                    var bibleId = this.UploadBible(client, bible.Version, bible.Language, bible.IsDefault, bible.Culture);
                    foreach (var b in bible.Books)
                    {
                        var bookId = this.UploadBibleBook(client, bibleId, b.Order, b.Name, b.Abbreviation);
                        foreach (var c in b.Chapters)
                        {
                            while (true)
                            {
                                try
                                {
                                    this.UploadBibleChapter(client, bookId, c.Order, c.Verses);
                                    break;
                                }
                                catch
                                {
                                    Console.WriteLine("Error");
                                }
                            }
                        }
                        Console.WriteLine("Done - " + b.Order.ToString());
                    }
                }
            }

            return true;
        }

        protected Guid UploadBible(HttpClient client, string version, string language, bool isDefault, string culture)
        {
            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("version");
            }
            if (string.IsNullOrEmpty(language))
            {
                throw new ArgumentNullException("language");
            }
            if (string.IsNullOrEmpty(culture))
            {
                throw new ArgumentNullException("culture");
            }

            var pairs = new Dictionary<string, string>();
            //pairs.Add("version", version);
            pairs.Add("isDefault", isDefault.ToString());
            pairs.Add("language", language);

            string uri = this.HttpHost + @"/Bible/Bibles/" + version;
            var content = new FormUrlEncodedContent(pairs);
            var responseResult = client.PutAsync(uri, content).Result;
            responseResult.EnsureSuccessStatusCode();
            var responseContent = responseResult.Content;
            string ret = responseContent.ReadAsStringAsync().Result;
            var res = Utilities.ParseFromJson<HttpClientResult>(ret);

            if (res.Success)
            {
                return new Guid(res.Message);
            }
            else
            {
                throw new HttpRequestException(res.Message);
            }
        }

        public Guid UploadBibleBook(HttpClient client, Guid bibleId, int order, string name, string abbreviation)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (string.IsNullOrEmpty(abbreviation))
            {
                throw new ArgumentNullException("abbreviation");
            }

            var pairs = new Dictionary<string, string>();
            pairs.Add("abbreviation", abbreviation);
            pairs.Add("name", name);

            string uri = this.HttpHost + @"/Bible/Bibles/" + bibleId + "/Books/" + order.ToString();
            var content = new FormUrlEncodedContent(pairs);
            var responseResult = client.PutAsync(uri, content).Result;
            responseResult.EnsureSuccessStatusCode();
            var responseContent = responseResult.Content;
            string ret = responseContent.ReadAsStringAsync().Result;
            var res = Utilities.ParseFromJson<HttpClientResult>(ret);

            if (res.Success)
            {
                return new Guid(res.Message);
            }
            else
            {
                throw new HttpRequestException(res.Message);
            }
        }

        public void UploadBibleChapter(HttpClient client, Guid bookId, int chapter, IEnumerable<string> verses)
        {
            if (null == verses)
            {
                throw new ArgumentNullException("verses");
            }

            var pairs = new List<KeyValuePair<string, string>>();
            foreach (var v in verses)
            {
                pairs.Add(new KeyValuePair<string, string>("verses", v));
            }

            string uri = this.HttpHost + @"/Bible/Books/" + bookId + "/Chapters/" + chapter;
            var content = new FormUrlEncodedContent(pairs);
            var responseResult = client.PutAsync(uri, content).Result;
            responseResult.EnsureSuccessStatusCode();
            var responseContent = responseResult.Content;
            string ret = responseContent.ReadAsStringAsync().Result;
            var res = Utilities.ParseFromJson<HttpClientResult>(ret);

            if (!res.Success)
            {
                throw new HttpRequestException(res.Message);
            }
        }

        public bool UploadHymn(string userName, SecureString securePassword, Hymn hymn, string culture)
        {
            if (null == hymn)
            {
                throw new ArgumentNullException("hymn");
            }
            if (string.IsNullOrEmpty(hymn.Name))
            {
                throw new ArgumentNullException("hymn.Name");
            }
            if (null == hymn.Source)
            {
                throw new ArgumentNullException("hymn.Source");
            }
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                CookieContainer cookies = new CookieContainer();
                handler.CookieContainer = cookies;
                cookies.Add(new Uri(this.HttpHost), new Cookie("culture", culture));
                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    this.LogOn(client, userName, securePassword);

                    var pairs = new Dictionary<string, string>();
                    pairs.Add("source", hymn.Source);
                    pairs.Add("lyrics", hymn.Lyrics);
                    pairs.Add("links", hymn.Links);

                    string uri = this.HttpHost + @"/Library/Hymns/" + hymn.Name;
                    var content = new FormUrlEncodedContent(pairs);
                    var responseResult = client.PutAsync(uri, content).Result;
                    responseResult.EnsureSuccessStatusCode();
                    var responseContent = responseResult.Content;
                    string ret = responseContent.ReadAsStringAsync().Result;

                    return bool.Parse(ret);
                }
            }
        }

        public Hymn GetHymn(string name, string source)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if (null == source)
            {
                throw new ArgumentNullException("source");
            }

            using (HttpClientHandler handler = new HttpClientHandler())
            {
                using (var client = new HttpClient())
                {
                    string uri = this.HttpHost + @"/Library/Hymns/" + name + "?source=" + Uri.EscapeDataString(source);
                    var ret = client.GetStringAsync(uri).Result;

                    return Utilities.ParseFromJson<Hymn>(ret);
                }
            }
        }
    }
}
