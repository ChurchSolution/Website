// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Church">
//     Copyright (c) Rui Min. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Church.Models
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Security;

    using Microsoft.VisualBasic;

    public static class Utilities
    {
        public static SecureString GetPasswordFromConsole(string username)
        {
            var password = new SecureString();

            Console.WriteLine("Username: " + username);
            Console.Write("Password: ");
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (0 < password.Length)
                    {
                        password.RemoveAt(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    password.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();

            return password;
        }

        public static string ToJson<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                var ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(stream, obj);

                stream.Position = 0;
                var sr = new StreamReader(stream);
                return sr.ReadToEnd();
            }
        }

        public static T ParseFromJson<T>(string json)
        {
            using (var stream = new MemoryStream())
            {
                StreamWriter writer=new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                var ser = new DataContractJsonSerializer(typeof(T));

                stream.Position = 0;
                return (T)ser.ReadObject(stream);
            }
        }

        ///   <summary>
        ///   转换为简体中文 
        ///   </summary> 
        public static string ToSChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }

        ///   <summary> 
        ///   转换为繁体中文 
        ///   </summary> 
        public static string ToTChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }
    }
}
