module ColdTranslation.Interception

open PS4RemotePlayInterceptor
open Elmish
open System.Windows
open Elmish.WPF


type Model = 
  { ControllerMode: bool
    DPadLeft: bool
    DPadRight: bool }

let init = 
  { ControllerMode = false
    DPadLeft = false
    DPadRight = false }

type Msg =
  | InterceptFail
  | Init
  | DPadLeft
  | DPadRight

let initInterception () =
  try
    Interceptor.Inject() |> ignore
  with 
    | :? System.Exception ->
      Application.Current.MainWindow.Close()
      MessageBox.Show(
          "Error on injecting." +
          "\nEither Remote Play is not started or something other hinders the injection." +
          "\nPlease start/restart RemotePlay before restarting Cold Translation.",
          "Injection Error",
          MessageBoxButton.OK,
          MessageBoxImage.Error) |> ignore
      Application.Current.Shutdown ()

   

let update msg m =
    match msg with
    | InterceptFail -> m,Cmd.none
    | Init -> m, Cmd.attemptFunc initInterception () raise
    | DPadLeft ->
      printfn "LEFT DOWN"
      m, Cmd.none
    | DPadRight ->
      printfn "RIGHT DOWN"
      m, Cmd.none

let bindings () = 
  [
    "ControllerModeInverse" |> Binding.oneWay(fun m -> m.ControllerMode |> not)
    "ControllerMode" |> Binding.oneWay(fun m -> m.ControllerMode)
  ]

let subscribe initial =
  let sub dispatch =
    Interceptor.Callback <- fun state -> 
      ( 
      )
  Cmd.ofSub sub


