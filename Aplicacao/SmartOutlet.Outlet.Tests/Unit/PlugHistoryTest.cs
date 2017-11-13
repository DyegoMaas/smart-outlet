using System;
using FluentAssertions;
using NUnit.Framework;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.Tests.Unit
{
    public class PlugHistoryTest
    {
        private PlugHistory _plugHistory;

        [SetUp]
        public void SetUp()
        {
            _plugHistory = new PlugHistory();
        }
        
        [Test]
        public void activating_a_plug()
        {
            var plugActivated = new PlugActivated(Guid.NewGuid(), "WONDROUS");
            
            _plugHistory.Apply(plugActivated);

            _plugHistory.Id.Should().Be(plugActivated.PlugId);
            _plugHistory.Name.Should().Be(plugActivated.PlugName);
        }

        [Test]
        public void turning_a_plug_on()
        {
            _plugHistory.Apply(new PlugTurnedOn());
            _plugHistory.CurrentState.Should().Be(PlugState.On);
        }
        
        [Test]
        public void turning_a_plug_off()
        {
            _plugHistory.Apply(new PlugTurnedOff());
            _plugHistory.CurrentState.Should().Be(PlugState.Off);
        }

        [Test]
        public void reading_consumption()
        {
            var consumptionReadingReceived = new ConsumptionReadingReceived(20.5d);
            
            _plugHistory.Apply(consumptionReadingReceived);

            _plugHistory.LastConsumptionInWatts.Should().Be(consumptionReadingReceived.ConsumptionInWatts);
        }

        [Test]
        public void renaming_a_plug()
        {
            var plugRenamed = new PlugRenamed(newName: "ELECTRIC SHEEP");
            
            _plugHistory.Apply(plugRenamed);

            _plugHistory.Name.Should().Be(plugRenamed.NewName);
        }
    }
}