Imports System

Namespace InfiniteAsyncSourceAdvancedSample

    Public Class IssueFilter

        Private _CreatedFrom As System.DateTime?, _CreatedTo As System.DateTime?, _MinVotes As Integer?, _MaxVotes As Integer?, _Tag As String

        Public Sub New(ByVal Optional createdFrom As System.DateTime? = Nothing, ByVal Optional createdTo As System.DateTime? = Nothing, ByVal Optional minVotes As Integer? = Nothing, ByVal Optional maxVotes As Integer? = Nothing, ByVal Optional tag As String = Nothing)
            Me.CreatedFrom = createdFrom
            Me.CreatedTo = createdTo
            Me.MinVotes = minVotes
            Me.MaxVotes = maxVotes
            Me.Tag = tag
        End Sub

        Public Property CreatedFrom As System.DateTime?
            Get
                Return _CreatedFrom
            End Get

            Private Set(ByVal value As System.DateTime?)
                _CreatedFrom = value
            End Set
        End Property

        Public Property CreatedTo As System.DateTime?
            Get
                Return _CreatedTo
            End Get

            Private Set(ByVal value As System.DateTime?)
                _CreatedTo = value
            End Set
        End Property

        Public Property MinVotes As Integer?
            Get
                Return _MinVotes
            End Get

            Private Set(ByVal value As Integer?)
                _MinVotes = value
            End Set
        End Property

        Public Property MaxVotes As Integer?
            Get
                Return _MaxVotes
            End Get

            Private Set(ByVal value As Integer?)
                _MaxVotes = value
            End Set
        End Property

        Public Property Tag As String
            Get
                Return _Tag
            End Get

            Private Set(ByVal value As String)
                _Tag = value
            End Set
        End Property
    End Class
End Namespace
