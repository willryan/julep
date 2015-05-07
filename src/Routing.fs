module Julep.Routing

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

type RouteId = {
  Type : string
  Id : string
}

type ResourceHandler = (RouteId list -> WebPart) option

type ResourceActions = {
  Index : ResourceHandler
  Show : ResourceHandler
  Create : ResourceHandler
  Update : ResourceHandler
  Destroy : ResourceHandler
}

type ResourceContext = {
  Prefix : string
  Names : string list
}

type Resource = {
  Context : ResourceContext
  Name : string
  UrlWithId : string
  UrlNoId : string
  Actions : ResourceActions
  SubResources : Resource list
}

type IdsFormat<'tuple> = PrintfFormat<string -> 'tuple -> string, unit, string, string, 'tuple> 

let buildRouteIds ctx ids =
  List.rev ids
  |> List.zip ctx.Names
  |> List.map (fun (name, id) -> { Type = name ; Id = id })

let routeMatchFn url ctx fn = 
  match ctx.Names.Length with 
    | 0 -> pathScan (new IdsFormat<unit>(url)) (fun _ -> fn [])
    | 1 -> pathScan (new IdsFormat<string>(url)) (fun id1 -> fn <| buildRouteIds ctx [id1])
    | 2 -> pathScan (new IdsFormat<string * string>(url)) (fun (id1, id2) -> fn <| buildRouteIds ctx [id1 ; id2])
    | 3 -> pathScan (new IdsFormat<string * string * string>(url)) (fun (id1, id2, id3) -> fn <| buildRouteIds ctx [id1 ; id2 ; id3])
    | 4 -> pathScan (new IdsFormat<string * string * string * string>(url)) (fun (id1, id2, id3, id4) -> fn <| buildRouteIds ctx [id1 ; id2 ; id3 ; id4])
    | 5 -> pathScan (new IdsFormat<string * string * string * string * string>(url)) (fun (id1, id2, id3, id4, id5) -> fn <| buildRouteIds ctx [id1 ; id2 ; id3 ; id4 ; id5])
    | _ -> raise (new Exception("NOPE"))

let methodHandler mthd resHndl ids =
  match resHndl with
  | Some fn -> mthd >>= fn ids
  | None -> NOT_FOUND "NOPE"

let noIdRoutes resource = 
  routeMatchFn resource.UrlNoId resource.Context (fun ids -> 
    choose [
      methodHandler GET resource.Actions.Index ids
      methodHandler POST resource.Actions.Create ids
    ])

let idRoutes resource =
  routeMatchFn resource.UrlWithId resource.Context (fun ids -> 
    choose [
      methodHandler GET resource.Actions.Show ids
      methodHandler PUT resource.Actions.Update ids
      methodHandler DELETE resource.Actions.Destroy ids
    ])

let rec mapResource resource = 
  let subCtx = { Prefix = resource.UrlWithId ; Names = resource.Name :: resource.Context.Names }
  let fmt = new PrintfFormat<string -> int -> string, unit, string, string, string>(resource.UrlWithId)
  choose [
    noIdRoutes resource
    idRoutes resource
    choose <| List.map (fun rsrc -> mapResource rsrc) resource.SubResources
  ]

let resource name hndl subResources ctx = 
  let url = sprintf "%s/%s" ctx.Prefix name
  let urlWithId = url + "/%s"
  let subCtx = { Prefix = urlWithId ; Names = name :: ctx.Names }
  let subR = List.map (fun sr -> sr subCtx) subResources

  { 
    UrlNoId = url
    UrlWithId = urlWithId
    Context = ctx
    Name = name 
    Actions = hndl 
    SubResources = subR 
  }

let printRouteLn verb uri action = 
  printfn "%-6s\t\t%-40s\t\t%-30s" verb uri action


let printResourceRoute verb withId action resource =
  let uri = if withId then resource.UrlWithId else resource.UrlNoId
  printRouteLn verb uri (sprintf "%s#%s" resource.Name action)

let rec printRoute resource = 
  [
    printResourceRoute "GET" false "Index"
    printResourceRoute "POST" false "Create"
    printResourceRoute "GET" true "Show"
    printResourceRoute "PUT" true "Update"
    printResourceRoute "DELETE" true "Delete"
  ]
  |> List.iter (fun f -> f resource)
  printRoutes resource.SubResources
  
and printRoutes resources =
  List.iter printRoute resources

let Root = { Prefix = "" ; Names = [] }

type ResourceDefinition = ResourceContext -> Resource

let mutable Resources : ResourceDefinition list = []

let printRouteDefs() = 
  printRouteLn "Verb" "Uri Pattern" "Action"
  printRoutes <| List.map (fun r -> r Root) Resources

let makeApp () =
  Resources 
  |> List.map (fun r -> mapResource (r Root))
  |> choose
