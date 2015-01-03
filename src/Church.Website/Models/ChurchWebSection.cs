using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Church.Website.Models
{
    public class ChurchWebSection : ConfigurationSection, IWebConfiguration
    {
        [ConfigurationProperty("defaultBible", IsRequired = true)]
        public DefaultBibleElement DefaultBible
        {
            get { return this["defaultBible"] as DefaultBibleElement; }
            set { this["defaultBible"] = value; }
        }

        [ConfigurationProperty("contentFolder", IsRequired = true)]
        public ContentFolderElement ContentFolder
        {
            get { return this["contentFolder"] as ContentFolderElement; }
            set { this["contentFolder"] = value; }
        }

        [ConfigurationProperty("administrators", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ElementCollection<AdministratorConfigElement>), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ElementCollection<AdministratorConfigElement> Administrators
        {
            get
            {
                return (ElementCollection<AdministratorConfigElement>)base["administrators"];
            }
        }

        [ConfigurationProperty("languages", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ElementCollection<LanguageConfigElement>), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ElementCollection<LanguageConfigElement> Languages
        {
            get
            {
                return (ElementCollection<LanguageConfigElement>)base["languages"];
            }
        }

        [ConfigurationProperty("sermonTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ElementCollection<SermonTypeConfigElement>), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ElementCollection<SermonTypeConfigElement> SermonTypes
        {
            get
            {
                return (ElementCollection<SermonTypeConfigElement>)base["sermonTypes"];
            }
        }

        [ConfigurationProperty("libraryMaterialTypes", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ElementCollection<LibraryMaterialTypeConfigElement>), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public ElementCollection<LibraryMaterialTypeConfigElement> LibraryMaterialTypes
        {
            get
            {
                return (ElementCollection<LibraryMaterialTypeConfigElement>)base["libraryMaterialTypes"];
            }
        }

        [ConfigurationProperty("numberOfIncidentDays", DefaultValue = -15)]
        public int? NumberOfIncidentDays
        {
            get { return this["numberOfIncidentDays"] as int?; }
            set { this["numberOfIncidentDays"] = value; }
        }

        [ConfigurationProperty("churchWebsiteLibrary", IsRequired = true)]
        public string ChurchWebsiteLibrary
        {
            get { return this["churchWebsiteLibrary"] as string; }
            set { this["churchWebsiteLibrary"] = value; }
        }

        [ConfigurationProperty("versionInfo", IsRequired = true)]
        public string VersionInfo
        {
            get { return this["versionInfo"] as string; }
            set { this["versionInfo"] = value; }
        }

        [ConfigurationProperty("bulletinFilename", IsRequired = true)]
        public BulletinFilenameElement BulletinFilename
        {
            get { return this["bulletinFilename"] as BulletinFilenameElement; }
            set { this["bulletinFilename"] = value; }
        }

        [ConfigurationProperty("mail", IsRequired = true)]
        public MailElement Mail
        {
            get { return this["mail"] as MailElement; }
            set { this["mail"] = value; }
        }

        [ConfigurationProperty("separatorsAmongNames", IsRequired = true)]
        public string SeparatorsAmongNames
        {
            get { return this["separatorsAmongNames"] as string; }
            set { this["separatorsAmongNames"] = value; }
        }
    }

    public class ElementCollection<T> : ConfigurationElementCollection, IEnumerable<T> where T : ItemElement, new()
    {
        public ElementCollection()
        {
        }

        //public override ConfigurationElementCollectionType CollectionType
        //{
        //    get
        //    {
        //        return ConfigurationElementCollectionType.AddRemoveClearMap;
        //    }
        //}

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            Debug.Assert(element is T);
            return (element as T).ID;
        }

        public new IEnumerator<T> GetEnumerator()
        {
            IEnumerator iterator = base.GetEnumerator();
            while (iterator.MoveNext())
            {
                Debug.Assert(iterator.Current is T);
                yield return iterator.Current as T;
            }
        }

        public T Default
        {
            get
            {
                Trace.Assert(0 < this.Count);
                return this.SingleOrDefault(item => item.IsDefault) ?? this.First();
            }
        }
    }

    public class ItemElement : ConfigurationElement
    {
        [ConfigurationProperty("id", IsRequired = true, IsKey = true)]
        //[RegexStringValidator(@"")]
        public string ID
        {
            get { return this["id"] as string; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("isDefault")]
        //[RegexStringValidator(@"")]
        public bool IsDefault
        {
            get { return this["isDefault"] as bool? ?? false; }
            set { this["isDefault"] = value; }
        }
    }

    public class AdministratorConfigElement : ItemElement
    {
        [ConfigurationProperty("password")]
        //[RegexStringValidator(@"")]
        public string Password
        {
            get { return this["password"] as string; }
            set { this["password"] = value; }
        }

        [ConfigurationProperty("email", IsRequired = true)]
        //[RegexStringValidator(@"")]
        public string Email
        {
            get { return this["email"] as string; }
            set { this["email"] = value; }
        }
    }

    public class LanguageConfigElement : ItemElement
    {
        [ConfigurationProperty("text", IsRequired = true)]
        public string Text
        {
            get { return this["text"] as string; }
            set { this["text"] = value; }
        }
    }

    public class SermonTypeConfigElement : ItemElement
    {
    }

    public class LibraryMaterialTypeConfigElement : ItemElement
    {
    }

    public class DefaultBibleElement : ConfigurationElement
    {
        [ConfigurationProperty("version", IsRequired = true)]
        public string Version
        {
            get { return this["version"] as string; }
            set { this["version"] = value; }
        }

        [ConfigurationProperty("culture")]
        public string Culture
        {
            get { return this["culture"] as string; }
            set { this["culture"] = value; }
        }
    }

    public class ContentFolderElement : ConfigurationElement
    {
        [ConfigurationProperty("bibleFolder", IsRequired = true)]
        public string BibleFolder
        {
            get { return this["bibleFolder"] as string; }
            set { this["bibleFolder"] = value; }
        }

        [ConfigurationProperty("bulletinFolder", IsRequired = true)]
        public string BulletinFolder
        {
            get { return this["bulletinFolder"] as string; }
            set { this["bulletinFolder"] = value; }
        }

        [ConfigurationProperty("sermonFolder", IsRequired = true)]
        public string SermonFolder
        {
            get { return this["sermonFolder"] as string; }
            set { this["sermonFolder"] = value; }
        }

        [ConfigurationProperty("materialFolder", IsRequired = true)]
        public string MaterialFolder
        {
            get { return this["materialFolder"] as string; }
            set { this["materialFolder"] = value; }
        }
    }

    public class BulletinFilenameElement : ConfigurationElement
    {
        [ConfigurationProperty("prefix", IsRequired = true)]
        public string Prefix
        {
            get { return this["prefix"] as string; }
            set { this["prefix"] = value; }
        }

        [ConfigurationProperty("separator", IsRequired = true)]
        public string Separator
        {
            get { return this["separator"] as string; }
            set { this["separator"] = value; }
        }

        [ConfigurationProperty("dateFormat", IsRequired = true)]
        public string DateFormat
        {
            get { return this["dateFormat"] as string; }
            set { this["dateFormat"] = value; }
        }
    }

    public class MailElement : ConfigurationElement
    {
        [ConfigurationProperty("relayServer", IsRequired = true)]
        public string RelayServer
        {
            get { return this["relayServer"] as string; }
            set { this["relayServer"] = value; }
        }

        [ConfigurationProperty("fromAddress", IsRequired = true)]
        public string FromAddress
        {
            get { return this["fromAddress"] as string; }
            set { this["fromAddress"] = value; }
        }

        [ConfigurationProperty("bccAddress", IsRequired = true)]
        public string BccAddress
        {
            get { return this["bccAddress"] as string; }
            set { this["bccAddress"] = value; }
        }
    }
}
