using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlueRavenUtility
{
    public class Logger
    {
        public enum LogLevel
        {
            Info,
            Caution,
            Error,
            Fatal,
			Debug
        }

		private static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();
		public static Logger InitLogger(string name)
		{
			loggers.Add(name, new Logger(name));

			return loggers[name];
		}

		public static Logger GetOrCreate(string name)
		{
			if (loggers.ContainsKey(name))
				return loggers[name];
			else return InitLogger(name);
		}

		public static Logger GetLogger(string name)
		{
			return loggers[name];
		}

		public List<string> totalLog;

		private StringBuilder dumpLog;

		private string sessionLogName;

		private string name;

		private Thread mainThread;

        public Logger(string name)
        {
			this.name = name;

			mainThread = Thread.CurrentThread;

			dumpLog = new StringBuilder();
			totalLog = new List<string>(512);

			sessionLogName = GetFileName();

			string folderName = GetFolderName();

			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			string[] logFiles = Directory.GetFiles(folderName).Where(x => x.StartsWith(folderName + "log_")).ToArray();

			int numPruned = 0;
			for (int i = 0; i < logFiles.Length; i++)
			{
				if (i > 10)
				{
					File.Delete(logFiles[i]);
					Log(LogLevel.Debug, "Pruning " + logFiles[i] + ".");
					numPruned++;
				}
			}

			Log(LogLevel.Info, "Pruned " + numPruned + " log files. There were " + logFiles.Length + " files in this directory.");
        }

        public void Log(LogLevel level, string message)
        {
            string fmessage = level.ToString() + ": " + message;
            Console.WriteLine(fmessage);

#if !DEBUG
			if (level == LogLevel.Debug)
				return;	//don't log debug logs if we're on release version
#endif
			string threadName = Thread.CurrentThread.Name;

			if (threadName == "" && Thread.CurrentThread == mainThread)
				threadName = "Main Thread";

			string full = "[" + name + "]" + "[" + level.ToString() + "][" + threadName + "][" + DateTime.Now.ToString("g") + "] " + message + "\n";

			totalLog.Add(full);
			dumpLog.Append(full);

			if (level == LogLevel.Fatal)
			{
				FlushToLog();
				throw new FatalLogException(message);
			}
        }

		public void FlushToLog()
		{
			if (!Directory.Exists(GetFolderName()))
				Directory.CreateDirectory(GetFolderName());

			using (StreamWriter fs = File.AppendText(GetFolderName() + "log_" + sessionLogName + ".log"))
				fs.Write(dumpLog.ToString());	//append

			dumpLog.Clear();
		}

		public void OnProgramExit()
		{
			FlushToLog();
		}

		private string GetFolderName()
		{
			return "logs/";
		}

		private string GetFileName()
		{
			return DateTime.Now.ToString("s").Replace(":", ".");
		}
    }

	class FatalLogException : Exception
	{
		public FatalLogException(string message) 
			: base("A Fatal Error has occured. The program will now exit. Please submit the log file located in the install directory/logs to a developer. " +
				  "The error that directly caused this error is as follows: " +
				  message)
		{
		}
	}
}
