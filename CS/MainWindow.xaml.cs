using DevExpress.Data.Filtering;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InfiniteAsyncSourceAdvancedSample {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            var source = new InfiniteAsyncSource() {
                CustomProperties = GetCustomProperties()
            };

            Unloaded += (o, e) => {
                source.Dispose();
            };

            source.FetchRows += (o, e) => {
                e.Result = FetchRowsAsync(e);
            };

            source.GetUniqueValues += (o, e) => {
                e.Result = GetUniqueValuesAsync(e.PropertyName);
            };

            grid.ItemsSource = source;
        }

        static PropertyDescriptorCollection GetCustomProperties() {
            var customProperties = TypeDescriptor.GetProperties(typeof(IssueData))
                .Cast<PropertyDescriptor>()
                .Where(x => x.Name != "Tags")
                .Concat(new[] {
                    CreateTagsProperty(),
                    new DynamicPropertyDescriptor("Hot", typeof(string), x => null),
                    new DynamicPropertyDescriptor("Week", typeof(string), x => null)
                })
                .ToArray();
            return new PropertyDescriptorCollection(customProperties);
        }

        static DynamicPropertyDescriptor CreateTagsProperty() {
            return new DynamicPropertyDescriptor(
                name: "Tags",
                propertyType: typeof(string),
                getValue: x => string.Join(", ", ((IssueData)x).Tags));
        }

        static async Task<FetchRowsResult> FetchRowsAsync(FetchRowsAsyncEventArgs e) {
            IssueSortOrder sortOrder = GetIssueSortOrder(e);
            IssueFilter filter = MakeIssueFilter(e.Filter);

            int take = e.Take ?? 30;
            var issues = await IssuesService.GetIssuesAsync(
                skip: e.Skip,
                take: take,
                sortOrder: sortOrder,
                filter: filter);

            return new FetchRowsResult(issues, hasMoreRows: issues.Length == take);
        }

        static IssueSortOrder GetIssueSortOrder(FetchRowsAsyncEventArgs e) {
            if(e.SortOrder.Length == 0)
                return IssueSortOrder.Default;
            var sort = e.SortOrder.Single();
            switch(sort.PropertyName) {
                case "Hot":
                    if(sort.Direction != ListSortDirection.Descending)
                        throw new InvalidOperationException();
                    return IssueSortOrder.Hot;
                case "Week":
                    if(sort.Direction != ListSortDirection.Descending)
                        throw new InvalidOperationException();
                    return IssueSortOrder.Week;
                case "Created":
                    return sort.Direction == ListSortDirection.Ascending
                        ? IssueSortOrder.CreatedAscending
                        : IssueSortOrder.CreatedDescending;
                case "Votes":
                    return sort.Direction == ListSortDirection.Ascending
                        ? IssueSortOrder.VotesAscending
                        : IssueSortOrder.VotesDescending;
                default:
                    return IssueSortOrder.Default;
            }
        }

        static IssueFilter MakeIssueFilter(CriteriaOperator filter) {
            return filter.Match(
                binary: (propertyName, value, type) => {
                    switch(propertyName) {
                        case "Votes":
                            if(type == BinaryOperatorType.GreaterOrEqual)
                                return new IssueFilter(minVotes: (int)value);
                            if(type == BinaryOperatorType.LessOrEqual)
                                return new IssueFilter(maxVotes: (int)value);
                            throw new InvalidOperationException();
                        case "Created":
                            if(type == BinaryOperatorType.GreaterOrEqual)
                                return new IssueFilter(createdFrom: (DateTime)value);
                            if(type == BinaryOperatorType.Less)
                                return new IssueFilter(createdTo: (DateTime)value);
                            throw new InvalidOperationException();
                        case "Tags":
                            if(type == BinaryOperatorType.Equal)
                                return new IssueFilter(tag: (string)value);
                            throw new InvalidOperationException();
                        default:
                            throw new InvalidOperationException();
                    }
                },
                and: filters => {
                    return new IssueFilter(
                        createdFrom: filters.Select(x => x.CreatedFrom).SingleOrDefault(x => x != null),
                        createdTo: filters.Select(x => x.CreatedTo).SingleOrDefault(x => x != null),
                        minVotes: filters.Select(x => x.MinVotes).SingleOrDefault(x => x != null),
                        maxVotes: filters.Select(x => x.MaxVotes).SingleOrDefault(x => x != null),
                        tag: filters.Select(x => x.Tag).SingleOrDefault(x => x != null)
                    );
                },
                @null: default(IssueFilter)
            );
        }
        static async Task<object[]> GetUniqueValuesAsync(string propertyName) {
            if(propertyName == "Tags")
                return await IssuesService.GetTagsAsync();
            throw new InvalidOperationException();
        }
        void OnSearchStringToFilterCriteria(object sender, SearchStringToFilterCriteriaEventArgs e) {
            if(!string.IsNullOrEmpty(e.SearchString))
                e.Filter = new BinaryOperator("Tags", e.SearchString.Trim().ToLower(), BinaryOperatorType.Equal);
            e.ApplyToColumnsFilter = true;
        }

        void OnFilterGroupSortChanging(object sender, FilterGroupSortChangingEventArgs e) {
            e.SplitColumnFilters.TryGetValue("Tags", out CriteriaOperator tagsFilter);
            e.SearchString = tagsFilter.Match(binary : (propertyName, value, type) => {
                if(propertyName != "Tags" || type != BinaryOperatorType.Equal)
                    throw new InvalidOperationException();
                return (string)value;
            });

            var sortProperty = e.SortInfo.SingleOrDefault()?.PropertyName;
            var invalidFilters = e.SplitColumnFilters.Keys
                .Where(key => !IsValidFilter(key, sortProperty))
                .ToArray();
            foreach(var invalidFilter in invalidFilters)
                e.SplitColumnFilters.Remove(invalidFilter);
        }

        static bool IsValidFilter(string filterProperty, string sortProperty) {
            if(filterProperty == "Tags")
                return true;
            if(sortProperty == "Hot" || sortProperty == "Week")
                return false;
            if(filterProperty == "Votes" && sortProperty != "Votes")
                return false;
            return true;
        }
    }
}
