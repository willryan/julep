namespace TestTemplate

open NUnit.Framework
open FsUnit


[<TestFixture>]
type ``addition works`` ()=
  let five = 5

  [<Test>] 
  member x.``when i add 3`` ()= 
    five + 3 |> should equal 8

  [<Test>] 
  member x.``when i add 6`` ()= 
    five + 6 |> should equal 12

