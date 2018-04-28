using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InfiniteAsyncSourceAdvancedSample {
    public static class IssuesService {
        #region helpers
        static object SyncObject = new object();
        static Lazy<IssueData[]> AllIssues = new Lazy<IssueData[]>(() => {
            var date = DateTime.Today;
            var rnd = new Random(0);
            return Enumerable.Range(0, 100000)
                .Select(i => {
                    date = date.AddSeconds(-rnd.Next(20 * 60));
                    return new IssueData(
                        subject: OutlookDataGenerator.GetSubject(),
                        user: OutlookDataGenerator.GetFrom(),
                        created: date,
                        votes: rnd.Next(100),
                        tags: OutlookDataGenerator.GetTags());
                }).ToArray();
        });
        #endregion

        public async static Task<IssueData[]> GetIssuesAsync(int page, int pageSize, IssueSortOrder sortOrder, IssueFilter filter) {
            await Task.Delay(300);
            CheckRestrictions(sortOrder, filter);
            var issues = SortIssues(sortOrder, AllIssues.Value);
            if(filter != null)
                issues = FilterIssues(filter, issues);
            return issues.Skip(page * pageSize).Take(pageSize).ToArray();
        }

        static void CheckRestrictions(IssueSortOrder sortOrder, IssueFilter filter) {
            if(filter == null)
                return;
            if((sortOrder == IssueSortOrder.Hot || sortOrder == IssueSortOrder.Week)
                && (filter.CreatedFrom != null || filter.CreatedTo != null || filter.MaxVotes != null || filter.MinVotes != null))
                throw new InvalidOperationException("Restrictions violation");

            if((filter.MaxVotes != null || filter.MinVotes != null)
                && !(sortOrder == IssueSortOrder.VotesAscending || sortOrder == IssueSortOrder.VotesDescending))
                throw new InvalidOperationException("Restrictions violation");
        }

        public async static Task<string[]> GetTagsAsync() {
            await Task.Delay(300);
            return OutlookDataGenerator.Tags;
        }


        #region filter
        static IEnumerable<IssueData> FilterIssues(IssueFilter filter, IEnumerable<IssueData> issues) {
            if(filter.CreatedFrom != null) {
                issues = issues.Where(x => x.Created >= filter.CreatedFrom.Value);
            }
            if(filter.CreatedTo != null) {
                issues = issues.Where(x => x.Created < filter.CreatedTo.Value);
            }
            if(filter.MinVotes != null) {
                issues = issues.Where(x => x.Votes >= filter.MinVotes.Value);
            }
            if(filter.MaxVotes != null) {
                issues = issues.Where(x => x.Votes <= filter.MaxVotes.Value);
            }
            if(!string.IsNullOrEmpty(filter.Tag)) {
                issues = issues.Where(x => x.Tags.Contains(filter.Tag));
            }
            return issues;
        }
        #endregion

        #region sort
        static IEnumerable<IssueData> SortIssues(IssueSortOrder sortOrder, IEnumerable<IssueData> issues) {
            switch(sortOrder) {
            case IssueSortOrder.Default:
                return issues
                    .Where(x => x.Created > DateTime.Today.AddDays(-2) && x.Votes > 70)
                    .OrderByDescending(x => x.Created);
            case IssueSortOrder.Hot:
                return issues
                    .Where(x => x.Created > DateTime.Today.AddDays(-2) && x.Votes > 70)
                    .OrderByDescending(x => x.Votes)
                    .ThenByDescending(x => x.Created);
            case IssueSortOrder.Week:
                return issues
                    .Where(x => x.Created > DateTime.Today.AddDays(-7) && x.Votes > 90)
                    .OrderByDescending(x => x.Votes)
                    .ThenByDescending(x => x.Created);

            case IssueSortOrder.CreatedAscending:
                return issues.OrderBy(x => x.Created);
            case IssueSortOrder.CreatedDescending:
                return issues.OrderByDescending(x => x.Created);

            case IssueSortOrder.VotesAscending:
                return issues
                    .OrderBy(x => x.Votes)
                    .ThenByDescending(x => x.Created);
            case IssueSortOrder.VotesDescending:
                return issues
                    .OrderByDescending(x => x.Votes)
                    .ThenByDescending(x => x.Created);
            default:
                throw new InvalidOperationException();
            }
        } 
        #endregion
    }
}
