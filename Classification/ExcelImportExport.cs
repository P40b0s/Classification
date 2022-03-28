using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Xml.Linq;
using Excel;
using System.Data;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading.Tasks;

namespace Classification
{
    class ClassForExport
    {
        public ClassForExport(int From, int To, string path)
        {
            this.From = From;
            this.To = To;
            this.path = path;
        }
        public int From { get; set; }
        public int To { get; set; }
        public string path { get; set; }
    }
    class ExcelImportExport
    {       	
            public static BackgroundWorker bg = new BackgroundWorker();
            public static MainWindow win = System.Windows.Application.Current.Windows.Cast<System.Windows.Window>().FirstOrDefault(window => window is MainWindow) as MainWindow;
           

        // Считывание данных из файла эксель.
        public static void ImportFromExcel(string path, bool Clear = true)
        {
            try
            {
                Task.Factory.StartNew(()=>{
                FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read);
                
                IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
                excelReader.IsFirstRowAsColumnNames = false;
                SQLMethods sq = new SQLMethods();
                DataTable dt = excelReader.AsDataSet().Tables[0];

                int count = dt.Rows.Count;
                int i = 0;
                    if (Clear)
                    {
                        sq.DeleteDataFromTables();
                    }            
                    
                    for ( int c = 0; c < dt.Rows.Count; c++)
                    {

                        string dataformat = "dd.MM.yyyy";
                        sq.AddingDataToTables(int.Parse(dt.Rows[c]["Column1"].ToString()), DateTime.ParseExact(dt.Rows[c]["Column4"].ToString(), dataformat, System.Globalization.CultureInfo.InvariantCulture), dt.Rows[c]["Column5"].ToString().Trim(), dt.Rows[c]["Column6"].ToString().Trim());
                        i++;
                        win.ImportFromExcelProgress.Dispatcher.Invoke(new Action(() =>
                        {
                            win.ImportFromExcelProgress.Maximum = count;
                            win.ImportFromExcelProgress.Value = i;
                            win.SerilizeExcelProcessTextBlock.Text = i.ToString() + " из " + count;
                        }));
                    }
               
                excelReader.Close();
                MessageBox.Show("Файл успешно считан!", "Считываниe excel файла");
                    DirectoryInfo complite = Directory.CreateDirectory("ImportComplite");
                    File.Copy(path, complite.Name + DateTime.Now.ToShortDateString() + ".xls");
                    File.Delete(path);
                });
            }
            catch (Exception ex) { MessageBox.Show("Ошибка: " + ex.Message, "Ошибка при считывании excel файла"); }           
        }

        public static void ExportToExcel(ClassForExport export)
        {
           if (!bg.IsBusy)
           {
               bg.WorkerReportsProgress = true;
               bg.RunWorkerCompleted += bg_RunWorkerCompleted;
               bg.DoWork += bg_DoWork;
               bg.RunWorkerAsync(export);
           }          
        }
      
        static async void  bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                SQLMethods sq = new SQLMethods();
                ObservableCollection<ReadyListBoxClass> coll = await sq.ReadFromTable();              
                ClassForExport exp = (ClassForExport)e.Argument;
                int z = 0;               
                FileInfo fi = new FileInfo(exp.path);
                ExcelPackage pac = new ExcelPackage(fi);
                pac.Workbook.Worksheets.Add("На классификацию " + DateTime.Now.ToShortDateString());
                ExcelWorksheet ws = pac.Workbook.Worksheets[1];
                ws.Name = "На классификацию " + DateTime.Now.ToShortDateString(); 
                ws.Cells.Style.Font.Size = 12; 
                ws.Cells.Style.Font.Name = "Calibri";
                ws.Cells.Style.WrapText = true;
                ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                for (int i = exp.From; i <= exp.To; i++)
                {
                    if (coll.Where(x => x.IndexNum == i).Select(m => m.Keywordd != null).FirstOrDefault())
                    {
                        ws.Cells[z + 1, 1].Value = (int)coll.Where(x => x.IndexNum == i).Select(m => m.IndexNum).FirstOrDefault();
                        ws.Cells[z + 1, 2].Value = (string)coll.Where(x => x.IndexNum == i).Select(m => m.SignDate).FirstOrDefault();
                        ws.Cells[z + 1, 3].Value = (string)coll.Where(x => x.IndexNum == i).Select(m => m.Number).FirstOrDefault();
                        ws.Cells[z + 1, 4].Value = (string)coll.Where(x => x.IndexNum == i).Select(m => m.ActName).FirstOrDefault();
                        ws.Cells[z + 1, 5].Value = GetStringFromList(coll.Where(x => x.IndexNum == i).Select(m => m.Rubriki).FirstOrDefault());
                        ws.Cells[z + 1, 6].Value = GetStringFromList(coll.Where(x => x.IndexNum == i).Select(m => m.Keywordd).FirstOrDefault());
                        bg.ReportProgress(z);
                        sq.UpdateIsReady(i);
                        z++;
                    }                   
                }
                ws.Column(1).Width = 7;
                ws.Column(2).Width = 12;
                ws.Column(3).Width = 11;
                ws.Column(4).Width = 56;
                ws.Column(5).Width = 48;
                ws.Column(6).Width = 25;
                ws.DefaultRowHeight = 140;
                pac.Save();                
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибко!"); }
        }

        static string GetStringFromList(List<string> list)
        {
            string s = null;
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (i == list.Count - 1)
                    {
                        s += list[i].Trim();
                    }
                    else
                    {
                        s += list[i].Trim() + ";" + Environment.NewLine;
                    }
                }
            }
            return s;
        }

        static async void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            win._ProgressBar.Value = 0;
            win.ProgressTextBlock.Text = "Создание EXCEL файла успешно завершено!";
            bg.RunWorkerCompleted -= bg_RunWorkerCompleted;
            bg.DoWork -= bg_DoWork;
            win.SpisokListBox.Items.Refresh();
            SQLMethods sq = new SQLMethods();
            win.ActsCount.Text = await sq.GetActsCount();
        }
    }
}
