Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports DevExpress.Xpf.Grid
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows

Namespace InfiniteAsyncSourceAdvancedSample

    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            Dim source = New InfiniteAsyncSource() With {.CustomProperties = GetCustomProperties()}
            AddHandler Unloaded, Sub(o, e) source.Dispose()
            AddHandler source.FetchRows, Sub(o, e) e.Result = FetchRowsAsync(e)
            AddHandler source.GetUniqueValues, Sub(o, e) e.Result = GetUniqueValuesAsync(e.PropertyName)
            Me.grid.ItemsSource = source
        End Sub

        Private Shared Function GetCustomProperties() As PropertyDescriptorCollection
            Dim customProperties = TypeDescriptor.GetProperties(GetType(IssueData)).Cast(Of PropertyDescriptor)().Where(Function(x) Not Equals(x.Name, "Tags")).Concat({CreateTagsProperty(), New DynamicPropertyDescriptor("Hot", GetType(String), Function(x) Nothing), New DynamicPropertyDescriptor("Week", GetType(String), Function(x) Nothing)}).ToArray()
            Return New PropertyDescriptorCollection(customProperties)
        End Function

        Private Shared Function CreateTagsProperty() As DynamicPropertyDescriptor
            Return New DynamicPropertyDescriptor(name:="Tags", propertyType:=GetType(String), getValue:=Function(x) String.Join(", ", CType(x, IssueData).Tags))
        End Function

        Private Shared Async Function FetchRowsAsync(ByVal e As FetchRowsAsyncEventArgs) As Task(Of FetchRowsResult)
            Dim sortOrder As IssueSortOrder = GetIssueSortOrder(e)
            Dim filter As IssueFilter = MakeIssueFilter(e.Filter)
            Dim take As Integer = If(e.Take, 30)
            Dim issues = Await GetIssuesAsync(skip:=e.Skip, take:=take, sortOrder:=sortOrder, filter:=filter)
            Return New FetchRowsResult(issues, hasMoreRows:=issues.Length = take)
        End Function

        Private Shared Function GetIssueSortOrder(ByVal e As FetchRowsAsyncEventArgs) As IssueSortOrder
            If e.SortOrder.Length = 0 Then Return IssueSortOrder.Default
            Dim sort = e.SortOrder.[Single]()
            Select Case sort.PropertyName
                Case "Hot"
                    If sort.Direction <> ListSortDirection.Descending Then Throw New InvalidOperationException()
                    Return IssueSortOrder.Hot
                Case "Week"
                    If sort.Direction <> ListSortDirection.Descending Then Throw New InvalidOperationException()
                    Return IssueSortOrder.Week
                Case "Created"
                    Return If(sort.Direction = ListSortDirection.Ascending, IssueSortOrder.CreatedAscending, IssueSortOrder.CreatedDescending)
                Case "Votes"
                    Return If(sort.Direction = ListSortDirection.Ascending, IssueSortOrder.VotesAscending, IssueSortOrder.VotesDescending)
                Case Else
                    Return IssueSortOrder.Default
            End Select
        End Function

        Private Shared Function MakeIssueFilter(ByVal filter As CriteriaOperator) As IssueFilter
            Return filter.Match(binary:=Function(propertyName, value, type)
                Select Case propertyName
                    Case "Votes"
                        If type = BinaryOperatorType.GreaterOrEqual Then Return New IssueFilter(minVotes:=CInt(value))
                        If type = BinaryOperatorType.LessOrEqual Then Return New IssueFilter(maxVotes:=CInt(value))
                        Throw New InvalidOperationException()
                    Case "Created"
                        If type = BinaryOperatorType.GreaterOrEqual Then Return New IssueFilter(createdFrom:=CDate(value))
                        If type = BinaryOperatorType.Less Then Return New IssueFilter(createdTo:=CDate(value))
                        Throw New InvalidOperationException()
                    Case "Tags"
                        If type = BinaryOperatorType.Equal Then Return New IssueFilter(tag:=CStr(value))
                        Throw New InvalidOperationException()
                    Case Else
                        Throw New InvalidOperationException()
                End Select
            End Function, [and]:=Function(filters) New IssueFilter(createdFrom:=filters.[Select](Function(x) x.CreatedFrom).SingleOrDefault(Function(x) x IsNot Nothing), createdTo:=filters.[Select](Function(x) x.CreatedTo).SingleOrDefault(Function(x) x IsNot Nothing), minVotes:=filters.[Select](Function(x) x.MinVotes).SingleOrDefault(Function(x) x IsNot Nothing), maxVotes:=filters.[Select](Function(x) x.MaxVotes).SingleOrDefault(Function(x) x IsNot Nothing), tag:=filters.[Select](Function(x) x.Tag).SingleOrDefault(Function(x) Not Equals(x, Nothing))), null:=Nothing)
        End Function

        Private Shared Async Function GetUniqueValuesAsync(ByVal propertyName As String) As Task(Of Object())
            If Equals(propertyName, "Tags") Then Return Await GetTagsAsync()
            Throw New InvalidOperationException()
        End Function

        Private Sub OnSearchStringToFilterCriteria(ByVal sender As Object, ByVal e As SearchStringToFilterCriteriaEventArgs)
            If Not String.IsNullOrEmpty(e.SearchString) Then e.Filter = New BinaryOperator("Tags", e.SearchString.Trim().ToLower(), BinaryOperatorType.Equal)
            e.ApplyToColumnsFilter = True
        End Sub

        Private Sub OnFilterGroupSortChanging(ByVal sender As Object, ByVal e As FilterGroupSortChangingEventArgs)
            Dim tagsFilter As CriteriaOperator = Nothing
            e.SplitColumnFilters.TryGetValue("Tags", tagsFilter)
            e.SearchString = tagsFilter.Match(binary:=Function(propertyName, value, type)
                If Not Equals(propertyName, "Tags") OrElse type <> BinaryOperatorType.Equal Then Throw New InvalidOperationException()
                Return CStr(value)
            End Function)
            Dim sortProperty = e.SortInfo.SingleOrDefault()?.PropertyName
            Dim invalidFilters = e.SplitColumnFilters.Keys.Where(Function(key) Not IsValidFilter(key, sortProperty)).ToArray()
            For Each invalidFilter In invalidFilters
                e.SplitColumnFilters.Remove(invalidFilter)
            Next
        End Sub

        Private Shared Function IsValidFilter(ByVal filterProperty As String, ByVal sortProperty As String) As Boolean
            If Equals(filterProperty, "Tags") Then Return True
            If Equals(sortProperty, "Hot") OrElse Equals(sortProperty, "Week") Then Return False
            If Equals(filterProperty, "Votes") AndAlso Not Equals(sortProperty, "Votes") Then Return False
            Return True
        End Function
    End Class
End Namespace
