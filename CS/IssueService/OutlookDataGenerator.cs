using System;
using System.Linq;

namespace InfiniteAsyncSourceAdvancedSample {
    public static class OutlookDataGenerator {
        static Random rnd = new Random();
        static string[] Subjects = new string[] { "Developer Express MasterView. Integrating the control into an Accounting System.",
                                                "Web Edition: Data Entry Page. There is an issue with date validation.",
                                                "Payables Due Calculator is ready for testing.",
                                                "Web Edition: Search Page is ready for testing.",
                                                "Main Menu: Duplicate Items. Somebody has to review all menu items in the system.",
                                                "Receivables Calculator. Where can I find the complete specs?",
                                                "Ledger: Inconsistency. Please fix it.",
                                                "Receivables Printing module is ready for testing.",
                                                "Screen Redraw. Somebody has to look at it.",
                                                "Email System. What library are we going to use?",
                                                "Cannot add new vendor. This module doesn't work!",
                                                "History. Will we track sales history in our system?",
                                                "Main Menu: Add a File menu. File menu item is missing.",
                                                "Currency Mask. The current currency mask in completely unusable.",
                                                "Drag & Drop operations are not available in the scheduler module.",
                                                "Data Import. What database types will we support?",
                                                "Reports. The list of incomplete reports.",
                                                "Data Archiving. We still don't have this features in our application.",
                                                "Email Attachments. Is it possible to add multiple attachments? I haven't found a way to do this.",
                                                "Check Register. We are using different paths for different modules.",
                                                "Data Export. Our customers asked us for export to Microsoft Excel"};

        static readonly string[] Users = new string[] {
            "Peter Dolan",
            "Ryan Fischer",
            "Richard Fisher",
            "Tom Hamlett",
            "Mark Hamilton",
            "Steve Lee",
            "Jimmy Lewis",
            "Jeffrey W McClain",
            "Andrew Miller",
            "Dave Murrel",
            "Bert Parkins",
            "Mike Roller",
            "Ray Shipman",
            "Paul Bailey",
            "Brad Barnes",
            "Carl Lucas",
            "Jerry Campbell",
        };
        public static readonly string[] Tags = new string[] {
            "wpf",
            "c#",
            ".net",
            "java",
            "php",
            "android",
            "python",
            "jquery",
            "html",
            "c++",
            "ios",
            "css",
            "mysql",
            "sql",
            "asp.net",
            "ruby-on-rails",
            "objective-c",
            "c",
            "arrays",
            "angularjs",
            "r",
            "json",
            "sql-server",
            "node.js",
            "iphone",
            "ruby",
            "swift",
            "regex",
            "ajax",
            "xml",
            "javascript",
            "winforms"
        };

        public static string GetSubject() {
            return Subjects[rnd.Next(Subjects.Length - 1)];
        }

        public static string GetFrom() {
            return Users[rnd.Next(Users.Length)];
        }

        public static string[] GetTags() {
            return Enumerable.Range(0, rnd.Next(4) + 1)
                .Select(i => Tags[rnd.Next(Tags.Length)])
                .ToArray();
        }
    }
}
