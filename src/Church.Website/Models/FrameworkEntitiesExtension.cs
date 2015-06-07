namespace Church.Website.Models
{
    using Church.Models;
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

        public static Task LogInfoAsync(this FrameworkEntities entities, string ipAddress, string username, string content)
        {
            const string EventType = "Information";

            var value = entities.LogAsync(ipAddress, EventType, username, content);

            return Task.FromResult(value);
        }

        public static Task LogExceptionAsync(this FrameworkEntities entities, string ipAddress, string username, Exception exception)
        {
            const string EventType = "Exception";

            var discription = string.Format("Message: {0}{1}Stack trae: {1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
            var value = entities.LogAsync(ipAddress, EventType, username, discription);

            return Task.FromResult(value);
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

        private static Task<int> LogAsync(this FrameworkEntities entities, string ipAddress, string type, string username, string discription)
        {
            var row = new Event
            {
                Id = Guid.NewGuid(),
                Time = DateTime.Now,
                IPAddress = ipAddress,
                Type = type,
                Username = username,
                Description = discription,
            };

            entities.Events.Add(row);
            return entities.SaveChangesAsync();
        }
    }
}
