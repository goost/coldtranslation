using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using ColdTranslation.Properties;
using ColdTranslation.View;
using OfficeOpenXml;

namespace ColdTranslation.Translation
{
    public class TranslationReader : IDisposable
    {
        public ExcelWorksheet CurrentSheet { get; private set; }
        public int CurrentRow { get; private set; }
        private Settings Settings { get; } = Properties.Settings.Default;
        private string LastSpeaker { get; set; } = "";

        private ExcelPackage _package;

        public TranslationReader()
        {
        }

        public Model.Translation Next()
        {
            CurrentRow++;
            return GetCurrentLine();
        }

        public Model.Translation Previous()
        {
            CurrentRow--;
            CurrentRow = Math.Max(1, CurrentRow);
            return GetCurrentLine();
        }

        public Model.Translation? ReadXlsx(string path, Window owner)
        {
            try
            {
                _package = new ExcelPackage(new FileInfo(path));
                _package.Compatibility.IsWorksheets1Based = false;

                var selection = new SelectSheetWindow(_package.Workbook.Worksheets.Select(s => s.Name),
                    Settings.Sen4Mode)
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                if (selection.ShowDialog() != true) return null;
                CurrentSheet = _package.Workbook.Worksheets[selection.SelectedSheet];
                Settings.Sen4Mode = selection.IsSen4Mode;
                selection.Close();

                var last = Settings.LastRows.Find(it => it.Sheet == $"{_package.File.Name}:{CurrentSheet.Name}");
                CurrentRow = last.Row == 0 ? 3 : last.Row;
                if (!Settings.Sen4Mode && string.IsNullOrEmpty(CurrentSheet.Cells[CurrentRow, 1].Text)
                                       && !string.IsNullOrEmpty(CurrentSheet.Cells[CurrentRow, 2].Text)
                                       && !CurrentSheet.Cells[CurrentRow, 2].Text.Contains(">"))
                {
                    var row = CurrentRow - 1;
                    do
                    {
                        LastSpeaker = CurrentSheet.Cells[row--, 1].Text;
                    } while (string.IsNullOrEmpty(LastSpeaker));
                }

                Settings.LastTranslationSheet = path;
                return GetCurrentLine();
            }
            catch (Exception e)
            {
                _package?.Dispose();
                _package = null;
              
                return null;
            }
        }

        private Model.Translation GetCurrentLine()
        {
            if (_package == null)
            {
                return new Model.Translation()
                {
                    Extra = "",
                    Speaker = "",
                    Color = "FF000000",
                    Speech = ""
                };
            }
            Save();
            var speaker = CurrentSheet.Cells[CurrentRow, 1].Text;
            var speechCell = CurrentSheet.Cells[CurrentRow, 2];

            var speech = speechCell.RichText.Text;
            if (!Settings.Sen4Mode && string.IsNullOrEmpty(speaker)
                && !string.IsNullOrEmpty(speech)
                && !CurrentSheet.Cells[CurrentRow, 2].Text.Contains(">"))
            {
                speaker = LastSpeaker;
            }

            LastSpeaker = speaker;
            return new Model.Translation()
            {
                Speaker = speaker,
                Speech = speech,
                Extra = CurrentSheet.Cells[CurrentRow, 3].Text,
                Color = speechCell.Style.Font.Color.Rgb ?? "FF000000"
            };
        }

        private void Save()
        {
            var sheetQualifier = $"{_package.File.Name}:{CurrentSheet.Name}";
            var last = Settings.LastRows.Find(r => r.Sheet == sheetQualifier);
            Settings.LastRows.Remove(last);
            last.Row = CurrentRow;
            last.Sheet = sheetQualifier;
            Settings.LastRows.Add(last);
            Settings.Save();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            _package?.Dispose();
            _package = null;
        }

        ~TranslationReader()
        {
            Dispose(false);
        }
    }
}