module ColdTranslation.Migration

open System.Windows

let migrateSettings () =
  let settings = Legacy.Settings.OldSettings
  if not <| isNull settings then 
    printfn "Got old Settings"
    let newS = Properties.Settings.Default
    newS.LastRows <- settings.LastRows
    newS.HideSpeaker <- settings.HideSpeaker
    newS.LastTranslationSheet <- settings.LastTranslationSheet
    newS.Top <- float settings.Location.Y
    newS.Left <- float settings.Location.X
    newS.Sen4Mode <- settings.Sen4Mode
    

