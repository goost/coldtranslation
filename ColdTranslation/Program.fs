module ColdTranslation.Program
open System
open System.Windows
open Elmish
open Elmish.WPF
open ColdTranslation.Properties
open System.Windows.Input
open PS4RemotePlayInterceptor
open System.Windows.Threading


module App =
  let timer = new System.Timers.Timer(500.)

  type Model =
    { Translation: Translation.Model
      Interception: Interception.Model }

  type Msg =
    | Pick
    | LoadLast
    | Exit
    | InterceptionMsg of Interception.Msg
    | TranslationMsg of Translation.Msg

  let init () =
    { Translation = Translation.init
      Interception = Interception.init },
      Cmd.none

  let confirmExit () =
    if MessageBox.Show 
      (
        "Exit Cold Translation?",
        "Confirm Exit",
        MessageBoxButton.OKCancel,
        MessageBoxImage.Question 
      )
      = MessageBoxResult.OK then
      Application.Current.Shutdown ()


  let update msg m =
    match msg with
    | Pick -> init ()
    | Exit -> m, Cmd.attemptFunc confirmExit () raise
    | InterceptionMsg msg ->
      let nm,cmd = Interception.update msg m.Interception
      { m with Interception =nm}, Cmd.map InterceptionMsg cmd
    | TranslationMsg msg ->
      let nm,cmd = Translation.update msg m.Translation
      { m with Translation = nm}, Cmd.map TranslationMsg cmd

  let bindings model dispatch =
    [
    "Translation" |> Binding.subModel 
      (fun m -> m.Translation)
      Translation.bindings
      TranslationMsg
    "Interception" |> Binding.subModel 
      (fun m -> m.Interception)
      Interception.bindings
      InterceptionMsg
    "Exit"    |> Binding.cmd (fun m -> Exit)
    ]

  let timerTick dispatch =
    timer.AutoReset <- false
    timer.Elapsed.Add (fun _ -> 
      dispatch <| InterceptionMsg Interception.Msg.Init
      timer.Stop()
      timer.Dispose()
    )
    timer.Start()

  let subscription model =
    Cmd.batch [ Cmd.map InterceptionMsg (Interception.subscribe model.Interception)
                Cmd.map TranslationMsg (Translation.subscribe model.Translation)
                Cmd.ofSub timerTick
                ]

let createWindow () =
  let window = Views.MainWindow()
  window.Top <- Settings.Default.Top
  window.Left <- Settings.Default.Left
  window.MouseDown.AddHandler 
    (fun _ e -> 
      if e.ChangedButton = MouseButton.Left then
        window.DragMove()
    )
  window.Closing.AddHandler
    (fun _ _ ->
      Settings.Default.Top <- window.Top
      Settings.Default.Left <- window.Left
    )
  window.Closed.AddHandler
    (fun _ _ ->
      Settings.Default.Save()
      Interceptor.StopInjection()
    )
  window

[<EntryPoint; STAThread>]
let main argv =
  if Settings.Default.UpgradeRequired then
    Settings.Default.Upgrade()
    Settings.Default.UpgradeRequired <- false
  Migration.migrateSettings()
  Settings.Default.Save()
  
  let window = createWindow()

  Interceptor.InjectionMode <- InjectionMode.Compatibility
  Interceptor.EmulateController <- false

  let d m : Func<'T> = Func<'T>(fun () -> m )
  let u = {
    Program.init = App.init
    Program.update = App.update
    Program.view = App.bindings
    setState = fun model  -> App.bindings model >> ignore
    subscribe = fun _ -> Cmd.none
    onError = fun _ -> Interceptor.StopInjection()
    syncDispatch = fun m ->  Application.Current.Dispatcher.Invoke(d m)
  }
  let p = Program.mkProgram App.init App.update App.bindings
  
  
  p
  |> Program.withErrorHandler (fun _ -> Interceptor.StopInjection())
  |> Program.withSubscription App.subscription
  |> Program.withConsoleTrace
  |> Program.runWindowWithConfig
      { ElmConfig.Default with LogConsole = true }
      (window)