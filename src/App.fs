module Julep.App

open System

open Suave
open Suave.Web
open Suave.Json
open Suave.Http
open Suave.Http.Applicatives
open Suave.Http.Files
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Types
open Suave.Log
open System.IO
open System.Text
open Nessos.FsPickler
open Nessos.FsPickler.Json

open Julep.Types
open Julep.Routing

  //let logger = Loggers.sane_defaults_for Debug

let tx =
  {
    Month = Month 5
    Date = DateTime.Now
    Category =
      {
        Name = "Mortgage"
        ValidRange = { From = Month 2 ; To = Some (Month 10) }
      }
    Amount = 1310m<dollars>
    Account = Account "LMCU"
    Description = "monthly mortgage payment"
  }

let pickler = FsPickler.CreateJson(indent = true, omitHeader = true)

let okJson o =
  OK (pickler.PickleToString o) >>= Writers.setMimeType "application/json"

let appOld =
  choose
    [ GET >>= choose
        [ path "/transaction" >>= okJson tx
          path "/goodbye" >>= OK "Good bye GET" ]
      POST >>= choose
        [ path "/hello" >>= OK "Hello POST"
          path "/goodbye" >>= OK "Good bye POST" ] ]


let handler resName actName ids = 
  OK (sprintf "OK: %s#%s %A" resName actName ids)

let testAct resName =
  let testHndl act = Some (handler resName act)
  {
    Show = None //testHndl "Show"
    Index = testHndl "Index"
    Create = testHndl "Create"
    Update = testHndl "Update"
    Destroy = testHndl "Destroy"
  }

let goalsH = testAct "Goals"
let transH = testAct "Transactions"
let microH = testAct "Micros"

let resources =
  [
    resource "goals" goalsH [
      resource "transactions" transH [
        resource "micros" microH []
      ]
    ]
  ]

let defaultArgs = [| "server" |]

let startApp () = 
  let app = makeApp()
  startWebServer defaultConfig app
  0

let start (args : string[]) =
  let realArgs = if (args.Length = 0) then defaultArgs else args

  Resources <- resources

  match args.[0] with
  | "routes" -> printRouteDefs() ; 0
  | "server" -> startApp()
  | _ -> printfn "Unrecognized argument %s" args.[0] ; 1

