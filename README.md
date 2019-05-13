# toastmasters-schedule
Website used to manage meeting and send reminders.

## Notes


View calendar
Manage members / use toastmasters api
Manage meetings

Take a role
Free a role
Decline a role
Create Meeting


Send Reminder
Parse email

Roles statistics



Role
{
	RoleId
	Name
	Description

	CreatedAt
	CreatedBy
	UpdatedAt
	UpdatedBy
	Alive
}

Branch
{
    PathwaysId
    Name
    Note

    CreatedAt
	CreatedBy
	UpdatedAt
	UpdatedBy
	Alive
}

Speech
{
    SpeechId
    MeetingId
    MeetingSpeakerId
    Name
    Note
    PathwaysId
    PathwaysLevel

	CreatedAt
	CreatedBy
	UpdatedAt
	UpdatedBy
	Alive
}


Meeting
{
	MeetingId
	Date
	Name
    Note

	Toastmaster
	Humorist
	Linguist

	Topicsmaster
	  Table Topics Speaker

	Meeting Speaker 1

	Meeting Speaker 2

	General Evaluator
      Evaluator 1
	  Evaluator 2
	  Grammarian
	  Ah-Counter
	  Timer

	CreatedAt
	CreatedBy
	UpdatedAt
	UpdatedBy
	Alive
}

Member
{
	MemberId
	Name
    Note

	Email

	CreatedAt
	CreatedBy
	UpdatedAt
	UpdatedBy
	Alive
}