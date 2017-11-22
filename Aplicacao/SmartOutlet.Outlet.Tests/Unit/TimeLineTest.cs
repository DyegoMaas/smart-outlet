using System;
using System.Linq;
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
        public void describing_a_plug_activation()
        {
            var activation = new PlugActivated(plugId:Guid.NewGuid(), plugName:"Sanduicheira");

            _timeLine.Apply(activation);

            var events = _timeLine.GetEvents();
            events.Should().Contain(x => x.EndsWith("Plugue ativado"));
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
        
        [TestCase(CommandType.TurnOn, "Ligar")]
        [TestCase(CommandType.TurnOff, "Desligar")]
        public void describing_an_operation_scheduled(CommandType type, string descricaoEsperadaComando)
        {
            var time = new DateTime(2017, 10, 10) + 17.Hours(10.Minutes(5.Seconds()));
            var scheduling = new OperationScheduled(type, time, timeInFuture:15.Minutes());

            _timeLine.Apply(scheduling);

            var events = _timeLine.GetEvents();
            events.First().Should().EndWith($"{descricaoEsperadaComando} em 900s (2017/10/10 17:25:05)");
        }
    }
}