using System;
using System.Collections.Generic;
using System.Text;

//===========================================================================================
namespace NGK.MeasuringDeviceTech.Classes.Print
{
    //=======================================================================================
    /// <summary>
    ///  Класс для огранизации печати для текстовых документов.
    /// </summary>
    public class TextDocumentPrinter
    {
        //-----------------------------------------------------------------------------------
        #region Fields and Properties
        //-----------------------------------------------------------------------------------
        private System.IO.StreamReader _FileToPrint;
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Поток-файл для печати
        /// </summary>
        public System.IO.StreamReader FileToPrint
        {
            get { return _FileToPrint; }
            set { _FileToPrint = value; }
        }
        //-----------------------------------------------------------------------------------
        private System.Drawing.Font _PrintFont;
        //-----------------------------------------------------------------------------------
        private System.Drawing.Printing.PrintDocument _PrintDocument;
        //-----------------------------------------------------------------------------------
        #endregion
        //-----------------------------------------------------------------------------------
        #region Constructors
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public TextDocumentPrinter()
        {
            _PrintDocument = new System.Drawing.Printing.PrintDocument();
            _PrintDocument.PrintPage += 
                new System.Drawing.Printing.PrintPageEventHandler(_PrintDocument_PrintPage);

            // Шрифт по умолчанию
            _PrintFont = new System.Drawing.Font("Courier New", 10);
        }
        //-----------------------------------------------------------------------------------
        #endregion
        //-----------------------------------------------------------------------------------
        #region Methods
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события печати страницы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _PrintDocument_PrintPage(
            object sender, 
            System.Drawing.Printing.PrintPageEventArgs e)
        {
            Single y = e.MarginBounds.Top;
            String line = null;
                        
            // Печать строк файла
            while (y < e.MarginBounds.Bottom)
            {
                try
                {
                    line = _FileToPrint.ReadLine();
                    
                    if (line == null)
                    {
                        break;
                    }

                    y = y + _PrintFont.Height;
                    e.Graphics.DrawString(
                        line, _PrintFont, System.Drawing.Brushes.Black, 
                        e.MarginBounds.Left, y);
                }
                catch (Exception ex)
                {
                    // Операция прервана в связи с какимто сбоем
                    System.Windows.Forms.MessageBox.Show(String.Format("Сбой при печати: {0}", ex.Message), 
                        "Ошибка", 
                        System.Windows.Forms.MessageBoxButtons.OK, 
                        System.Windows.Forms.MessageBoxIcon.Error);
                    
                    // Отменяем задание для принтера
                    e.Cancel = true;
                }
            }
            // Если есть ещё строки, то печать следующей страницы
            if (line != null)
            {
                e.HasMorePages = true;
            }
            else
            {
                e.HasMorePages = false;
                //Устанавливаем на начало
                // Устанавливаем курсор на начало файла
                System.IO.Stream stream = _FileToPrint.BaseStream;
                stream.Seek(0, System.IO.SeekOrigin.Begin);
            }
            return;
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Печать текстового документа
        /// </summary>
        /// <param name="documentsName">Название файла для печати</param>
        public void Pint(String documentsName)
        {
            if ((_FileToPrint != null) || (documentsName != String.Empty))
            {
                // Задаём имя, указываемого в очереди печати в диалоговом окне "состояние печати"
                _PrintDocument.DocumentName = "Document Name"; // documentsName

                // Устанавливаем курсор на начало файла
                System.IO.Stream stream = _FileToPrint.BaseStream;
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                
                // Печать документа
                _PrintDocument.Print();
            }
            return;
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Вызвает окно настроект принтера и параметров страницы
        /// </summary>
        public void PrintSettings()
        {
            if (_PrintDocument != null)
            {
                System.Windows.Forms.PrintDialog printDialog = 
                    new System.Windows.Forms.PrintDialog();

                printDialog.Document = _PrintDocument;

                System.Windows.Forms.DialogResult result = printDialog.ShowDialog();
            }
            return;
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Вызывает окно предварительного просмотра печати документа
        /// </summary>
        public void PrintPreview()
        {
            if ((_FileToPrint != null) && (_PrintDocument != null))
            {
                System.Windows.Forms.PrintPreviewDialog printPreviewDialog =
                    new System.Windows.Forms.PrintPreviewDialog();

                // Устанавливаем курсор на начало файла
                System.IO.Stream stream = _FileToPrint.BaseStream;
                stream.Seek(0, System.IO.SeekOrigin.Begin);

                printPreviewDialog.Document = _PrintDocument;
                System.Windows.Forms.DialogResult result = printPreviewDialog.ShowDialog();
            }
            return;
        }
        //-----------------------------------------------------------------------------------
        /// <summary>
        /// Выводит диалого настройки страницы для печати
        /// </summary>
        public void PageSettings()
        {
            if (_PrintDocument != null)
            {
                System.Windows.Forms.PageSetupDialog pageSetupDialog =
                    new System.Windows.Forms.PageSetupDialog();

                pageSetupDialog.Document = _PrintDocument;

                pageSetupDialog.ShowDialog();
            }
            return;
        }
        //-----------------------------------------------------------------------------------
        #endregion
        //-----------------------------------------------------------------------------------
        #region Events
        //-----------------------------------------------------------------------------------
        #endregion
        //-----------------------------------------------------------------------------------
    }
    //=======================================================================================
}
//===========================================================================================
// End of file