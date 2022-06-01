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

namespace Text_Editor
{
    public class ViewModel
    {
        private RichTextBox textEditor;
        private readonly RelayCommand newFileCommand;
        private readonly RelayCommand deselecAllCommand;
        private bool lockToolBar = true;

        #region [CONSTRUCTOR]
        public ViewModel()
        {
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
        public RichTextBox TextEditor{ get { return textEditor; } set { textEditor = value; } }
        public bool LockToolBar => lockToolBar; // { get { return lockToolBar; } set { lockToolBar = value; } }
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
    }
}
