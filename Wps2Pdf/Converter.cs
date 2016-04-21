using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Wps2Pdf
{
    public class Converter
    {
        private Word.Application wordApp = null;
        private Excel.Application excelApp = null;
        private PowerPoint.Application pptApp = null;

        private Regex wpsRegex = new Regex("\\.(docx?|dotx?|do[ct]m|wps|wpt|rtf|txt|xml|mht(ml)?|html?)$", RegexOptions.IgnoreCase);
        private Regex wppRegex = new Regex("\\.(dp[st]|pp[st][xm]?|pot[xm])$", RegexOptions.IgnoreCase);
        private Regex etRegex = new Regex("\\.(ett?|xl[st]|xls[xm]|xlt[xm]|csv)$", RegexOptions.IgnoreCase);
        private Regex pdfRegex = new Regex("\\.pdf$", RegexOptions.IgnoreCase);
        private Regex replaceRegex = new Regex("\\.([a-z]+)$", RegexOptions.IgnoreCase);

        private Stopwatch sw = new Stopwatch();

        public static Queue<QueueItem> queue = new Queue<QueueItem>();
        public static Semaphore sem = new Semaphore(65535, 65535);

        public bool isRun = true;
        
        public void run()
        {
            QueueItem item = null;
            while (isRun)
            {
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        item = queue.Dequeue();

                        item.session.Send(convert(item.filePath) ? "Success" : "Failure");
                        item = null;
                    }
                }

                sem.WaitOne();
            }
        }

        public bool convert(string filePath)
        {
            return convert(filePath, replaceRegex.Replace(filePath, ".pdf"));
        }

        public bool convert(string filePath, string pdfPath)
        {
            sw.Reset();
            if (pdfRegex.IsMatch(filePath))
            {
                Console.Write("文件 " + filePath + " 已是PDF格式。");
                Console.WriteLine();
                return false;
            }
            else if (File.Exists(pdfPath))
            {
                Console.Write("文件 " + filePath + " 的PDF格式文件已存在。");
                Console.WriteLine();
                return true;
            }

            Console.WriteLine("正在转换 " + filePath + " 到 " + pdfPath + " ...");

            sw.Start();
            try
            {
                if (wpsRegex.IsMatch(filePath))
                {
                    if (wordApp == null)
                    {
                        wordApp = new Word.Application();
                        //wordApp.Visible = true;
                        wordApp.AutoCorrect.DisplayAutoCorrectOptions = false;
                    }

                    Word.Document word = wordApp.Documents.Open(filePath);
                    word.ExportAsFixedFormat(pdfPath, Word.WdExportFormat.wdExportFormatPDF);
                    word.Close();
                    word = null;
                }
                else if (wppRegex.IsMatch(filePath))
                {
                    if (pptApp == null)
                    {
                        pptApp = new PowerPoint.Application();
                        //pptApp.Visible = PowerPoint.MsoTriState.msoTrue;
                        pptApp.AutoCorrect.DisplayAutoCorrectOptions = false;
                        pptApp.AutoCorrect.DisplayAutoLayoutOptions = false;
                    }

                    PowerPoint.Presentation ppt = pptApp.Presentations.Open(filePath, PowerPoint.MsoTriState.msoTrue, PowerPoint.MsoTriState.msoFalse, PowerPoint.MsoTriState.msoFalse);
                    ppt.ExportAsFixedFormat(pdfPath, PowerPoint.PpFixedFormatType.ppFixedFormatTypePDF);
                    ppt.Close();
                    ppt = null;
                }
                else if (etRegex.IsMatch(filePath))
                {
                    if (excelApp == null)
                    {
                        excelApp = new Excel.Application();
                        //excelApp.Visible = true;
                        excelApp.AutoCorrect.DisplayAutoCorrectOptions = false;
                    }

                    Excel.Workbook excel = excelApp.Workbooks.Open(filePath);
                    excel.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfPath);
                    excel.Close();
                    excel = null;
                }
                else
                {
                    Console.WriteLine("不支持此格式");
                    Console.WriteLine();
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            sw.Stop();

            Console.WriteLine("运行时间 " + (sw.ElapsedMilliseconds / 1000.0) + "s");

            if (File.Exists(pdfPath))
            {
                Console.WriteLine("转换为PDf成功。");
                Console.WriteLine();

                return true;
            }
            else
            {
                Console.WriteLine("转换为PDF失败。");
                Console.WriteLine();

                return false;
            }
        }
    }
}
