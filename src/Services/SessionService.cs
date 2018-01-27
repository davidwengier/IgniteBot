using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using Newtonsoft.Json;

namespace IgniteBot.Services
{
	public class SessionService
	{
		private const string SessionCacheKey = "Sessions";
		private const string SessionList = "https://api.msftignite.com.au/searchApi/GetAllConfirmedFilteredSessions";

		public IEnumerable<Session> GetNextSessions()
		{
			Trace.TraceInformation("Getting next sessions where start > " + DateTime.UtcNow.Subtract(TimeSpan.FromHours(-10)));

			var data = LoadSessionData();
			var sessions = (from s in data.Sessions
							where !s.Details.Track.Equals("Expo", StringComparison.OrdinalIgnoreCase)
							where !s.Details.Track.Equals("Hack", StringComparison.OrdinalIgnoreCase)
							where s.Schedule?.Room?.StartsWith("Exam Cram", StringComparison.OrdinalIgnoreCase) != true
							where s.Schedule.StartDatetime > DateTime.UtcNow.Subtract(TimeSpan.FromHours(-10))
							group s by s.Schedule.StartDatetime into g
							orderby g.Key
							select new
							{
								DateTime = g.Key,
								Sessions = g.ToArray()
							}).Take(1).FirstOrDefault().Sessions;

			Trace.TraceInformation("Got back " + sessions.Length + " sessions");
			return sessions;
		}

		public IEnumerable<Session> GetNowSessions()
		{
			Trace.TraceInformation("Getting next sessions where start > " + DateTime.UtcNow.Subtract(TimeSpan.FromHours(-10)));

			var data = LoadSessionData();
			var sessions = (from s in data.Sessions
							where !s.Details.Track.Equals("Expo", StringComparison.OrdinalIgnoreCase)
							where !s.Details.Track.Equals("Hack", StringComparison.OrdinalIgnoreCase)
							where s.Schedule?.Room?.StartsWith("Exam Cram", StringComparison.OrdinalIgnoreCase) != true
							where s.Schedule.StartDatetime < DateTime.UtcNow.Subtract(TimeSpan.FromHours(-10))
							group s by s.Schedule.StartDatetime into g
							orderby g.Key descending
							select new
							{
								DateTime = g.Key,
								Sessions = g.ToArray()
							}).Take(1).FirstOrDefault().Sessions;

			Trace.TraceInformation("Got back " + sessions.Length + " sessions");
			return sessions;
		}

		private Rootobject LoadSessionData()
		{
			var sessionData = (Rootobject)MemoryCache.Default.Get(SessionCacheKey);
			if (sessionData == null)
			{
				using (WebClient client = new WebClient())
				{
					string results = client.UploadString(SessionList, "");

					sessionData = JsonConvert.DeserializeObject<Rootobject>(results);
					MemoryCache.Default.Set(SessionCacheKey, sessionData, DateTimeOffset.Now.AddMinutes(30));
				}
			}
			return sessionData;
		}
	}
}