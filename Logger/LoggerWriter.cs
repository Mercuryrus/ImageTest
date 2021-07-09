using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageTest.Logger
{
    public class LoggerWriter
    {
        public void WriteLog(string logMessage)
        {
            using (StreamWriter writeLog = File.AppendText("logger/log.txt"))
            {
                Log(logMessage, writeLog);
            }
        }
        public void Log(string logMessage, TextWriter writeLog)
        {
            writeLog.Write("---------------------------------------------------------------------");
            writeLog.Write("\r\nLog Entry : ");
            writeLog.WriteLine($"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()} ");
            writeLog.WriteLine($"{logMessage}");
            writeLog.WriteLine("---------------------------------------------------------------------");
        }
    }
}
