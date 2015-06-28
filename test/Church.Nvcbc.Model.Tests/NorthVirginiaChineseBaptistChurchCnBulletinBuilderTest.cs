namespace Church.Nvcbc.Model.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Church.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NorthVirginiaChineseBaptistChurchCnBulletinBuilderTest
    {
        private NorthVirginiaChineseBaptistChurchCnBulletinBuilder builder;

        [ClassInitialize]
        public static void TestsSetup(TestContext context)
        {
        }

        [TestInitialize]
        public void TestInit()
        {
            this.builder = new NorthVirginiaChineseBaptistChurchCnBulletinBuilder();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfInputLineEmpty()
        {
            // Arrange
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();
            var lines = Enumerable.Empty<string>();

            // Action
            privateObject.Invoke("ProcessWordFromPastor", new object[] { bulletin, lines });

            // Assert
            // Do nothing
        }

        [TestMethod]
        public void ShouldProcessWordFromPastorSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "牧者的话  WORD FROM  PASTOR - 金春叶牧师",
                "",
                "在上帝面前我们都是不完美的半成品。正因为我们不够成熟才使我们蒙召聚集在一起。",
                "所以当提起教会的时候主耶稣说“康健的人用不着医生，有病的人才用得着” （太9：12）。",
                " ",
                "",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessWordFromPastor", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            Assert.IsTrue(Lines[0].Contains(bulletin.PastorName));
            Assert.AreEqual(Lines[2], bulletin.WordFromPastor.First());
            Assert.AreEqual(Lines[3], bulletin.WordFromPastor.Skip(1).First());
        }

        [TestMethod]
        public void ShouldProcessLastWeekDataSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "上周主日出席人数：	成人：68	儿童：",
                "上周周间活动人次：	成人：	儿童：38",
                "上周总奉献：$10	一般：$1	建堂：2",
                "    爱心：$3	彼岸基金：$4	震灾：",
                "",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessLastWeekData", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            var lastWeekData = bulletin.LastWeekData;
            Assert.AreEqual(68, lastWeekData["主日成人"]);
            Assert.AreEqual(0, lastWeekData["主日儿童"]);
            Assert.AreEqual(0, lastWeekData["周间成人"]);
            Assert.AreEqual(38, lastWeekData["周间儿童"]);
            Assert.AreEqual(10, lastWeekData["奉献总计"]);
            Assert.AreEqual(1, lastWeekData["一般奉献"]);
            Assert.AreEqual(2, lastWeekData["建堂奉献"]);
            Assert.AreEqual(3, lastWeekData["爱心奉献"]);
            Assert.AreEqual(4, lastWeekData["彼岸基金"]);
        }

        [TestMethod]
        public void ShouldProcessServicesOfNextWeekSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "下周主日崇拜事奉分工",
                @"会司
TBD
清点奉献
谢靖, 霍静
领歌

场地
郑金平、王忠诚、马念
 学龄儿童崇拜
Sharon, 谢靖



",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessServicesOfNextWeek", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            var services = bulletin.Services;
            Assert.AreEqual(5, services.Count());
            Assert.AreEqual(1, services["会司"].PeopleForNext.Count());
            Assert.AreEqual(2, services["清点奉献"].PeopleForNext.Count());
            Assert.AreEqual(1, services["领歌"].PeopleForNext.Count());
            Assert.AreEqual(3, services["场地"].PeopleForNext.Count());
            Assert.AreEqual(2, services["学龄儿童崇拜"].PeopleForNext.Count());
        }

        [TestMethod]
        public void ShouldProcessHeaderSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "北  维  州  华  人  浸  信  会",
                "NORTHERN  VIRGINIA  CHINESE  BAPTIST CHURCH",
                "Oak View Elementary School, 5004 Sideburn Rd, Fairfax, VA 22032",
                " 电话703-451-0815   牧师手机703-474-7076       http://www.nvcbc.org",
                "讲员：金春叶牧师      	  	    司琴：Daniel Song",
                "",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessHeader", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            Assert.AreEqual("金春叶牧师", bulletin.Speaker);
            var services = bulletin.Services;
            Assert.AreEqual(1, services.Count());
            Assert.AreEqual(1, services["司琴"].People.Count());
        }

        [TestMethod]
        public void ShouldProcessWorshipProgramSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "主 日 崇 拜",
                "4/1/2012  上午10:45时",
                "",
                "赞美诗一	和散那	Praise#1",
                "☆序乐	625、主在圣殿中	Prelude",
                "☆ 祷告		Prayer",
                "",
                "* 有☆符号的地方请大家站立",
                "",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessWorshipProgram", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            Assert.AreEqual(new DateTime(2012, 4, 1), bulletin.Date);
            Assert.AreEqual(3, bulletin.WorshipProgram.Count());
            var firstProgram = bulletin.WorshipProgram.First();
            Assert.AreEqual("赞美诗一", firstProgram.Name);
            Assert.AreEqual("和散那", firstProgram.Value);
            Assert.AreEqual("Praise#1", firstProgram.NameInEnglish);
            var secondProgram = bulletin.WorshipProgram.Skip(1).First();
            Assert.AreEqual("序乐", secondProgram.Name);
            Assert.AreEqual("625、主在圣殿中", secondProgram.Value);
            Assert.AreEqual("Prelude", secondProgram.NameInEnglish);
            var lastProgram = bulletin.WorshipProgram.Last();
            Assert.AreEqual("祷告", lastProgram.Name);
            Assert.AreEqual(string.Empty, lastProgram.Value);
            Assert.AreEqual("Prayer", lastProgram.NameInEnglish);
        }

        [TestMethod]
        public void ShouldProcessServicesSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "主日崇拜事奉分工",
                @"招待
高建生, 邓宇彪
场地
黄永星、王忠诚、马念
学龄儿童崇拜
Sharon, 谢靖
领歌


",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessServices", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            var services = bulletin.Services;
            Assert.AreEqual(4, services.Count());
            Assert.AreEqual(2, services["招待"].People.Count());
            Assert.AreEqual(3, services["场地"].People.Count());
            Assert.AreEqual(2, services["学龄儿童崇拜"].People.Count());
            Assert.AreEqual(1, services["领歌"].People.Count());
        }

        [TestMethod]
        public void ShouldProcessFamilyWorshipSuccessfully()
        {
            // Arrange
            var Lines = new[]
            {
                "本周家庭崇拜诗歌经文",
                "诗歌: 《直到主耶穌再來時候》，经文：约16：33",
                "网上播放:	 http://www.youtube.com/watch?v=UGXsz8RDp9g&feature=related",
                "",
                "",
            };
            var privateObject = new PrivateObject(this.builder);
            var bulletin = new NorthVirginiaChineseBaptistChurchBulletin();

            // Action
            var res = privateObject.Invoke("ProcessFamilyWorship", new object[] { bulletin, Lines }) as IEnumerable<string>;

            // Assert
            Assert.AreEqual(0, res.Count());
            var familyWorship = bulletin.FamilyWorship;
            Assert.AreEqual("直到主耶穌再來時候", familyWorship.PraiseName);
            Assert.AreEqual("约16：33", familyWorship.Verse);
            Assert.AreEqual("http://www.youtube.com/watch?v=UGXsz8RDp9g&feature=related", familyWorship.PraiseUri);
        }

        [TestMethod]
        public void ShouldProcessBulletinSuccessfully()
        {
            // Arrange
            string plainText;
            using (var sr = new StreamReader(@"C:\Workspaces\Data\NVCBC\20120401_bulletin.txt"))
            {
                plainText = sr.ReadToEnd();
            }

            // Action
            var properties = this.builder.Make<NorthVirginiaChineseBaptistChurchBulletin>(plainText);

            // Assert
            Assert.IsTrue(properties is NorthVirginiaChineseBaptistChurchBulletin);
            var bulletin = properties as NorthVirginiaChineseBaptistChurchBulletin;
            Assert.AreEqual(new DateTime(2012, 4, 1), bulletin.Date);
        }
    }
}
