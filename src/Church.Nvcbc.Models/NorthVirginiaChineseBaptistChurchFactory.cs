namespace Church.Models
{
    using System;
    using System.Globalization;

    public class NorthVirginiaChineseBaptistChurchFactory : IFactory
    {
        private CultureInfo culture;

        private BulletinTextBuilder builder;

        private NorthVirginiaChineseBaptistChurchFactory()
        {
        }

        public static IFactory Create(CultureInfo culture)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(culture, "culture");

            return new NorthVirginiaChineseBaptistChurchFactory
            {
                culture = culture,
                builder = culture.Name == "zh-TW" ?
                     new NorthVirginiaChineseBaptistChurchTwBulletinBuilder(culture) as BulletinTextBuilder :
                     new NorthVirginiaChineseBaptistChurchCnBulletinBuilder(culture) as BulletinTextBuilder,
            };
        }

        public WeeklyBulletin CreateBulletin(DateTime date, string fileUri, string plainText)
        {
            ExceptionUtilities.ThrowArgumentNullExceptionIfEmpty(plainText, "plainText");

            var properties = this.builder.Make<NorthVirginiaChineseBaptistChurchBulletin>(plainText);
            ExceptionUtilities.ThrowFormatExceptionIfFalse(
                properties.Date == date,
                "The date entered ({0}) doesn't match the date on the bulletin ({1}).",
                properties.Date.ToShortDateString(),
                date.ToShortDateString());

            return WeeklyBulletin.Create(date, fileUri, culture, properties);
        }
    }
}
