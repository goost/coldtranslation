module ColdTranslation.Program
open System
open System.Windows
open Elmish
open Elmish.WPF
open ColdTranslation.Properties
open System.Windows.Input
open PS4RemotePlayInterceptor
open System.IO
open System.Reflection
open System.Diagnostics


module App =
  open System.Windows.Media

  let private timer = new System.Timers.Timer(500.)

  type Model =
    { Translation: Translation.Model
      Interception: Interception.Model
      Visible: bool
      TextBackgroundColor: string}

  type Msg =
    | Exit
    | ToggleVisibility
    | TextBackgroundColorChange of string
    | InterceptionMsg of Interception.Msg
    | TranslationMsg of Translation.Msg

  let init () =
    { Translation = Translation.init
      Interception = Interception.init
      Visible = true
      TextBackgroundColor = Settings.Default.TextBackgroundColor},
      Cmd.none

  let private confirmExit () =
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
    | TextBackgroundColorChange nc -> 
      Settings.Default.TextBackgroundColor <- nc
      {m with TextBackgroundColor = nc}, Cmd.none
    | ToggleVisibility -> {m with Visible = not m.Visible}, Cmd.none
    | Exit -> m, Cmd.attemptFunc confirmExit () raise
    | InterceptionMsg msg ->
      match msg with
      | Interception.Msg.Circle ->
        m,Cmd.ofMsg (TranslationMsg Translation.Msg.Next)
      | Interception.Msg.DPadRight ->
        m,Cmd.ofMsg (TranslationMsg Translation.Msg.Next)
      | Interception.Msg.DPadLeft ->
        m,Cmd.ofMsg (TranslationMsg Translation.Msg.Previous)
      | Interception.Msg.L3 ->
        m,Cmd.ofMsg ToggleVisibility
      | _ ->
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
    "Visible" |> Binding.oneWay(fun m-> m.Visible )
    "ToggleVisibility"    |> Binding.cmd(fun m -> ToggleVisibility)
    "TextBackgroundColorPicker" |> Binding.twoWay (fun m-> ColorConverter.ConvertFromString(m.TextBackgroundColor) :?> Color) (fun c m -> TextBackgroundColorChange (c.ToString())) 
    "TextBackgroundColor" |> Binding.oneWay (fun m-> m.TextBackgroundColor) 
    ]

  let private timerTick dispatch =
    timer.AutoReset <- false
    timer.Elapsed.Add (fun _ -> 
      dispatch <| InterceptionMsg Interception.Msg.Init
      timer.Stop()
      timer.Dispose()
    )
    timer.Start()

  let subscription model =
    Cmd.batch [ Cmd.map InterceptionMsg (Interception.subscribe model.Interception)
                Cmd.ofSub timerTick
                ]

let private createWindow () =
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
let main _argv =
  if Settings.Default.UpgradeRequired then
    Settings.Default.Upgrade()
    Settings.Default.UpgradeRequired <- false
  Migration.migrateSettings()
  Settings.Default.Save()
  
  let window = createWindow()

  #if DEBUG
  let tracePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "MessageTrace.log");
  use file = File.AppendText(tracePath)
  file.WriteLineAsync("################# NEW START ###################") |> ignore
  let fileTrace (msg: App.Msg) (model: App.Model) =
    file.WriteLine "------Update-----" |> ignore
    //printfn "Current Message: %s" (msg.ToString())
    let updatedModel = if model.Translation.Translations.Length <> 0 then
                        { model with Translation = {model.Translation with Translations = Array.take 3 <| model.Translation.Translations } }
                       else 
                         model 
    //printfn "%s" (updatedModel.ToString())
    file.WriteLine(">>>> Current Model") |> ignore
    file.WriteLine(updatedModel.ToString()) |> ignore
    file.WriteLine(">>>> Current Message: " + msg.ToString()) |> ignore
    file.Flush() |> ignore
  #endif




  Interceptor.InjectionMode <- InjectionMode.Compatibility
  Interceptor.EmulateController <- false
 
  Program.mkProgram App.init App.update App.bindings
  |> Program.withErrorHandler (fun _ -> Interceptor.StopInjection())
  |> Program.withSubscription App.subscription
#if DEBUG
  |> Program.withConsoleTrace
  |> Program.withTrace fileTrace
  |> Program.runWindowWithConfig
      { ElmConfig.Default with LogConsole = true }
      (window)
#else
  |> Program.runWindowWithConfig
      { ElmConfig.Default with LogConsole = false }
      (window)
#endif