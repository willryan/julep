module Julep.DbTest

(*
open System
open System.ComponentModel
open System.Data
open System.Data.Linq
open Microsoft.FSharp.Data.TypeProviders
open Microsoft.FSharp.Linq

type dbSchema = SqlDataConnection<"Data Source=/Users/krelian18/julep.sqlite">
let db = dbSchema.GetDataContext()
let transactions = db.Transactions
let goals = db.Goals
let query1 =
        query {
            for row in transactions do
            select row
        }
query1 |> Seq.iter (fun row -> printfn "%A %s %A" row.Amount row.Description row.Category)
*)
