namespace Church.Models.EntityFramework
{
    using System;
    using System.Threading.Tasks;

    public static class FrameworkEntitiesExtensions
    {
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
