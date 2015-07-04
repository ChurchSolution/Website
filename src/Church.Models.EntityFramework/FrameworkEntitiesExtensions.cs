namespace Church.Models.EntityFramework
{
    using System;
    using System.Threading.Tasks;

    public static class FrameworkEntitiesExtensions
    {
        /// <summary>
        /// Converts to Sermon model class.
        /// </summary>
        /// <param name="sermon">The sermon.</param>
        /// <returns>The <see cref="Models.Sermon"/>.</returns>
        public static Models.Sermon ToModel(this Sermon sermon)
        {
            return new Models.Sermon
                       {
                           Id = sermon.Id,
                           Type = sermon.Type,
                           Date = sermon.Date,
                           Speaker = sermon.Speaker,
                           Title = sermon.Title,
                           FileUrl = sermon.FileUrl
                       };
        }

        /// <summary>
        /// Converts to Material model class.
        /// </summary>
        /// <param name="material">The material.</param>
        /// <returns>The <see cref="Models.Material"/>.</returns>
        public static Models.Material ToModel(this Material material)
        {
            return new Models.Material
            {
                Id = material.Id,
                Type = material.Type,
                Date = material.Date,
                Authors = material.Authors,
                Title = material.Title,
                FileUrl = material.FileUrl
            };
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
