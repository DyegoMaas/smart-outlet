using SmartOutlet.Outlet.Tests.EventSourcingExamples.Projections;

namespace SmartOutlet.Outlet.Tests.EventSourcingExamples.Events
{
    public class AccountCredited : Transaction
    {
      public override void Apply(Account account) {
          account.Balance += Amount;
      }

      public AccountDebited ToDebit() {
          return new AccountDebited {
              Amount = Amount,
              To = From,
              From = To,
              Description = Description
          };
      }

      public override string ToString() {
          return $"{Time} Credited {Amount.ToString("C")} From {From}";
      }
    }
}