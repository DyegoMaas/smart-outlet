using System;
using FluentAssertions;
using NUnit.Framework;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.Tests.Unit
{
    public class TimeLineTest
    {
        private TimeLine _timeLine;

        [SetUp]
        public void SetUp()
        {
            _timeLine = new TimeLine();
        }
        
        [Test]
        public void describing_a_plug_turning_on()
        {
            var time = new DateTime(2017, 10, 10) + 17.Hours(10.Minutes(5.Seconds()));
            var plugTurnedOn = new PlugTurnedOn(time);

            _timeLine.Apply(plugTurnedOn);

            var events = _timeLine.GetEvents();
            events.Should().Contain("2017/10/10 17:10:05 - Plugue ligado");
        }
        
        [Test]
        public void describing_a_plug_turning_off()
        {
            var time = new DateTime(2017, 10, 10) + 17.Hours(10.Minutes(5.Seconds()));
            var plugTurnedOff = new PlugTurnedOff(time);

            _timeLine.Apply(plugTurnedOff);

            var events = _timeLine.GetEvents();
            events.Should().Contain("2017/10/10 17:10:05 - Plugue desligado");
        }
        
        [Test]
        public void describing_a_plug_renamed()
        {
            var time = new DateTime(2017, 10, 10) + 17.Hours(10.Minutes(5.Seconds()));
            var plugRenamed = new PlugRenamed("Lala");

            _timeLine.Apply(plugRenamed);

            var events = _timeLine.GetEvents();
            events.Should().Contain(x => x.EndsWith("Plugue renomeado para Lala"));
        }
    }
}