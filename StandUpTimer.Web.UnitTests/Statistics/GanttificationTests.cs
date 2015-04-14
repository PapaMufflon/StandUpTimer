using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Common;
using StandUpTimer.Web.Models;
using StandUpTimer.Web.Statistics;

namespace StandUpTimer.Web.UnitTests.Statistics
{
    [TestFixture]
    public class GanttificationTests
    {
        [Test]
        public void Two_statuses_will_become_one_gantt_item()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 1));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "14.04.2015",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 1, 20, 0, 0, 0",
                    EndDate = "2015, 4, 1, 21, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void The_start_and_end_date_is_always_today()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 1));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "14.04.2015",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 1, 20, 0, 0, 0",
                    EndDate = "2015, 4, 1, 21, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void When_a_status_is_really_today_Name_it_so()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 1));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 1, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 1, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 1, 20, 0, 0, 0",
                    EndDate = "2015, 4, 1, 21, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void When_a_status_is_from_yesterday_Name_it_so()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 2));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 1, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 1, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Gestern",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 2, 20, 0, 0, 0",
                    EndDate = "2015, 4, 2, 21, 0, 0, 0"
                }
            }));
        }

        // status over 2 days
    }
}