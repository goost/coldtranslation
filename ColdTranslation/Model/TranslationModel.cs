using System.ComponentModel;
using System.Runtime.CompilerServices;
using ColdTranslation.Annotations;

namespace ColdTranslation.Model
{
    public class TranslationModel : INotifyPropertyChanged
    {

        private Model.Translation _translation;
        public Model.Translation Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                OnPropertyChanged(nameof(Translation));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}