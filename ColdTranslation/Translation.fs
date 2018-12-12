module ColdTranslation.Translation

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
open System.Reflection

let private Settings = Properties.Settings.Default

type T = {
  Speaker: string
  Speech: string
  Extra: string
  Color: string
}

type Model = 
  { Current: T
    Loading: bool
    CurrentSheet: string
    Translations: T[]
    }

let init =
  { Current = {
      Speaker = "F#Speaker"
      Speech = "F#Speech"
      Extra = "F#Extra"
      Color = "FF00FF00" }
    Loading = false
    CurrentSheet = ""
    Translations = Array.empty
    }

type Msg =
  | LoadLast
  | Picked of string
  | PickSpreadhsheet
  | LoadingError of string
  | Translations of string * T[]
  | Next
  | Previous

let private loadTranslation path =
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
    
let private openFileDialog () =
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
      (fun a -> if a <> "" then Picked a else LoadingError "You closed the file dialog. Please select it again to load a translation sheet.")
      (fun e -> LoadingError ("There was an error during file load. The message is\n" + e.Message + "\nPlease provide this when asking for support."))
  | Picked path ->
    {m with Loading = true},
    Cmd.ofFunc 
      loadTranslation 
      path
      (fun a -> if (fst a) <> "" then Translations a else LoadingError "Error on selecting sheet. Did you closed the selection window not via the button?")
      (fun _ -> LoadingError "Could not load provided XLSX. Is it a translation sheet?")
  | LoadLast -> 
    {m with Loading = true},
    Cmd.ofFunc 
      loadTranslation 
      Settings.LastTranslationSheet
      (fun a -> Translations a)
      (fun _ -> LoadingError "Could not load last loaded XLSX. Was the file deleted?")
  | LoadingError message ->
    Application.Current.MainWindow.Cursor <- null
    if message <> "" then
      MessageBox.Show(
        message,
        "Loading Error",
        MessageBoxButton.OK,
        MessageBoxImage.Error) |> ignore
    { m with Loading = false}, Cmd.none
  | Translations (cs,t) -> 
      Application.Current.MainWindow.Cursor <- null;
      if Settings.LastRows = null then Settings.LastRows <- new System.Collections.Generic.List<LastRow>()
      let last = Settings.LastRows |> Seq.tryFind (fun r -> r.Sheet = cs) |> Option.defaultValue (new LastRow(cs,0))
      let safetyBelt = if last.Row < 0 then new LastRow(cs, 0) else last
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(safetyBelt)
      Keyboard.Focus(Application.Current.MainWindow) |> ignore
      {m with 
        Loading = false
        Translations = t
        CurrentSheet=cs
        Current = t.[safetyBelt.Row] }, Cmd.none
  | Next -> 
      let last = Settings.LastRows |> Seq.find(fun r -> r.Sheet = m.CurrentSheet)
      let next = new LastRow(last.Sheet, min (m.Translations.Length-1) (last.Row+1))
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(next)
      Application.Current.Dispatcher.InvokeAsync(fun () -> Settings.Save()) |> ignore
      { m with Current = m.Translations.[next.Row]}, Cmd.none
  | Previous ->
      let last = Settings.LastRows |> Seq.find(fun r -> r.Sheet = m.CurrentSheet)
      let previous = new LastRow(last.Sheet, max 0 (last.Row-1))
      Settings.LastRows.Remove(last) |> ignore
      Settings.LastRows.Add(previous)
      Application.Current.Dispatcher.InvokeAsync(fun () -> Settings.Save()) |> ignore
      { m with Current = m.Translations.[previous.Row] }, Cmd.none

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

