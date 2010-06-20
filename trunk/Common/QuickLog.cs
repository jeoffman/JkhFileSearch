using System;
using System.Diagnostics;	//EventLog
using System.IO;	//StreamWriter

namespace QuickLogging
{
	public class QuickLog
	{
		public static string GenerateDefaultLogFileName()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\" + DateTime.Now.Year + "-" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".log";
		}

		/// <summary>
		/// Pass in the fully qualified name of the log file you want to write to
		/// and the message to write
		/// </summary>
		/// <param name="LogPath"></param>
		/// <param name="Message"></param>
		public static void WriteToLog(string LogPath, string text)
		{
			Debug.WriteLine("LOG>" + text);
			try
			{
				using(StreamWriter s = File.AppendText(LogPath))
				{
					s.WriteLine(DateTime.Now + "\t" + text);
				}
			}
			catch(Exception exc)
			{
				System.Diagnostics.Debug.WriteLine(exc.Message);
			}
		}

		public static void Error(string text)
		{
			WriteToLog(GenerateDefaultLogFileName(), text);
		}

		public static void ErrorException(string text, Exception exc)
		{
			WriteToLog(GenerateDefaultLogFileName(), text + exc.Message);
		}

		/// <summary>
		/// Writes a message to the application event log
		/// /// </summary>
		/// <param name="Source">Source is the source of the message ususally you will want this to be the application name</param>
		/// <param name="Message">message to be written</param>
		/// <param name="EntryType">the entry type to use to categorize the message like for exmaple error or information</param>
		public static void WriteToEventLog(string Source, string Message, System.Diagnostics.EventLogEntryType EntryType)
		{
			try
			{
				if(!EventLog.SourceExists(Source))
				{
					EventLog.CreateEventSource(Source, "Application");
				}
				EventLog.WriteEntry(Source, Message, EntryType);
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.Message);
			}
		}
	}
}
