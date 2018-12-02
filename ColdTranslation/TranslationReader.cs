using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OfficeOpenXml;

namespace ColdTranslation
{
    public struct Translation
    {
        public string Speaker;
        public string Speech;
        public string Extra;
        public string Color;
    }

    public class TranslationReader: IDisposable
    {
        public ExcelWorksheet CurrentSheet { get; private set; }
        public int CurrentRow { get; private set; }
        private Settings Settings { get; }
        private string LastSpeaker { get; set; } = "";

        private ExcelPackage _package;

        public TranslationReader(Settings settings)
        {
            Settings = settings;
        }

        public Translation Next()
        {
            CurrentRow++;
            return GetCurrentLine();
        }

        public Translation Previous()
        {
            CurrentRow--;
            CurrentRow = Math.Max(1, CurrentRow);
            return GetCurrentLine();
        }

        public Translation? ReadXlsx(string path, IWin32Window owner)
        {
            try
            {
                _package = new ExcelPackage(new FileInfo(path));
                _package.Compatibility.IsWorksheets1Based = false;

                var selection = new SelectSheetForm(_package.Workbook.Worksheets.Select(s => s.Name).ToList());
                selection.ShowDialog(owner);
                CurrentSheet = _package.Workbook.Worksheets[selection.SelectedSheet];
                selection.Close();

                var last = Settings.LastRows.Find(it => it.Sheet == $"{_package.File.Name}:{CurrentSheet.Name}");
                CurrentRow = last.Row == 0 ? 3 : last.Row;
                if (string.IsNullOrEmpty(CurrentSheet.Cells[CurrentRow, 1].Text)
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

        private Translation GetCurrentLine()
        {
            if (_package == null)
            {
                return new Translation();
            }
            Console.WriteLine($"Current Sheet is {CurrentSheet.Name}:{CurrentRow}");
            Save();
            var speaker = CurrentSheet.Cells[CurrentRow, 1].Text;
            var speechCell = CurrentSheet.Cells[CurrentRow, 2];

            var speech = speechCell.RichText.Text;
            if (string.IsNullOrEmpty(speaker) 
                && !string.IsNullOrEmpty(speech)
                && !CurrentSheet.Cells[CurrentRow, 2].Text.Contains(">"))
            {
                speaker = LastSpeaker;
            }

            LastSpeaker = speaker;
            return new Translation()
            {
                Speaker = speaker,
                Speech = speech,
                Extra = CurrentSheet.Cells[CurrentRow, 3].Text,
                Color = speechCell.Style.Font.Color.Rgb
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
            Settings.Serialize(Settings.SettingsPath, Settings);
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
            Dispose(true);
        }
    }
}