using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Church.Website.Models
{
    public static class BibleEntitiesExtension
    {
        public static Bible GetDefaultBible(this BibleEntities entities, string culture)
        {
            return entities.Bibles.FirstOrDefault(b => b.Culture == culture && b.IsDefault) ??
                entities.Bibles.OrderBy(b => b.Culture).First(b => b.IsDefault);
        }

        public static Bible GetBible(this BibleEntities entities, Guid id)
        {
            return entities.Bibles.First(b => b.Id == id);
        }
    }
}