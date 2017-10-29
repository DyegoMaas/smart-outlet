//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Account.Events;
//using Accounting.Events;
//using Marten;
//using NUnit.Framework;
//using SmartOutlet.Outlet.Consumption;
//
//namespace SmartOutlet.Outlet.Tests
//{
//    public class ConsumptionReadingEventStore
//    {
//        private DocumentStore _documentStore;
//
//        [SetUp]
//        public void SetUp()
//        {
////            _documentStore = DocumentStoreForTests.NewEventSource<Accounting.Account>(
////                typeof(AccountCreated),
////                typeof(AccountCredited),
////                typeof(AccountDebited)
////            );
//        }
//
//        [Test]
//        public void saving_consumption_readings_in_event_store()
//        {
//            
//        }
//
//        [Test]
//        public void saving_consumption_readings_in_event_store2()
//        {
////            var readings = new[]
////            {
////                new ConsumptionReading
////                {
////                    PlugName = "Pinheiro de natal", 
////                    PlugState = PlugState.Off,
////                    ConsumoEmWatts = 0,
////                    TimeStamp = DateTime.Today
////                },
////                new ConsumptionReading
////                {
////                    PlugName = "Pinheiro de natal", 
////                    PlugState = PlugState.On,
////                    ConsumoEmWatts = 25,
////                    TimeStamp = DateTime.Today.AddMinutes(1)
////                },
////                new ConsumptionReading
////                {
////                    PlugName = "Pinheiro de natal", 
////                    PlugState = PlugState.On,
////                    ConsumoEmWatts = 20,
////                    TimeStamp = DateTime.Today.AddMinutes(2)
////                },
////                new ConsumptionReading
////                {
////                    PlugName = "Pinheiro de natal", 
////                    PlugState = PlugState.On,
////                    ConsumoEmWatts = 20,
////                    TimeStamp = DateTime.Today.AddMinutes(3)
////                }
////            };
////            
////            using (var session = _documentStore.OpenSession())
////            {
////                foreach (var reading in readings)
////                {
////                    session.Store(reading);
////                }
////                session.SaveChanges();
////            }
//            
//
//            var khalid = new AccountCreated
//            {
//                Owner = "Khalid Abuhakmeh",
//                AccountId = Guid.NewGuid(),
//                StartingBalance = 1000m
//            };
//
//            var bill = new AccountCreated
//            {
//                Owner = "Bill Boga",
//                AccountId = Guid.NewGuid()
//            };
//
//            using (var session = _documentStore.OpenSession())
//            {
//                // create banking accounts
//                session.Events.Append(khalid.AccountId, khalid);
//                session.Events.Append(bill.AccountId, bill);
//
//                session.SaveChanges();
//            }
//
//            using (var session = _documentStore.OpenSession())
//            {
//                // load khalid's account
//                var account = session.Load<Accounting.Account>(khalid.AccountId);
//                // let's be generous
//                var amount = 100m;
//                var give = new AccountDebited
//                {
//                    Amount = amount,
//                    To = bill.AccountId,
//                    From = khalid.AccountId,
//                    Description = "Bill helped me out with some code."
//                };
//
//                if (account.HasSufficientFunds(give))
//                {
//                    session.Events.Append(give.From, give);
//                    session.Events.Append(give.To, give.ToCredit());
//                }
//                // commit these changes
//                session.SaveChanges();
//            }
//
//            using (var session = _documentStore.OpenSession())
//            {
//                // load bill's account
//                var account = session.Load<Accounting.Account>(bill.AccountId);
//                // let's try to over spend
//                var amount = 1000m;
//                var spend = new AccountDebited
//                {
//                    Amount = amount,
//                    From = bill.AccountId,
//                    To = khalid.AccountId,
//                    Description = "Trying to buy that Ferrari"
//                };
//
//                if (account.HasSufficientFunds(spend))
//                {
//                    // should not get here
//                    session.Events.Append(spend.From, spend);
//                    session.Events.Append(spend.To, spend.ToCredit());
//                } else {
//                   session.Events.Append(account.Id, new InvalidOperationAttempted {
//                        Description = "Overdraft" 
//                    }); 
//                }
//                // commit these changes
//                session.SaveChanges();
//            }
//
//            using (var session = _documentStore.OpenSession())
//            {
//                var accounts = session.LoadMany<Accounting.Account>(khalid.AccountId, bill.AccountId);
//
//                foreach (var account in accounts)
//                {
//                    Console.WriteLine(account);
//                }
//            }
//
//            using (var session = _documentStore.LightweightSession())
//            {
//                foreach (var account in new[] { khalid, bill })
//                {
//                    Console.WriteLine();
//                    Console.WriteLine($"Transaction ledger for {account.Owner}");
//                    var stream = session.Events.FetchStream(account.AccountId);
//                    foreach (var item in stream)
//                    {
//                        Console.WriteLine(item.Data);
//                    }
//                    Console.WriteLine();
//                }
//            }
//        }
//    }
//
//    public class PlugData
//    {
//    }
//}