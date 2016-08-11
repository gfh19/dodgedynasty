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
			Log(Constants.LogTypes.Error, ex.Message, ex.StackTrace, requestUrl, userName, draftId);
		}

		public static void LogInfo(Exception ex, string requestUrl = null, string userName = null, int? draftId = null)
		{
			Log(Constants.LogTypes.Info, ex.Message, ex.StackTrace, requestUrl, userName, draftId);
		}

		public static void Log(string logType, string message, string stackTrace, string requestUrl = null, string userName = null, int? draftId = null)
		{
			try
			{
				var now = Utilities.GetEasternTime();
				using (var homeEntity = new HomeEntity())
				{
					homeEntity.ErrorLogs.AddObject(new ErrorLog
					{
						ErrorType = logType,
						MessageText = message.Truncate(1000),
						StackTrace = stackTrace.Truncate(3000),
                        Request = requestUrl.Truncate(500),
						UserName = userName,
                        DraftId = draftId,
						AddTimestamp = now,
						LastUpdateTimestamp = now
					});
					homeEntity.SaveChanges();
                }
			}
			catch { }
		}
	}
}
