using System;

namespace Church.Website.Models
{
    public interface IWebConfiguration
    {
        DefaultBibleElement DefaultBible { get; set; }
        ContentFolderElement ContentFolder { get; set; }
        ElementCollection<AdministratorConfigElement> Administrators { get; }
        ElementCollection<LanguageConfigElement> Languages { get; }
        BulletinFilenameElement BulletinFilename { get; set; }
        ElementCollection<LibraryMaterialTypeConfigElement> LibraryMaterialTypes { get; }
        MailElement Mail { get; set; }
        int? NumberOfIncidentDays { get; set; }
        string SeparatorsAmongNames { get; set; }
        ElementCollection<SermonTypeConfigElement> SermonTypes { get; }
        string ChurchWebsiteLibrary { get; set; }
        string VersionInfo { get; set; }
    }
}
