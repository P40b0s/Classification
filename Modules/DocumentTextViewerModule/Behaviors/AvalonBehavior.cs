using NLog;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Shapes;
using System.Linq;
using System.Windows;
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Document;
using Classification.Modules.DocumentTextViewerModule.Models;

namespace Classification.Modules.DocumentTextViewerModule.Behaviors
{
    public sealed class AvalonBehavior : Behavior<ICSharpCode.AvalonEdit.TextEditor>
    {
        #region Привязка текста
        public static readonly DependencyProperty DocumentTextProperty =
                               DependencyProperty.Register("DocumentText", typeof(string), typeof(AvalonBehavior),
                               new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PropertyChangedCallback));

        public string DocumentText
        {
            get { return (string)GetValue(DocumentTextProperty); }
            set { SetValue(DocumentTextProperty, value); }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            try
            {
                var behavior = dependencyObject as AvalonBehavior;
                if (behavior.AssociatedObject != null)
                {
                    var editor = behavior.AssociatedObject as ICSharpCode.AvalonEdit.TextEditor;
                    if (editor.Document != null)
                    {
                        var caretOffset = editor.CaretOffset;
                        editor.Document.Text = dependencyPropertyChangedEventArgs.NewValue != null ? dependencyPropertyChangedEventArgs.NewValue.ToString() : string.Empty;
                        editor.CaretOffset = caretOffset;
                    }
                }
            }
            catch (Exception ex) { }
        }
      
        #endregion

        #region Привязка выбранного слова
        /// <summary>
        /// Если local = false то изменение свойства было проведено из привязанного свойства, а слово необходимо заменить, если local = true то это двойной клик по слову
        /// </summary>
        private static bool local = false;
        public static readonly DependencyProperty SelectedTextProperty =
                               DependencyProperty.Register("SelectedText", typeof(WordPositionStruct), typeof(AvalonBehavior),
                               new FrameworkPropertyMetadata(default(WordPositionStruct), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, SelectedTextPropertyChangedCallback));

        public WordPositionStruct SelectedText
        {
            get { return (WordPositionStruct)GetValue(SelectedTextProperty); }
            set { SetValue(SelectedTextProperty, value); }
        }

        private static void SelectedTextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var behavior = dependencyObject as AvalonBehavior;
            if (behavior.AssociatedObject != null)
            {
                var editor = behavior.AssociatedObject as ICSharpCode.AvalonEdit.TextEditor;
                if (editor.Document != null)
                {
                    var newvalue = (WordPositionStruct)dependencyPropertyChangedEventArgs.NewValue;
                    var oldvalue = (WordPositionStruct)dependencyPropertyChangedEventArgs.OldValue;
                    if (oldvalue != null && newvalue == oldvalue)
                        return;
                    if (!local)
                    {
                        editor.TextArea.Document.Replace(newvalue.IndexOfWord, newvalue.rootLenght, newvalue.Word, OffsetChangeMappingType.RemoveAndInsert);
                        editor.TextArea.TextView.Redraw();
                    }
                    local = false;
                }
            }
        }
        #endregion

        readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected override void OnAttached()
        {
            base.OnAttached();
            base.AssociatedObject.Loaded += AssociatedObject_Loaded;
            base.AssociatedObject.TextChanged += AssociatedObject_TextChanged;
            base.AssociatedObject.MouseDoubleClick += AssociatedObject_MouseDoubleClick;

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            base.AssociatedObject.Loaded -= AssociatedObject_Loaded;
            base.AssociatedObject.TextChanged -= AssociatedObject_TextChanged;
            base.AssociatedObject.MouseDoubleClick -= AssociatedObject_MouseDoubleClick;
        }

        private void AssociatedObject_TextChanged(object sender, EventArgs e)
        {
            var textEditor = sender as ICSharpCode.AvalonEdit.TextEditor;
            if (textEditor != null)
            {
                if (textEditor.Document != null)
                    DocumentText = textEditor.Document.Text;
            }
        }

        private void AssociatedObject_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ICSharpCode.AvalonEdit.TextEditor editor = (ICSharpCode.AvalonEdit.TextEditor)sender;
            if (editor.SelectionLength > 0)
            {
                local = true;
                SetValue(SelectedTextProperty, new WordPositionStruct()
                {
                    IndexOfWord = editor.SelectionStart,
                    rootLenght = editor.SelectionLength,
                    Word = editor.SelectedText
                });
            }
        }
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            var editor = (ICSharpCode.AvalonEdit.TextEditor)sender;
            editor.TextArea.TextView.LineTransformers.Add(new ColorizeAvalonEdit());
        }


    }
}
