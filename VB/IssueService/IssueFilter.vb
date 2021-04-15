Imports System

Namespace InfiniteAsyncSourceAdvancedSample
	Public Class IssueFilter
		Public Sub New(Optional ByVal createdFrom? As DateTime = Nothing, Optional ByVal createdTo? As DateTime = Nothing, Optional ByVal minVotes? As Integer = Nothing, Optional ByVal maxVotes? As Integer = Nothing, Optional ByVal tag As String = Nothing)
			Me.CreatedFrom = createdFrom
			Me.CreatedTo = createdTo
			Me.MinVotes = minVotes
			Me.MaxVotes = maxVotes
			Me.Tag = tag
		End Sub

		Private privateCreatedFrom? As DateTime
		Public Property CreatedFrom() As DateTime?
			Get
				Return privateCreatedFrom
			End Get
			Private Set(ByVal value? As DateTime)
				privateCreatedFrom = value
			End Set
		End Property
		Private privateCreatedTo? As DateTime
		Public Property CreatedTo() As DateTime?
			Get
				Return privateCreatedTo
			End Get
			Private Set(ByVal value? As DateTime)
				privateCreatedTo = value
			End Set
		End Property
		Private privateMinVotes? As Integer
		Public Property MinVotes() As Integer?
			Get
				Return privateMinVotes
			End Get
			Private Set(ByVal value? As Integer)
				privateMinVotes = value
			End Set
		End Property
		Private privateMaxVotes? As Integer
		Public Property MaxVotes() As Integer?
			Get
				Return privateMaxVotes
			End Get
			Private Set(ByVal value? As Integer)
				privateMaxVotes = value
			End Set
		End Property
		Private privateTag As String
		Public Property Tag() As String
			Get
				Return privateTag
			End Get
			Private Set(ByVal value As String)
				privateTag = value
			End Set
		End Property
	End Class
End Namespace
