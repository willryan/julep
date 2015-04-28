namespace Julep.Tests

open NUnit.Framework
open FsUnit
open Julep.Types
open Julep.AggregateTypesBuilder
open System


[<TestFixture>]
type ``gathers income projections from transactions`` ()=
  let range min max =
    { From = Month min ; To = Option.map Month max }

  let tCat name rng =
    {
      TransactionCategory.Name = name
      ValidRange = rng
    }

  let iCat name rng =
    {
      IncomeCategory.Name = name
      ValidRange = rng
    }

  let txHelper month amount cat =
    {
      Amount = amount
      Month = month
      Date = DateTime.Now
      Category = cat
      Account = Account "LMCU"
      Description = "Money"
    }

  let transCategories = [|
    tCat "Paycheck" <| range 3 (Some 8)
    tCat "Renter" <| range 3 None
    tCat "Renter" <| range 1 (Some 2)
  |]

  let incomeCategories = [|
    iCat "Paycheck" <| range 3 (Some 8)
    iCat "Renter" <| range 3 None
    iCat "Old" <| range 1 (Some 5)
  |]

  let findTxCat cat =
    Array.find (fun (c:TransactionCategory) -> c.Name = cat) transCategories

  let findIncCat cat =
    Array.find (fun (c:IncomeCategory) -> c.Name = cat) incomeCategories

  let tx month amount cat =
    transCategories
    |> Array.find (fun c -> c.Name = cat)
    |> txHelper month amount

  let transactions month txList =
    let txMonth = tx month
    (
      month,
      txList
        |> List.map (fun (amt,cat) -> txMonth amt cat)
    )

  let iSrc incCat txCat rng =
    {
      IncomeCategory = findIncCat incCat
      TransactionCategory = findTxCat txCat
      ValidRange = rng
    }


  [<Test>]
  member x.``it returns income projections for a month of transactions`` ()=
    let sources = [
      iSrc "Paycheck" "AO" <| range 3 (Some 8)
      iSrc "Renter" "Jeff" <| range 3 (Some 8)
    ]
    let txs =
      [
        (5.0m<dollars>, "AO")
      ]
      |> transactions (Month 5)

    getIncomeProjections txs sources
    |> should equal [
      {
        Category = incomeCategories.[0]
        Projected = 5.0m<dollars>
        Month = Month 5
      }
    ]

  [<Test>]
  member x.``excludes sources not valid for that month`` ()=
    let sources = [
      iSrc "Paycheck" "AO" <| range 3 (Some 8)
      iSrc "Renter" "Jeff" <| range 3 (Some 8)
    ]
    let txs =
      [
        (5.0m<dollars>, "AO")
      ]
      |> transactions (Month 5)
    getIncomeProjections txs sources
    |> should equal [
      {
        Category = incomeCategories.[0]
        Projected = 5.0m<dollars>
        Month = Month 5
      }
    ]

