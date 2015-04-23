namespace Julep.Tests

open NUnit.Framework
open FsUnit
open Julep.Types
open System


[<TestFixture>]
type ``gathers income projections from transactions`` ()=
  let categories = [|
    { 
      TransactionCategory.Name = "Paycheck"
      ValidRange = { From = Month 3 ; To = Some (Month 8) }
    }
    { 
      TransactionCategory.Name = "Renter"
      ValidRange = { From = Month 3 ; To = Some (Month 8) }
    }
  |]
  let transactions = [
    { 
      Amount = 5.0m<dollars> 
      Month = Month 5
      Date = DateTime.Now
      Category = categories.[0]
      Account = Account "LMCU"
      Description = "Money"
    }
  ]

  [<Test>] 
  member x.``do stuff`` ()= 
    5 + 3 |> should equal 8

  [<Test>] 
  member x.``when i add 6`` ()= 
    5 + 6 |> should equal 12

