using System;

namespace InfiniteAsyncSourceAdvancedSample {
    public class IssueFilter {
        public IssueFilter(DateTime? createdFrom = null, DateTime? createdTo = null, int? minVotes = null, int? maxVotes = null, string tag = null) {
            CreatedFrom = createdFrom;
            CreatedTo = createdTo;
            MinVotes = minVotes;
            MaxVotes = maxVotes;
            Tag = tag;
        }

        public DateTime? CreatedFrom { get; private set; }
        public DateTime? CreatedTo { get; private set; }
        public int? MinVotes { get; private set; }
        public int? MaxVotes { get; private set; }
        public string Tag { get; private set; }
    }
}
