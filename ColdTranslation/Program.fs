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
  type Model =
    { Translation: Translation.Model
      Interception: Interception.Model}

  let init () =
    { Translation = Translation.init
      Interception = Interception.init },
    Cmd.none

  type Msg =
    | Pick
    | Exit
    | InterceptionMsg of Interception.Msg
    | TranslationMsg of Translation.Msg

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
      { m with Interception = Interception.update msg m.Interception}, Cmd.none
    | TranslationMsg msg ->
      let nt,cmd = Translation.update msg m.Translation
      { m with Translation = nt}, Cmd.map TranslationMsg cmd

  let bindings model dispatch =
    [
    "Translation" |> Binding.subModel 
      (fun m -> m.Translation)
      Translation.bindings
      TranslationMsg
    "Exit"    |> Binding.cmd (fun m -> Exit)
    ]

  let subscription model =
    Cmd.batch [ Cmd.map InterceptionMsg (Interception.subscribe model.Interception)
                Cmd.map TranslationMsg (Translation.subscribe model.Translation)
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
      Interceptor.StopInjection()
      Settings.Default.Save()
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
 // let pid = Interceptor.Inject()
 // printfn "%i" pid
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
  //let p = Program.mkProgram App.init App.update App.bindings
  
  
  u
  |> Program.withErrorHandler (fun _ -> Interceptor.StopInjection())
  |> Program.withSubscription App.subscription
  |> Program.withConsoleTrace
  |> Program.runWindowWithConfig
      { ElmConfig.Default with LogConsole = true }
      (window)