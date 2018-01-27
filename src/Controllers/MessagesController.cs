using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using IgniteBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace IgniteBot
{
	// remove this attribute if you want easy live testing
	// BUT FOR THE LOVE OF GOD PUT IT BACK IF YOU WANT TO WORK FOR REALS
	// ooh.. idea
#if !DEBUG
	[BotAuthentication]
#endif

	public class MessagesController : ApiController
	{
		private static readonly SessionService m_sessionService = new SessionService();

		/// <summary>
		/// POST: api/Messages
		/// Receive a message from a user and reply to it
		/// </summary>
		public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
		{
			try
			{
				// for snooping...

				//string rawContent;
				//using (var contentStream = await this.Request.Content.ReadAsStreamAsync())
				//{
				//	contentStream.Seek(0, SeekOrigin.Begin);
				//	using (var sr = new StreamReader(contentStream))
				//	{
				//		rawContent = sr.ReadToEnd();
				//		// use raw content here
				//	}
				//}

				Activity reply = null;
				if (activity.Type == ActivityTypes.Message && activity.Text != null)
				{
					Trace.TraceInformation($"{activity.Id}: Channel: {activity.ChannelId}, Conv: {activity.Conversation.Id} ({activity.Conversation.Name}), From: {activity.From.Id} ({activity.From.Name}), Text: {activity.Text}");

					// LUIS would be a better fit if I could think of more functions to add
					if (new string[] { "Whats next", "Whats happening next", "Whats on next", }.Any(s => s.Equals(activity.Text.Trim().Replace("'", "").Replace("?", ""), StringComparison.OrdinalIgnoreCase)))
					{
						reply = GetSessionsResponse(activity, false);
					}
					else if (new string[] { "Whats on now", "Whats happening?", "Whats on", "Whats now" }.Any(s => s.Equals(activity.Text.Trim().Replace("'", "").Replace("?", ""), StringComparison.OrdinalIgnoreCase)))
					{
						reply = GetSessionsResponse(activity, true);
					}
					else if (activity.Text.IndexOf("printed", StringComparison.OrdinalIgnoreCase) > -1 && activity.Text.IndexOf("agenda", StringComparison.OrdinalIgnoreCase) > -1)
					{
						reply = activity.CreateReply("Aww " + activity.From.Name + ", why you not love me? :broken_heart: ");
					}
				}
				else
				{
					reply = HandleSystemMessage(activity);
				}

				if (reply != null)
				{
					Trace.TraceInformation("Sending back response");
					ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

					await connector.Conversations.ReplyToActivityAsync(reply);
				}

				return Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (Exception ex)
			{
				Trace.TraceError("Error! " + ex.Message + " : " + ex.StackTrace + ":" + ex.ToString());
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
			}
		}

		private static Activity GetSessionsResponse(Activity activity, bool now)
		{
			Activity reply = activity.CreateReply("The " + (now ? "current" : "next") + " session(s) are:");
			IEnumerable<Session> sessions = now ? m_sessionService.GetNowSessions() : m_sessionService.GetNextSessions();
			foreach (var session in sessions)
			{
				string time;
				if (session.Schedule.IsToday)
				{
					time = session.Schedule.StartDatetime.TimeOfDay.ToString();
				}
				else
				{
					time = session.Schedule.FormattedStartDate;
				}
				reply.Attachments.Add(new ThumbnailCard(
					subtitle: session.Name,
					text: "Speakers: " + string.Join(", ", session.Speakers.Select(s => s.Name)) +
					"\nRoom: " + session.Schedule.Room +
					"\nTime: " + time).ToAttachment());
			}
			return reply;
		}

		private Activity HandleSystemMessage(Activity message)
		{
			if (message.Type == ActivityTypes.DeleteUserData)
			{
				// Implement user deletion here
				// If we handle user deletion, return a real message
			}
			else if (message.Type == ActivityTypes.ConversationUpdate)
			{
				// Handle conversation state changes, like members being added and removed
				// Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
				// Not available in all channels
				//if (message.MembersAdded.Count > 0)
				//{
				//	string names = string.Join(" and ", message.MembersAdded.Select(c => c.Name));
				//	return message.CreateReply("Hi there " + names + "!");
				//}
			}
			else if (message.Type == ActivityTypes.ContactRelationUpdate)
			{
				// Handle add/remove from contact lists
				// Activity.From + Activity.Action represent what happened
			}
			else if (message.Type == ActivityTypes.Typing)
			{
				// Handle knowing tha the user is typing
			}
			else if (message.Type == ActivityTypes.Ping)
			{
			}

			return null;
		}
	}
}