using System;
using FluentAssertions;
using Marten;
using NUnit.Framework;
using SmartOutlet.Outlet.EventSourcing;
using SmartOutlet.Outlet.EventSourcing.Events;

namespace SmartOutlet.Outlet.Tests.Integration
{
    public class ConsumptionReadingEventStore
    {
        private DocumentStore _documentStore;

        [SetUp]
        public void SetUp()
        {
            _documentStore = DocumentStoreForTests.NewEventSource<PlugHistory>(
                typeof(PlugActivated),
                typeof(PlugTurnedOn),
                typeof(PlugTurnedOff),
                typeof(ConsumptionReadingReceived)
            );
        }

        [Test]
        public void saving_consumption_readings_in_event_store()
        {
            var pinheiro = new PlugActivated(Guid.NewGuid(), "Pinheiro de Natal");
            var tv = new PlugActivated(Guid.NewGuid(), "TV Set");

            // activating two plugs
            using (var session = _documentStore.OpenSession())
            {
                session.Events.Append(pinheiro.PlugId, pinheiro);
                session.Events.Append(tv.PlugId, tv);

                session.SaveChanges();
            }
            
            using (var session = _documentStore.OpenSession())
            {
                session.Events.Append(pinheiro.PlugId, 
                    new PlugTurnedOn(),
                    new ConsumptionReadingReceived(20),
                    new ConsumptionReadingReceived(20),
                    new ConsumptionReadingReceived(22),
                    new ConsumptionReadingReceived(23),
                    new ConsumptionReadingReceived(24),
                    new ConsumptionReadingReceived(20),
                    new ConsumptionReadingReceived(21),
                    new ConsumptionReadingReceived(22),
                    new ConsumptionReadingReceived(20),
                    new ConsumptionReadingReceived(21),
                    new PlugTurnedOff(), 
                    new PlugTurnedOn(),
                    new ConsumptionReadingReceived(21)
                );
                session.Events.Append(tv.PlugId, new PlugTurnedOn(), new PlugTurnedOff());

                session.SaveChanges();
            }
            
            using (var session = _documentStore.LightweightSession())
            {
                var plugPinheiro = session.Load<PlugHistory>(pinheiro.PlugId);
                var plugTv = session.Load<PlugHistory>(tv.PlugId);

                plugPinheiro.IsOn().Should().BeTrue();
                plugTv.IsOn().Should().BeFalse();
            }
        }
       
        
        
    }
}