using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;

namespace Text_Editor
{
    public class ViewModel : INotifyPropertyChanged
    {
        private List<double> fontSizes = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
        private RichTextBox textEditor;
        private readonly RelayCommand newFileCommand;
        private readonly RelayCommand deselecAllCommand;
        private bool lockToolBar = true;
        private bool showStatusBar = true;
        public Visibility statusBarVisibility;

        private Block _block;
        private Paragraph _paragraph;
        private Inline _Inline;
        private InlineUIContainer _inlineUIContainer;
        private bool foundit = false;
        //public Fonts.SystemFontFamilies.f fontFamily;

        #region [CONSTRUCTOR]
        public ViewModel(RichTextBox richTextBox)
        {
            textEditor = richTextBox;
            // Налаштовуємо команду "Прибрати виділення тексту"
            newFileCommand = new RelayCommand((o) => NewFile(), IsCreateNewFile);
            deselecAllCommand = new RelayCommand((o) => DeselectAllText(), IsTextSelected);
        }
        #endregion

        #region [COMMANDS]
        public ICommand DeselectAllCommand => deselecAllCommand;
        public ICommand NewFileCommand => newFileCommand;
        #endregion

        #region [PROPERTIES]
        // Властивість для об'єкту RichTextBox
        //public RichTextBox TextEditor{ get { return textEditor; } set { textEditor = value; } }
        public bool LockToolBar
        { 
            get { return lockToolBar; }
            set
            {
                lockToolBar = value;
                OnPropertyChanged();
            }
        }
        public bool IsShowStatusBar
        { 
            get { return showStatusBar; }
            set 
            { 
                showStatusBar = value;
                //statusBarVisibility = showStatusBar == true ? Visibility.Visible : Visibility.Hidden;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StatusBarVisibility));
            }
        }
        public Visibility StatusBarVisibility => showStatusBar == true ? Visibility.Visible : Visibility.Collapsed;
        public List<double> FontSizes { get { return fontSizes; } }

        public float CanUndo
        {
            get
            {
                if (textEditor.CanUndo)
                    return 1;
                else
                    return 0.5f;
            }
        }
        #endregion

        #region [METHODS]
        // Перевірка чи виділений зараз якийсь текст
        private bool IsTextSelected(object obj)
        {
            return !textEditor.Selection.IsEmpty;
        }
        // Прибрати виділення тексту
        public void DeselectAllText()
        {
            textEditor.Selection.Select(textEditor.Document.ContentEnd, textEditor.Document.ContentEnd);
        }
        
        // Створення нового файлу(насправді просто очистка текста)
        public void NewFile()
        {
            textEditor.SelectAll();
            textEditor.Selection.Text = "";
        }
        // Перевірка чи створювати новий файл за допомогою MessageBox`а
        private bool IsCreateNewFile(object obj)
        {
            return MessageBox.Show("Create new file?", "New file", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
