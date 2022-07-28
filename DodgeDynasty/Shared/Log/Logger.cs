using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DodgeDynasty.Entities;

namespace DodgeDynasty.Shared.Log
{
	public class Logger
	{
		public static void LogError(Exception ex, string requestUrl = null, string userName = null, int? draftId = null)
		{
			Log(Constants.LogTypes.Error, GetMessage(ex), GetStackTrace(ex), requestUrl, userName, draftId);
		}

		public static void LogInfo(string message, string stackTrace, string requestUrl = null, string userName = null, int? draftId = null)
		{
			Log(Constants.LogTypes.Info, message, stackTrace, requestUrl, userName, draftId);
		}

		public static void Log(string logType, string message, string stackTrace, string requestUrl = null, string userName = null, int? draftId = null)
		{
			try
			{
				if (!isKnownError(message))
				{
					var now = Utilities.GetEasternTime();
					using (var homeEntity = new HomeEntity())
					{
						homeEntity.ErrorLogs.AddObject(new ErrorLog
						{
							ErrorType = logType,
							MessageText = message?.Truncate(1000),
							StackTrace = stackTrace?.Truncate(3000),
							Request = requestUrl?.Truncate(500),
							UserName = userName,
							DraftId = draftId,
							AddTimestamp = now,
							LastUpdateTimestamp = now
						});
						homeEntity.SaveChanges();
					}
				}
			}
			catch { }
		}

		public static string GetMessage(Exception ex)
		{
			var message = new StringBuilder();
			if (ex != null)
			{
				message.Append(ex.Message);
				if (ex.InnerException != null)
				{
					message.Append(string.Format("{0} Inner Exception: {1}", Environment.NewLine, ex.InnerException.Message));
				}
			}
			return message.ToString();
		}

		public static string GetStackTrace(Exception ex)
		{
			var stackTrace = new StringBuilder();
			if (ex != null)
			{
				if (ex.InnerException != null)
				{
					stackTrace.Append(string.Format("Inner Exception: {1}{0} Exception: ", Environment.NewLine, ex.InnerException.StackTrace));
				}
				stackTrace.Append(ex.StackTrace);
			}
			return stackTrace.ToString();
		}

		private static bool isKnownError(string message)
		{
			List<string[]> knownErrors = new List<string[]>();
			knownErrors.Add(new[] { "The controller for path", "/bundles/", "was not found or does not implement IController" });

			foreach (var knownError in knownErrors)
			{
				if (knownError.ToList().All(fragment=>message.Contains(fragment)))
				{
					return true;
				}
			}
			return false;
		}
	}
}
