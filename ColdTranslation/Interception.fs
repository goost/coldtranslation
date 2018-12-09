module ColdTranslation.Interception

open PS4RemotePlayInterceptor
open Elmish


type Model = 
  { DPadLeft: bool
    DPadRight: bool }

let init = 
  { DPadLeft = false
    DPadRight = false }

type Msg =
  | DPadLeftDown
  | DPadLeftUp
  | DPadRightUp
  | DPadRightDown

let update msg m =
    match msg with
    | DPadLeftDown ->
      printfn "LEFT DOWN"
      m
    | DPadLeftUp -> 
      printfn "LEFT UP"
      m
    | DPadRightDown ->
      printfn "RIGHT DOWN"
      m
    | DPadRightUp ->
      printfn "RIGHT UP"
      m

let subscribe initial =
  let sub dispatch =
    Interceptor.Callback <- fun state -> 
      ( if state.DPad_Left then
          dispatch <| DPadLeftDown
        else 
          dispatch <| DPadLeftUp
      )
  Cmd.ofSub sub


