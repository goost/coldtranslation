module ColdTranslation.Interception

open PS4RemotePlayInterceptor
open Elmish
open System.Windows
open Elmish.WPF

//TODO (goost) Keyboad advance not implemented yet
let mutable private Advance = false

type Model = 
  { ControllerMode: bool
     }

let init = 
  { ControllerMode = false
     }

type Msg =
  | Init
  | ControllerModeChange of bool
  | DPadLeft
  | DPadRight
  | Circle
  | L3
  | R3
  | SimulateCircleUp
  | SimulateCircleDown

let private initInterception () =
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
    | Init -> m, Cmd.attemptFunc initInterception () raise
    | ControllerModeChange mode ->
      {m with ControllerMode = mode}, Cmd.none
    

let bindings () = 
  [
    "ControllerModeInverse" |> Binding.oneWay(fun m -> m.ControllerMode |> not)
    "ControllerMode" |> Binding.oneWay(fun m -> m.ControllerMode)
  ]

let subscribe initial =
  let sub dispatch =
    let mutable ControllerMode = false
    let mutable Touch = false
    let mutable DPadLeft = false
    let mutable DPadRight = false
    let mutable L3 = false
    let mutable R3 = false
    let mutable Circle = false
    Interceptor.Callback <- fun state -> 
      ( 
        if state.Touch1.IsTouched then Touch <- true
        else if Touch then ControllerMode <- not ControllerMode; dispatch <| Msg.ControllerModeChange ControllerMode; Touch <-false
        if ControllerMode then
          if state.DPad_Left then DPadLeft <- true
          else if DPadLeft then dispatch <| Msg.DPadLeft; DPadLeft <- false
          if state.DPad_Right then DPadRight <- true
          else if DPadRight then dispatch <| Msg.DPadRight; DPadRight <- false
          if state.L3 then L3 <- true
          else if L3 then dispatch <| Msg.L3; L3 <- false
          if state.R3 then R3 <- true
          else if R3 then dispatch <| Msg.L3; R3 <- false
          if state.Circle then Circle <- true
          else if Circle then dispatch <| Msg.Circle; Circle <- false
        if Advance then state.Circle <- true
      )
  Cmd.ofSub sub


