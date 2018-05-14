Imports DevExpress.Data.Filtering
Imports DevExpress.Xpf.Data
Imports DevExpress.Xpf.Grid
Imports System
Imports System.ComponentModel
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Windows

Namespace InfiniteAsyncSourceAdvancedSample
    Partial Public Class MainWindow
        Inherits Window

        Public Sub New()
            InitializeComponent()

            Dim source = New InfiniteAsyncSource() With {.CustomProperties = GetCustomProperties()}
            AddHandler Unloaded, Sub(o, e)
                source.Dispose()
            End Sub

            AddHandler source.FetchRows, Sub(o, e)
                e.Result = FetchRowsAsync(e)
            End Sub

            AddHandler source.GetUniqueValues, Sub(o, e)
                e.Result = GetUniqueValuesAsync(e.PropertyName)
            End Sub

            grid.ItemsSource = source
        End Sub

        Private Shared Function GetCustomProperties() As PropertyDescriptorCollection
            Dim customProperties = TypeDescriptor.GetProperties(GetType(IssueData)).Cast(Of PropertyDescriptor)().Where(Function(x) x.Name <> "Tags").Concat( { CreateTagsProperty(), New DynamicPropertyDescriptor("Hot", GetType(String), Function(x) Nothing), New DynamicPropertyDescriptor("Week", GetType(String), Function(x) Nothing) }).ToArray()
            Return New PropertyDescriptorCollection(customProperties)
        End Function

        Private Shared Function CreateTagsProperty() As DynamicPropertyDescriptor
            Return New DynamicPropertyDescriptor(name:= "Tags", propertyType:= GetType(String), getValue:= Function(x) String.Join(", ", CType(x, IssueData).Tags))
        End Function

        Private Shared Async Function FetchRowsAsync(ByVal e As FetchRowsAsyncEventArgs) As Task(Of FetchRowsResult)
            Dim sortOrder As IssueSortOrder = GetIssueSortOrder(e)
            Dim filter As IssueFilter = MakeIssueFilter(e.Filter)

            Const pageSize As Integer = 30
            Dim issues = Await IssuesService.GetIssuesAsync(page:= e.Skip / pageSize, pageSize:= pageSize, sortOrder:= sortOrder, filter:= filter)

            Return New FetchRowsResult(issues, hasMoreRows:= issues.Length = pageSize)
        End Function

        Private Shared Function GetIssueSortOrder(ByVal e As FetchRowsAsyncEventArgs) As IssueSortOrder
            If e.SortOrder.Length = 0 Then
                Return IssueSortOrder.Default
            End If
            Dim sort = e.SortOrder.Single()
            Select Case sort.PropertyName
                Case "Hot"
                    If sort.Direction <> ListSortDirection.Descending Then
                        Throw New InvalidOperationException()
                    End If
                    Return IssueSortOrder.Hot
                Case "Week"
                    If sort.Direction <> ListSortDirection.Descending Then
                        Throw New InvalidOperationException()
                    End If
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
            Return filter.Match(binary:= Function(propertyName, value, type)
                Select Case propertyName
                    Case "Votes"
                        If type = BinaryOperatorType.GreaterOrEqual Then
                            Return New IssueFilter(minVotes:= CInt((value)))
                        End If
                        If type = BinaryOperatorType.LessOrEqual Then
                            Return New IssueFilter(maxVotes:= CInt((value)))
                        End If
                        Throw New InvalidOperationException()
                    Case "Created"
                        If type = BinaryOperatorType.GreaterOrEqual Then
                            Return New IssueFilter(createdFrom:= CDate(value))
                        End If
                        If type = BinaryOperatorType.Less Then
                            Return New IssueFilter(createdTo:= CDate(value))
                        End If
                        Throw New InvalidOperationException()
                    Case "Tags"
                        If type = BinaryOperatorType.Equal Then
                            Return New IssueFilter(tag:= CStr(value))
                        End If
                        Throw New InvalidOperationException()
                    Case Else
                        Throw New InvalidOperationException()
                End Select
            End Function, [and]:= Function(filters)
                Return New IssueFilter(createdFrom:= filters.Select(Function(x) x.CreatedFrom).SingleOrDefault(Function(x) x IsNot Nothing), createdTo:= filters.Select(Function(x) x.CreatedTo).SingleOrDefault(Function(x) x IsNot Nothing), minVotes:= filters.Select(Function(x) x.MinVotes).SingleOrDefault(Function(x) x IsNot Nothing), maxVotes:= filters.Select(Function(x) x.MaxVotes).SingleOrDefault(Function(x) x IsNot Nothing), tag:= filters.Select(Function(x) x.Tag).SingleOrDefault(Function(x) x IsNot Nothing))
            End Function, null:= Nothing)
        End Function
        Private Shared Async Function GetUniqueValuesAsync(ByVal propertyName As String) As Task(Of Object())
            If propertyName = "Tags" Then
                Return Await IssuesService.GetTagsAsync()
            End If
            Throw New InvalidOperationException()
        End Function
        Private Sub OnSearchStringToFilterCriteria(ByVal sender As Object, ByVal e As SearchStringToFilterCriteriaEventArgs)
            If Not String.IsNullOrEmpty(e.SearchString) Then
                e.Filter = New BinaryOperator("Tags", e.SearchString.Trim().ToLower(), BinaryOperatorType.Equal)
            End If
            e.ApplyToColumnsFilter = True
        End Sub

        Private Sub OnFilterGroupSortChanging(ByVal sender As Object, ByVal e As FilterGroupSortChangingEventArgs)
            Dim tagsFilter As CriteriaOperator
            e.SplitColumnFilters.TryGetValue("Tags", tagsFilter)
            e.SearchString = tagsFilter.Match(binary := Function(propertyName, value, type)
                If propertyName <> "Tags" OrElse type <> BinaryOperatorType.Equal Then
                    Throw New InvalidOperationException()
                End If
                Return CStr(value)
            End Function)

            Dim sortProperty = e.SortInfo.SingleOrDefault()?.PropertyName
            Dim invalidFilters = e.SplitColumnFilters.Keys.Where(Function(key) Not IsValidFilter(key, sortProperty)).ToArray()
            For Each invalidFilter In invalidFilters
                e.SplitColumnFilters.Remove(invalidFilter)
            Next invalidFilter
        End Sub

        Private Shared Function IsValidFilter(ByVal filterProperty As String, ByVal sortProperty As String) As Boolean
            If filterProperty = "Tags" Then
                Return True
            End If
            If sortProperty = "Hot" OrElse sortProperty = "Week" Then
                Return False
            End If
            If filterProperty = "Votes" AndAlso sortProperty <> "Votes" Then
                Return False
            End If
            Return True
        End Function
    End Class
End Namespace
