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



let goalsH = { Show = Some (fun _ -> OK "OK") ; Index = None ; Create = None ; Update = None ; Destroy = None }
let transH = { Show = Some (fun _ -> OK "OK") ; Index = None ; Create = None ; Update = None ; Destroy = None }

let resources = 
  [
    resource "goals" goalsH [
      resource "transactions" transH []
    ] 
  ]

let start () =
  Resources <- resources
  printRouteDefs()
  let app = makeApp()
  startWebServer defaultConfig app

