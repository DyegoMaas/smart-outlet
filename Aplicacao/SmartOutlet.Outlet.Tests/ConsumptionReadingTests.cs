using System;
using System.Linq;
using FluentAssertions;
using Marten;
using NUnit.Framework;
using SmartOutlet.Outlet.Consumption;

namespace SmartOutlet.Outlet.Tests
{
    [TestFixture]
    public class ConsumptionReadingTests
    {
        private DocumentStore _documentStore;

        [SetUp]
        public void SetUp()
        {
            _documentStore = DocumentStoreForTests.SetupNew();
        }
        
        [Test]
        public void saving_consumption_readings()
        {
            var readings = new[]
            {
                new ConsumptionReading
                {
                    PlugName = "Pinheiro de natal", 
                    PlugState = PlugState.Off,
                    ConsumoEmWatts = 0,
                    TimeStamp = DateTime.Today
                },
                new ConsumptionReading
                {
                    PlugName = "Pinheiro de natal", 
                    PlugState = PlugState.On,
                    ConsumoEmWatts = 25,
                    TimeStamp = DateTime.Today.AddMinutes(1)
                },
                new ConsumptionReading
                {
                    PlugName = "Pinheiro de natal", 
                    PlugState = PlugState.On,
                    ConsumoEmWatts = 20,
                    TimeStamp = DateTime.Today.AddMinutes(2)
                },
                new ConsumptionReading
                {
                    PlugName = "Pinheiro de natal", 
                    PlugState = PlugState.On,
                    ConsumoEmWatts = 20,
                    TimeStamp = DateTime.Today.AddMinutes(3)
                }
            };
            using (var session = _documentStore.LightweightSession())
            {
                foreach (var reading in readings)
                {
                    session.Store(reading);
                }
                session.SaveChanges();
            }
            
            using (var session = _documentStore.LightweightSession())
            {
                var allReadings = session
                    .Query<ConsumptionReading>()
                    .ToArray();
                allReadings.Should().BeEquivalentTo(readings);
            }
        }
    }
}