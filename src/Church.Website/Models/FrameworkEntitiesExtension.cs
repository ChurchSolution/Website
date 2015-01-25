namespace Church.Website.Models
{
    using Church.Model;
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;

    public static class FrameworkEntitiesExtension
    {
        public static async Task<WeeklyBulletin> GetDefaultBulletinAsync(this FrameworkEntities entities, string cultureName)
        {
            var bulletin = await entities.Bulletins.OrderByDescending(b => b.Date).FirstOrDefaultAsync(b => cultureName.Equals(b.Culture, StringComparison.Ordinal)) ??
                await entities.Bulletins.OrderByDescending(b => b.Date).FirstOrDefaultAsync();

            return FrameworkEntitiesExtension.GetBulletin(bulletin);
        }

        public static async Task<WeeklyBulletin> GetBulletinByDateAsync(this FrameworkEntities entities, DateTime date, string cultureName)
        {
            var bulletin = await entities.Bulletins.FirstOrDefaultAsync(b => b.Date == date && cultureName.Equals(b.Culture, StringComparison.Ordinal));

            return FrameworkEntitiesExtension.GetBulletin(bulletin);
        }

        private static WeeklyBulletin GetBulletin(Bulletin bulletin)
        {
            if (null == bulletin)
            {
                return null;
            }

            var factory = Utilities.CreateFactory(bulletin.Culture);
            var weeklyBulletin = factory.CreateBulletin(bulletin.Date, bulletin.FileUrl, bulletin.PlainText);

            return weeklyBulletin;
        }
    }
}
