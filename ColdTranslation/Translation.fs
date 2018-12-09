module ColdTranslation.Translation

open System.Runtime.CompilerServices
open Elmish.WPF
open Elmish
open OfficeOpenXml
open System.IO
open System.Windows
open FSharp.Linq.NullableOperators
open System.Windows.Input
open NullCoalese
open ColdTranslation.Model
open Microsoft.Win32
open System
open System.Reflection

let Settings = Properties.Settings.Default
let Timer = new System.Windows.Threading.DispatcherTimer(Threading.DispatcherPriority.Loaded)
let DefaultDelay = 4


type T = {
  Speaker: string
  Speech: string
  Extra: string
  Color: string
}

type Model = 
  { Current: T
    SpeechView: string
    SubStringLength: int
    Loading: bool
    CurrentSheet: string
    Translations: T[]
    Delay: int
    }

let init =
  { Current = {
      Speaker = "F#Speaker"
      Speech = "F#Speech"
      Extra = "F#Extra"
      Color = "FF00FF00" }
    SpeechView = "F#Speaker"
    SubStringLength = 9
    Loading = false
    CurrentSheet = ""
    Translations = Array.empty
    Delay = 0
    }

type Msg =
  | LoadLast
  | Picked of string
  | PickSpreadhsheet
  | LoadingError of string
  | Translations of string * T[]
  | Next
  | Previous
  | Tick

let loadTranslation path =
  use package = new ExcelPackage(new FileInfo(path))
  package.Compatibility.IsWorksheets1Based <- true
  let selection = 
    new Views.SelectSheetWindow (
      package.Workbook.Worksheets |> Seq.map (fun s -> s.Name),
      Settings.Sen4Mode )
  selection.WindowStartupLocation <- WindowStartupLocation.CenterScreen
  if selection.ShowDialog () ?<> true then "",Array.empty
  else
    Settings.Sen4Mode <- selection.IsSen4Mode
    let currentSheet = package.Workbook.Worksheets.[selection.SelectedSheet]
    selection.Close()
    Settings.LastTranslationSheet <- path

    let rowCount = currentSheet.Dimension.End.Row
    let translations = 
      [| 3..rowCount|] 
        |> Array.map (
          fun i -> 
            { Speaker = currentSheet.Cells.[i,1].Text
              Speech = currentSheet.Cells.[i,2].RichText.Text
              Extra = currentSheet.Cells.[i,3].Text
              Color = currentSheet.Cells.[i,2].Style.Font.Color.Rgb |?? lazy "FF000000"  }  
        )
    if Settings.Sen4Mode then package.File.Name+":"+currentSheet.Name,translations
    else
      let f = fun s t ->
        match (t.Speaker,t.Speech) with
        | ("","")  -> t,""
        | ("", l) when not (l.Contains ">") -> {t with Speaker = s},s
        | ("", l)  -> t,t.Speaker
        | (u,v) -> t,t.Speaker
      package.File.Name+":"+currentSheet.Name,
      translations |> Array.mapFold f "" |> fst
    
let openFileDialog () =
  let dialog = new OpenFileDialog()
  dialog.InitialDirectory <- Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @""
  dialog.Filter <- "Excel 2007+ Files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
  dialog.FilterIndex <- 0
  dialog.RestoreDirectory <- true
  if dialog.ShowDialog() ?<> true then ""
  else
    dialog.FileName

let update msg m =
  match msg with 
  | PickSpreadhsheet ->
    {m with Loading = true},
    Cmd.ofFunc
      openFileDialog
      ()
      (fun a -> if a <> "" then Picked a else LoadingError "")
      (fun _ -> LoadingError "")
  | Picked path ->
    {m with Loading = true},
    Cmd.ofFunc 
      loadTranslation 
      path
      (fun a -> Translations a)
      (fun _ -> LoadingError "Could not load provided XLSX. Is it a translation sheet?")
  | LoadLast -> 
    {m with Loading = true},
    Cmd.ofFunc 
      loadTranslation 
      Settings.LastTranslationSheet
      (fun a -> Translations a)
      (fun _ -> LoadingError "Could not load last loaded XLSX. Was the file deleted?")
  | LoadingError message ->
    if message <> "" then
      MessageBox.Show(
        message,
        "Loading Error",
        MessageBoxButton.OK,
        MessageBoxImage.Error) |> ignore
    { m with Loading = false}, Cmd.none
  | Translations (cs,t) -> 
      Application.Current.MainWindow.Cursor <- null;
      let last = Settings.LastRows |> Seq.tryFind (fun r -> r.Sheet = cs) |> Option.defaultValue (new LastRow(cs,0))
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(last)
      Keyboard.Focus(Application.Current.MainWindow) |> ignore
      {m with 
        Loading = false
        Translations = t
        CurrentSheet=cs
        Current = t.[last.Row]
        Delay= DefaultDelay
        SubStringLength = 0
        SpeechView = ""}, Cmd.none
  | Next -> 
      let last = Settings.LastRows |> Seq.find(fun r -> r.Sheet = m.CurrentSheet)
      let next = new LastRow(last.Sheet, min (m.Translations.Length-1) last.Row+1)
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(next)
      Application.Current.Dispatcher.InvokeAsync(fun () -> Settings.Save()) |> ignore
      { m with 
          Current = m.Translations.[next.Row]
          Delay = DefaultDelay
          SubStringLength = 0
          SpeechView =""}, Cmd.none
  | Previous ->
      let last = Settings.LastRows |> Seq.find(fun r -> r.Sheet = m.CurrentSheet)
      let previous = new LastRow(last.Sheet, max 0 last.Row-1)
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(previous)
      Application.Current.Dispatcher.InvokeAsync(fun () -> Settings.Save()) |> ignore
      { m with 
          Current = m.Translations.[previous.Row]
          Delay = DefaultDelay
          SubStringLength = 0
          SpeechView =""}, Cmd.none
  | Tick ->
    match m.Delay with
    | 0 ->
      let sv = m.Current.Speech.Substring(0, m.SubStringLength)
      let nl = m.SubStringLength + 1
      if nl > m.Current.Speech.Length then Timer.Stop()
      { m with SpeechView = sv; SubStringLength = nl }, Cmd.none
    | x -> {m with Delay = m.Delay - 1 }, Cmd.none

let subscribe initial =
  Timer.Interval <- System.TimeSpan.FromMilliseconds(1.)
  Timer.Stop()
  let sub dispatch =
    Timer.Tick.Add (fun e -> dispatch <| Tick)
  Cmd.ofSub sub

let bindings () =
  [ "Speech"  |> Binding.oneWay(fun m -> m.Current.Speech)
    "Speaker" |> Binding.oneWay(fun m -> m.Current.Speaker)
    "Extra"   |> Binding.oneWay(fun m -> m.Current.Extra)
    "Color"   |> Binding.oneWay(fun m -> "#" + m.Current.Color)
    "Init"    |> Binding.oneWay(fun m -> m.Translations |> Array.isEmpty |> not)
    "InitInverse"    |> Binding.oneWay(fun m -> m.Translations |> Array.isEmpty)
    "Next"    |> Binding.cmdIf(fun m -> Next) (fun m -> m.Translations |> Array.isEmpty |> not)
    "Previous"|> Binding.cmdIf(fun m -> Previous) (fun m -> m.Translations |> Array.isEmpty |> not)
    "LoadLast"|> Binding.cmdIf (fun m -> Application.Current.MainWindow.Cursor <- Cursors.Wait; LoadLast) (fun m -> not m.Loading)
    "PickSpreadsheet"|> Binding.cmdIf (fun m -> Application.Current.MainWindow.Cursor <- Cursors.Wait; PickSpreadhsheet) (fun m -> not m.Loading)
  ]

