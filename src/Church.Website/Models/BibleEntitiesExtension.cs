namespace Church.Website.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public static class BibleEntitiesExtension
    {
        public static async Task<Bible> GetDefaultBibleAsync(this BibleEntities entities, string cultureName)
        {
            return await entities.Bibles.FirstOrDefaultAsync(b => cultureName.Equals(b.Culture, StringComparison.Ordinal) && b.IsDefault) ??
                await entities.Bibles.OrderBy(b => b.Culture).FirstAsync(b => b.IsDefault);
        }

        public static Task<Bible> GetBibleAsync(this BibleEntities entities, Guid id)
        {
            return entities.Bibles.FirstAsync(b => b.Id == id);
        }
    }
}