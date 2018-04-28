using System;

namespace InfiniteAsyncSourceAdvancedSample {
    public class IssueData {
        public IssueData(string subject, string user, DateTime created, int votes, string[] tags) {
            Subject = subject;
            User = user;
            Created = created;
            Votes = votes;
            Tags = tags;
        }
        public string Subject { get; private set; }
        public string User { get; private set; }
        public DateTime Created { get; private set; }
        public int Votes { get; private set; }
        public string[] Tags{ get; private set; }
    }
}
