using System;

namespace IgniteBot
{
	public class Rootobject
	{
		public int PageNumber { get; set; }
		public int PagesCount { get; set; }
		public string RegistrationId { get; set; }
		public Session[] Sessions { get; set; }
		public int UserId { get; set; }
	}

	public class Session
	{
		public int EventSessionId { get; set; }
		public int EventSessionRegistrationId { get; set; }
		public int SessionId { get; set; }
		public string Name { get; set; }
		public Speaker[] Speakers { get; set; }
		public Schedule Schedule { get; set; }
		public string Description { get; set; }
		public Details Details { get; set; }
		public bool IsScheduled { get; set; }
		public bool IsCommonSession { get; set; }
		public string SessionCss { get; set; }
	}

	public class Schedule
	{
		public DateTime StartDatetime { get; set; }
		public DateTime EndDatetime { get; set; }
		public string Venue { get; set; }
		public string Room { get; set; }
		public int EventSessionRegistrationId { get; set; }
		public string Status { get; set; }
		public bool IsToday { get; set; }
		public string FormattedVenueString { get; set; }
		public string FormattedStartDate { get; set; }
	}

	public class Details
	{
		public string Audience { get; set; }
		public string Topic { get; set; }
		public string Track { get; set; }
		public string Product { get; set; }
		public string Duration { get; set; }
		public string Level { get; set; }
	}

	public class Speaker
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string LastName { get; set; }
		public string FirstName { get; set; }
		public bool IsMVP { get; set; }
		public bool IsMicrosoftStaff { get; set; }
		public string PhotoPath { get; set; }
		public string Twitterusername { get; set; }
		public string LinkedInUrl { get; set; }
		public string Bio { get; set; }
		public string Organisation { get; set; }
	}
}