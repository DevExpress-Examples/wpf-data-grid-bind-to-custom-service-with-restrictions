Imports System

Namespace InfiniteAsyncSourceAdvancedSample

    Public Class IssueData

        Private _Subject As String, _User As String, _Created As DateTime, _Votes As Integer, _Tags As String()

        Public Sub New(ByVal subject As String, ByVal user As String, ByVal created As System.DateTime, ByVal votes As Integer, ByVal tags As String())
            Me.Subject = subject
            Me.User = user
            Me.Created = created
            Me.Votes = votes
            Me.Tags = tags
        End Sub

        Public Property Subject As String
            Get
                Return _Subject
            End Get

            Private Set(ByVal value As String)
                _Subject = value
            End Set
        End Property

        Public Property User As String
            Get
                Return _User
            End Get

            Private Set(ByVal value As String)
                _User = value
            End Set
        End Property

        Public Property Created As DateTime
            Get
                Return _Created
            End Get

            Private Set(ByVal value As DateTime)
                _Created = value
            End Set
        End Property

        Public Property Votes As Integer
            Get
                Return _Votes
            End Get

            Private Set(ByVal value As Integer)
                _Votes = value
            End Set
        End Property

        Public Property Tags As String()
            Get
                Return _Tags
            End Get

            Private Set(ByVal value As String())
                _Tags = value
            End Set
        End Property
    End Class
End Namespace
