using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Microsoft.Win32;

namespace Text_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();
            // При запуску ставить фокус на текстове поле
            txtEditor.Focus();
            // TODO зробити через прив'язку в XAML
            viewModel = new ViewModel(txtEditor);
            // Прив'яка до View Model
            this.DataContext = viewModel;
            // Біндимо до випадаючих списків список шрифтів та список розміру шрифту відповідно
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontFamily.SelectedItem = txtEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            cmbFontSize.Text = txtEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty).ToString();

            // -----------------------------------------[DEBUGGING]-----------------------------------------
            //txtEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            //txtEditor.Selection.Select(txtEditor.Document.ContentEnd, txtEditor.Document.ContentEnd);
            //this.Title = $"StatusBar: {statusBar.Visibility}, Property: {viewModel.StatusBarVisibility}";
            //this.Title = $"Can redo: {txtEditor.CanRedo}, Can undo: {txtEditor.CanUndo}";
            // ---------------------------------------------------------------------------------------------
        }

        #region [On any text changes in RichTextBox event handler]
        private void txtEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            CountInTextWordsCharsLines();
            UpdateToolBar();
        }
        #endregion

        #region [File-open/save/exit item command methods]
        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
            // Що б відразу після завантаження оновило значення в статус барі
            CountInTextWordsCharsLines();
        }
        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|PDF (*.pdf)|*.pdf|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }
        private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog
            {
                Filter = "Rich Text Format (*.rtf)|*.rtf|PDF (*.pdf)|*.pdf|All files (*.*)|*.*"
            };
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }
        private void Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }
        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtEditor.Selection.Text = "";
        }
        #endregion

        #region [Font settings handlers]
        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
            {
                txtEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
                cmbFontFamily.FontFamily = cmbFontFamily.SelectedItem as FontFamily;
            }
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            double textSize;
            if (double.TryParse(cmbFontSize.Text, out textSize))
            {
                txtEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cmbFontSize.Text);
            }
            else
            {
                cmbFontSize.Text = cmbFontSize.SelectedItem.ToString();
            }
        }
        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtEditor.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, cmbFontSize.Text);
        }
        #endregion

        #region [On program close event handler]
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = MessageBox.Show("Close Notepad?", "Exit program", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No;
        }
        #endregion

        #region [In text lines|words|chars counting]
        private void CountInTextWordsCharsLines()
        {
            // Кількість символів
            int charsCount = 0;
            // Кількість слів
            int wordsCount = 0;
            // Отримуємо текст з компоненту RichBoxText
            TextRange EditorText = new TextRange(txtEditor.Document.ContentStart, txtEditor.Document.ContentEnd);
            // Розбиваємо отриманий текст на масив текстових ліній 
            string[] splittedLines = EditorText.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            // Рахуємо кількість символів та слів в кожній лінії
            foreach (string line in splittedLines)
            {
                charsCount += line.Length;
                // Рахуємо кількість слів в лінії
                foreach (string word in line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    wordsCount++;
                }
            }

            // Виводимо результати в строку стану(та шо знизу, розбита спліттерами)
            lblLinesCount.Content = $"Lines: {splittedLines.Length}";
            lblCharsCount.Content = $"Chars: {charsCount}";
            lblWordsCount.Content = $"Words: {wordsCount}";
        }
        #endregion

        public void UpdateToolBar()
        {
            // Змінюємо значення checked кнопок в панелі інструментів в залежності від текста в місці каретки(або виділеного тексту)
            tglbtnBold.IsChecked = txtEditor.Selection.GetPropertyValue(TextElement.FontWeightProperty).Equals(FontWeights.Bold);
            tglbtnItalic.IsChecked = txtEditor.Selection.GetPropertyValue(TextElement.FontStyleProperty).Equals(FontStyles.Italic);
            tglbtnUnderline.IsChecked = txtEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty).Equals(TextDecorations.Underline);

            // Змінюємо обране значення комбобокса вибору шрифта текста в місці каретки(або виділеного тексту)
            cmbFontFamily.SelectedItem = txtEditor.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
            cmbFontSize.SelectedItem = txtEditor.Selection.GetPropertyValue(TextElement.FontSizeProperty).ToString();

            //tglbtnAlignLeft.IsChecked = txtEditor.Selection.GetPropertyValue(Block.TextAlignmentProperty).Equals(TextAlignment.Left);
            //tglbtnAlignCenter.IsChecked = txtEditor.Selection.GetPropertyValue(Block.TextAlignmentProperty).Equals(TextAlignment.Center);
            //tglbtnAlignRight.IsChecked = txtEditor.Selection.GetPropertyValue(Block.TextAlignmentProperty).Equals(TextAlignment.Right);
            //tglbtnAlignJustify.IsChecked = txtEditor.Selection.GetPropertyValue(Block.TextAlignmentProperty).Equals(TextAlignment.Justify);
        }

        private void RepeatButtonIncreaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            cmbFontSize.Text = (double.Parse(cmbFontSize.Text) + 0.5d).ToString();
        }
        private void RepeatButtonDereaseFontSize_Click(object sender, RoutedEventArgs e)
        {
            cmbFontSize.Text = (double.Parse(cmbFontSize.Text) - 0.5d).ToString();
        }

        
    }
}
