using System;
using System.Collections.Generic;
using FakeItEasy;
using NUnit.Framework;
using StandUpTimer.Common;
using StandUpTimer.Web.Models;
using StandUpTimer.Web.Statistic;

namespace StandUpTimer.Web.UnitTests.Statistic
{
    [TestFixture]
    public class GanttificationTests
    {
        [Test]
        public void No_status_to_process_returns_an_empty_list()
        {
            var target = new List<Status>();

            var actual = target.Ganttisize();

            Assert.That(actual, Is.Empty);
        }

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

        [Test]
        public void Two_consecutive_inactive_states_will_be_ignored()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 1));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 22, 0, 0)}
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
        public void A_non_inactive_state_at_the_end_produces_a_gantt_item_that_lasts_until_now()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 23, 45, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Sitting, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 0, 0, 0"
                },
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Sitting,
                    StartDate = "2015, 4, 14, 21, 0, 0, 0",
                    EndDate = "2015, 4, 14, 23, 45, 0, 0"
                }
            }));
        }

        [Test]
        public void A_status_lasting_more_than_24_hours_gets_ignored()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 23, 45, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 10, 20, 0, 0)},
                new Status {DeskState = DeskState.Sitting, DateTime = new DateTime(2015, 4, 10, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "10.04.2015",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void A_sitting_state_after_an_inactive_state_produces_just_one_state_starting_at_the_sitting_state()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 21, 21, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}

            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 21, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 21, 0, 0"
                }
            }));
        }

        [Test]
        public void A_status_over_midnight_gets_divided_into_two_statuses()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 13, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 1, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Gestern",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 23, 59, 59, 0"
                },
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 0, 0, 0, 0",
                    EndDate = "2015, 4, 14, 1, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void Inactive_before_midnight_produces_no_more_statuses_on_that_day()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 22, 0, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 13, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 13, 21, 0, 0)},
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Gestern",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 0, 0, 0"
                },
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 22, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void Two_statuses_with_the_same_state_produce_two_items()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 22, 0, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 0, 0, 0"
                },
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 21, 0, 0, 0",
                    EndDate = "2015, 4, 14, 22, 0, 0, 0"
                }
            }));
        }

        [Test]
        public void Unordered_items_are_acceptable()
        {
            TestableDateTime.DateTime = A.Fake<IDateTime>();
            A.CallTo(() => TestableDateTime.DateTime.Today).Returns(new DateTime(2015, 4, 14));
            A.CallTo(() => TestableDateTime.DateTime.Now).Returns(new DateTime(2015, 4, 14, 22, 0, 0));

            var target = new List<Status>
            {
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 13, 10, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 12, 20, 0, 0)},
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 12, 10, 0, 0)},
                new Status {DeskState = DeskState.Standing, DateTime = new DateTime(2015, 4, 14, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 13, 20, 0, 0)},
                new Status {DeskState = DeskState.Inactive, DateTime = new DateTime(2015, 4, 14, 21, 0, 0)}
            };

            var actual = target.Ganttisize();

            Assert.That(actual, Is.EquivalentTo(new List<GanttStatus>
            {
                new GanttStatus
                {
                    Day = "12.04.2015",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 10, 0, 0, 0",
                    EndDate = "2015, 4, 14, 20, 0, 0, 0"
                },
                new GanttStatus
                {
                    Day = "Gestern",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 10, 0, 0, 0",
                    EndDate = "2015, 4, 14, 20, 0, 0, 0"
                },
                new GanttStatus
                {
                    Day = "Heute",
                    DeskState = DeskState.Standing,
                    StartDate = "2015, 4, 14, 20, 0, 0, 0",
                    EndDate = "2015, 4, 14, 21, 0, 0, 0"
                }
            }));
        }
    }
}