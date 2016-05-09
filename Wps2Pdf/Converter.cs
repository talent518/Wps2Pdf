using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Wps2Pdf
{
    public class Converter
    {
        private static Word.Application wordApp = null;
        private static Excel.Application excelApp = null;
        private static PowerPoint.Application pptApp = null;

        private static Regex wpsRegex = new Regex("\\.(docx?|dotx?|do[ct]m|wps|wpt|rtf|txt|xml|mht(ml)?|html?)$", RegexOptions.IgnoreCase);
        private static Regex wppRegex = new Regex("\\.(dp[st]|pp[st][xm]?|pot[xm])$", RegexOptions.IgnoreCase);
        private static Regex etRegex = new Regex("\\.(ett?|xl[st]|xls[xm]|xlt[xm]|csv)$", RegexOptions.IgnoreCase);
        private static Regex pdfRegex = new Regex("\\.pdf$", RegexOptions.IgnoreCase);
        private static Regex replaceRegex = new Regex("\\.([a-z]+)$", RegexOptions.IgnoreCase);

        private Stopwatch sw = new Stopwatch();

        public static Queue<QueueItem> queue = new Queue<QueueItem>();

        public const int maxQueueLength = 65535;
        public static Semaphore sem = new Semaphore(0, maxQueueLength);

        public bool isRun = true;

        public static void init()
        {
            wordApp = new Word.Application();
            wordApp.Visible = Environment.UserInteractive;
            wordApp.AutoCorrect.DisplayAutoCorrectOptions = false;

            excelApp = new Excel.Application();
            excelApp.Visible = Environment.UserInteractive;
            excelApp.AutoCorrect.DisplayAutoCorrectOptions = false;

            pptApp = new PowerPoint.Application();
            if (Environment.UserInteractive)
            {
                pptApp.Visible = PowerPoint.MsoTriState.msoTrue;
            }
            pptApp.AutoCorrect.DisplayAutoCorrectOptions = false;
            pptApp.AutoCorrect.DisplayAutoLayoutOptions = false;
        }

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
                    }
                }

                if (item != null)
                {
                    item.session.Send(convert(item.filePath) ? "Success" : "Failure");
                    item = null;
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
                Deamon.Logger.Info("文件 " + filePath + " 已是PDF格式。");
                return false;
            }
            else if (File.Exists(pdfPath))
            {
                Deamon.Logger.Info("文件 " + filePath + " 的PDF格式文件已存在。");
                return true;
            }

            Deamon.Logger.Info("正在转换 " + filePath + " 到 " + pdfPath + " ...");

            sw.Start();
            try
            {
                if (wpsRegex.IsMatch(filePath))
                {
                    Word.Document word = wordApp.Documents.Open(filePath);
                    word.ExportAsFixedFormat(pdfPath, Word.WdExportFormat.wdExportFormatPDF);
                    word.Close();
                    word = null;
                }
                else if (wppRegex.IsMatch(filePath))
                {
                    PowerPoint.Presentation ppt = pptApp.Presentations.Open(filePath, PowerPoint.MsoTriState.msoTrue, PowerPoint.MsoTriState.msoFalse, PowerPoint.MsoTriState.msoFalse);
                    ppt.ExportAsFixedFormat(pdfPath, PowerPoint.PpFixedFormatType.ppFixedFormatTypePDF);
                    ppt.Close();
                    ppt = null;
                }
                else if (etRegex.IsMatch(filePath))
                {
                    Excel.Workbook excel = excelApp.Workbooks.Open(filePath);
                    excel.ExportAsFixedFormat(Excel.XlFixedFormatType.xlTypePDF, pdfPath);
                    excel.Close();
                    excel = null;
                }
                else
                {
                    Deamon.Logger.Info("文件 " + filePath + " 不支持此格式！");
                    return false;
                }
            }
            catch (Exception e)
            {
                Deamon.Logger.ErrorFormat("转换文件 {0} 遇到了错误：{1}", filePath, e.Message);
            }

            sw.Stop();

            Deamon.Logger.InfoFormat("转换文件 {0} 运行时间 {1}s {2}", filePath, sw.ElapsedMilliseconds / 1000.0, File.Exists(pdfPath) ? "转换为PDf成功。" : "转换为PDF失败。");

            return File.Exists(pdfPath);
        }

        public static void destory()
        {
            try
            {
                wordApp.Quit();
                Deamon.Logger.Info("Word 已成功退出");
            }
            catch (COMException e)
            {
                Deamon.Logger.ErrorFormat("Word 退出时遇到错误({0}): {1}", e.ErrorCode, e.Message);
            }
            finally
            {
                wordApp = null;
            }

            try
            {
                pptApp.Quit();
                Deamon.Logger.Info("PowerPoint 已成功退出");
            }
            catch (COMException e)
            {
                Deamon.Logger.ErrorFormat("PowerPoint 退出时遇到错误({0}): {1}", e.ErrorCode, e.Message);
            }
            finally
            {
                pptApp = null;
            }

            try
            {
                excelApp.Quit();
                Console.Write("Excel 已成功退出");
            }
            catch (COMException e)
            {
                Deamon.Logger.ErrorFormat("Excel 退出时遇到错误({0}): {1}", e.ErrorCode, e.Message);
            }
            finally
            {
                excelApp = null;
            }
        }
    }
}
