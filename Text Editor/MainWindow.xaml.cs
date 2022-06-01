﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace Text_Editor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ViewModel viewModel = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = viewModel;
            viewModel.TextEditor = txtEditor;

            // Біндимо до випадаючих списків список шрифтів та список розміру шрифту відповідно
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontFamily.SelectedIndex = 8;
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };

            //txtEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        #region [On any text changes event handler]
        private void txtEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            this.Title = txtEditor.CaretPosition.GetTextInRun(LogicalDirection.Forward);
            //txtEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            CountInTextWordsCharsLines();
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
                cmbFontFamily.FontFamily = cmbFontFamily.SelectedItem as FontFamily;
                txtEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
            }
        }
        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
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
            lblLinesCount.Text = $"Lines: {splittedLines.Length}";
            lblCharsCount.Text = $"Chars: {charsCount}";
            lblWordsCount.Text = $"Words: {wordsCount}";
        }
        #endregion
    }
}