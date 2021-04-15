Imports System

Namespace InfiniteAsyncSourceAdvancedSample
	Public Class IssueData
		Public Sub New(ByVal subject As String, ByVal user As String, ByVal created As DateTime, ByVal votes As Integer, ByVal tags() As String)
			Me.Subject = subject
			Me.User = user
			Me.Created = created
			Me.Votes = votes
			Me.Tags = tags
		End Sub
		Private privateSubject As String
		Public Property Subject() As String
			Get
				Return privateSubject
			End Get
			Private Set(ByVal value As String)
				privateSubject = value
			End Set
		End Property
		Private privateUser As String
		Public Property User() As String
			Get
				Return privateUser
			End Get
			Private Set(ByVal value As String)
				privateUser = value
			End Set
		End Property
		Private privateCreated As DateTime
		Public Property Created() As DateTime
			Get
				Return privateCreated
			End Get
			Private Set(ByVal value As DateTime)
				privateCreated = value
			End Set
		End Property
		Private privateVotes As Integer
		Public Property Votes() As Integer
			Get
				Return privateVotes
			End Get
			Private Set(ByVal value As Integer)
				privateVotes = value
			End Set
		End Property
		Private privateTags As String()
		Public Property Tags() As String()
			Get
				Return privateTags
			End Get
			Private Set(ByVal value As String())
				privateTags = value
			End Set
		End Property
	End Class
End Namespace
