Imports System

Namespace InfiniteAsyncSourceAdvancedSample
    Public Class IssueFilter
        Public Sub New(Optional ByVal createdFrom? As Date = Nothing, Optional ByVal createdTo? As Date = Nothing, Optional ByVal minVotes? As Integer = Nothing, Optional ByVal maxVotes? As Integer = Nothing, Optional ByVal tag As String = Nothing)
            Me.CreatedFrom = createdFrom
            Me.CreatedTo = createdTo
            Me.MinVotes = minVotes
            Me.MaxVotes = maxVotes
            Me.Tag = tag
        End Sub

        Private privateCreatedFrom? As Date
        Public Property CreatedFrom() As Date?
            Get
                Return privateCreatedFrom
            End Get
            Private Set(ByVal value? As Date)
                privateCreatedFrom = value
            End Set
        End Property
        Private privateCreatedTo? As Date
        Public Property CreatedTo() As Date?
            Get
                Return privateCreatedTo
            End Get
            Private Set(ByVal value? As Date)
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
