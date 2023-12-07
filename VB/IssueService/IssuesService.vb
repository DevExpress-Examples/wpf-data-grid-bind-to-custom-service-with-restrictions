Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading.Tasks

Namespace InfiniteAsyncSourceAdvancedSample

    Public Module IssuesService

#Region "helpers"
        Private SyncObject As Object = New Object()

        Private AllIssues As Lazy(Of IssueData()) = New Lazy(Of IssueData())(Function()
            Dim [date] = Date.Today
            Dim rnd = New Random(0)
            Return Enumerable.Range(0, 100000).[Select](Function(i)
                [date] = [date].AddSeconds(-rnd.Next(20 * 60))
                Return New IssueData(subject:=GetSubject(), user:=GetFrom(), created:=[date], votes:=rnd.Next(100), tags:=GetTags())
            End Function).ToArray()
        End Function)

#End Region
        Public Async Function GetIssuesAsync(ByVal skip As Integer, ByVal take As Integer, ByVal sortOrder As IssueSortOrder, ByVal filter As IssueFilter) As Task(Of IssueData())
            Await Task.Delay(300).ConfigureAwait(False)
            CheckRestrictions(sortOrder, filter)
            Dim issues = SortIssues(sortOrder, AllIssues.Value)
            If filter IsNot Nothing Then issues = FilterIssues(filter, issues)
            Return issues.Skip(skip).Take(take).ToArray()
        End Function

        Private Sub CheckRestrictions(ByVal sortOrder As IssueSortOrder, ByVal filter As IssueFilter)
            If filter Is Nothing Then Return
            If(sortOrder = IssueSortOrder.Hot OrElse sortOrder = IssueSortOrder.Week) AndAlso (filter.CreatedFrom IsNot Nothing OrElse filter.CreatedTo IsNot Nothing OrElse filter.MaxVotes IsNot Nothing OrElse filter.MinVotes IsNot Nothing) Then Throw New InvalidOperationException("Restrictions violation")
            If(filter.MaxVotes IsNot Nothing OrElse filter.MinVotes IsNot Nothing) AndAlso Not(sortOrder = IssueSortOrder.VotesAscending OrElse sortOrder = IssueSortOrder.VotesDescending) Then Throw New InvalidOperationException("Restrictions violation")
        End Sub

        Public Async Function GetTagsAsync() As Task(Of String())
            Await Task.Delay(300)
            Return Tags
        End Function

#Region "filter"
        Private Function FilterIssues(ByVal filter As IssueFilter, ByVal issues As IEnumerable(Of IssueData)) As IEnumerable(Of IssueData)
            If filter.CreatedFrom IsNot Nothing Then
                issues = issues.Where(Function(x) x.Created >= filter.CreatedFrom.Value)
            End If

            If filter.CreatedTo IsNot Nothing Then
                issues = issues.Where(Function(x) x.Created < filter.CreatedTo.Value)
            End If

            If filter.MinVotes IsNot Nothing Then
                issues = issues.Where(Function(x) x.Votes >= filter.MinVotes.Value)
            End If

            If filter.MaxVotes IsNot Nothing Then
                issues = issues.Where(Function(x) x.Votes <= filter.MaxVotes.Value)
            End If

            If Not String.IsNullOrEmpty(filter.Tag) Then
                issues = issues.Where(Function(x) x.Tags.Contains(filter.Tag))
            End If

            Return issues
        End Function

#End Region
#Region "sort"
        Private Function SortIssues(ByVal sortOrder As IssueSortOrder, ByVal issues As IEnumerable(Of IssueData)) As IEnumerable(Of IssueData)
            Select Case sortOrder
                Case IssueSortOrder.Default
                    Return issues.Where(Function(x) x.Created > Date.Today.AddDays(-2) AndAlso x.Votes > 70).OrderByDescending(Function(x) x.Created)
                Case IssueSortOrder.Hot
                    Return issues.Where(Function(x) x.Created > Date.Today.AddDays(-2) AndAlso x.Votes > 70).OrderByDescending(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
                Case IssueSortOrder.Week
                    Return issues.Where(Function(x) x.Created > Date.Today.AddDays(-7) AndAlso x.Votes > 90).OrderByDescending(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
                Case IssueSortOrder.CreatedAscending
                    Return issues.OrderBy(Function(x) x.Created)
                Case IssueSortOrder.CreatedDescending
                    Return issues.OrderByDescending(Function(x) x.Created)
                Case IssueSortOrder.VotesAscending
                    Return issues.OrderBy(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
                Case IssueSortOrder.VotesDescending
                    Return issues.OrderByDescending(Function(x) x.Votes).ThenByDescending(Function(x) x.Created)
                Case Else
                    Throw New InvalidOperationException()
            End Select
        End Function
#End Region
    End Module
End Namespace
