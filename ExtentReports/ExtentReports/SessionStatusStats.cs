using System.Collections.Generic;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class SessionStatusStats
    {
        public int ParentPass { get; private set; }
        public int ParentFail { get; private set; }
        public int ParentFatal { get; private set; }
        public int ParentError { get; private set; }
        public int ParentWarning { get; private set; }
        public int ParentSkip { get; private set; }
        public int ParentExceptions { get; private set; }

        public int ParentCount
        {
            get
            {
                return ParentPass +
                    ParentFail +
                    ParentFatal +
                    ParentError +
                    ParentWarning +
                    ParentSkip;
            }
        }

        public int ChildPass { get; private set; }
        public int ChildFail { get; private set; }
        public int ChildFatal { get; private set; }
        public int ChildError { get; private set; }
        public int ChildWarning { get; private set; }
        public int ChildSkip { get; private set; }
        public int ChildInfo { get; private set; }
        public int ChildExceptions { get; private set; }

        public int ChildCount
        {
            get
            {
                return ChildPass +
                    ChildFail +
                    ChildFatal +
                    ChildError +
                    ChildWarning +
                    ChildSkip +
                    ChildInfo;
            }
        }

        public int GrandChildPass { get; private set; }
        public int GrandChildFail { get; private set; }
        public int GrandChildFatal { get; private set; }
        public int GrandChildError { get; private set; }
        public int GrandChildWarning { get; private set; }
        public int GrandChildSkip { get; private set; }
        public int GrandChildInfo { get; private set; }
        public int GrandChildExceptions { get; private set; }

        public int GrandChildCount
        {
            get
            {
                return GrandChildPass +
                    GrandChildFail +
                    GrandChildFatal +
                    GrandChildError +
                    GrandChildWarning +
                    GrandChildSkip +
                    GrandChildInfo;
            }
        }

        private List<Test> _testCollection;
        private AnalysisStrategy _strategy;

        public SessionStatusStats(AnalysisStrategy strategy)
        {
            _strategy = strategy;
        }

        internal void Refresh(List<Test> testCollection)
        {
            Reset();

            _testCollection = testCollection;
            UpdateCounts();
        }

        private void UpdateCounts()
        {
            _testCollection.ForEach(x => ExtractCounts(x));
        }

        private void Reset()
        {
            ParentPass = 0;
            ParentFail = 0;
            ParentFatal = 0;
            ParentError = 0;
            ParentWarning = 0;
            ParentSkip = 0;
            ParentExceptions = 0;

            ChildPass = 0;
            ChildFail = 0;
            ChildFatal = 0;
            ChildError = 0;
            ChildWarning = 0;
            ChildSkip = 0;
            ChildInfo = 0;
            ChildExceptions = 0;

            GrandChildPass = 0;
            GrandChildFail = 0;
            GrandChildFatal = 0;
            GrandChildError = 0;
            GrandChildWarning = 0;
            GrandChildSkip = 0;
            GrandChildInfo = 0;
            GrandChildExceptions = 0;
        }

        private void ExtractCounts(Test test)
        {
            if (test.IsBehaviorDrivenType || (test.HasChildren() && test.NodeContext().Get(0).IsBehaviorDrivenType))
            {
                ExtractBddTestCounts(test);
                return;
            }

            if (_strategy == AnalysisStrategy.Class)
            {
                ExtractStandardTestCountsClassStrategy(test);
                return;
            }

            ExtractStandardTestCountsTestStrategy(test);
        }

        private void ExtractBddTestCounts(Test test)
        {
            IncrementItemCountByStatus(ItemLevel.Parent, test.Status);

            if (test.HasChildren())
            {
                test.NodeContext().GetAllItems().ForEach(x =>
                {
                    IncrementItemCountByStatus(ItemLevel.Child, x.Status);

                    if (x.HasChildren())
                    {
                        x.NodeContext().GetAllItems().ForEach(n =>
                        {
                            IncrementItemCountByStatus(ItemLevel.GrandChild, n.Status);
                        });
                    }
                });
            }
        }

        private void ExtractStandardTestCountsClassStrategy(Test test)
        {
            if (test.HasLog())
                test.LogContext().GetAllItems().ForEach(l => IncrementItemCountByStatus(ItemLevel.GrandChild, l.Status));

            if (test.HasChildren())
            {
                IncrementItemCountByStatus(ItemLevel.Parent, test.Status);
                UpdateGroupCountsForChildrenRecursive(test);
            }
            else
            {
                IncrementItemCountByStatus(ItemLevel.Child, test.Status);
            }
        }

        private void UpdateGroupCountsForChildrenRecursive(Test test)
        {
            if (test.HasLog())
            {
                test.LogContext().GetAllItems().ForEach(l => {
                    IncrementItemCountByStatus(ItemLevel.GrandChild, l.Status);
                });
            }

            if (test.HasChildren())
            {
                test.NodeContext().GetAllItems().ForEach(n => {
                    UpdateGroupCountsForChildrenRecursive(n);
                });
            }
            else
            {
                IncrementItemCountByStatus(ItemLevel.Child, test.Status);
            }
        }

        private void ExtractStandardTestCountsTestStrategy(Test test)
        {
            if (!test.HasChildren())
            {
                IncrementItemCountByStatus(ItemLevel.Parent, test.Status);
                test.LogContext().GetAllItems().ForEach(x =>
                {
                    IncrementItemCountByStatus(ItemLevel.Child, x.Status);
                });
            }
            else
            {
                test.NodeContext().GetAllItems().ForEach(x => ExtractStandardTestCountsTestStrategy(x));
            }
        }


        private void IncrementItemCountByStatus(ItemLevel level, Status status)
        {
            switch (level)
            {
                case ItemLevel.Parent:
                    IncrementParentCount(status);
                    break;

                case ItemLevel.Child:
                    IncrementChildCount(status);
                    break;

                case ItemLevel.GrandChild:
                    IncrementGrandChildCount(status);
                    break;

                default:
                    break;
            }
        }

        private void IncrementParentCount(Status status)
        {
            switch (status)
            {
                case Status.Pass:
                    ParentPass++;
                    return;

                case Status.Error:
                    ParentError++;
                    break;

                case Status.Fail:
                    ParentFail++;
                    break;

                case Status.Fatal:
                    ParentFatal++;
                    break;

                case Status.Skip:
                    ParentSkip++;
                    break;

                case Status.Warning:
                    ParentWarning++;
                    break;

                default:
                    break;
            }

            ParentExceptions++;
        }

        private void IncrementChildCount(Status status)
        {
            switch (status)
            {
                case Status.Pass:
                    ChildPass++;
                    break;

                case Status.Info:
                    ChildInfo++;
                    break;

                case Status.Error:
                    ChildError++;
                    break;

                case Status.Fail:
                    ChildFail++;
                    break;

                case Status.Fatal:
                    ChildFatal++;
                    break;

                case Status.Skip:
                    ChildSkip++;
                    break;

                case Status.Warning:
                    ChildWarning++;
                    break;

                default:
                    break;
            }

            if (status != Status.Pass && status != Status.Info)
                ChildExceptions++;
        }

        private void IncrementGrandChildCount(Status status)
        {
            switch (status)
            {
                case Status.Pass:
                    GrandChildPass++;
                    break;

                case Status.Info:
                    GrandChildInfo++;
                    break;

                case Status.Error:
                    GrandChildError++;
                    break;

                case Status.Fail:
                    GrandChildFail++;
                    break;

                case Status.Fatal:
                    GrandChildFatal++;
                    break;

                case Status.Skip:
                    GrandChildSkip++;
                    break;

                case Status.Warning:
                    GrandChildWarning++;
                    break;

                default:
                    break;
            }

            if (status != Status.Pass && status != Status.Info)
                GrandChildExceptions++;
        }

        private enum ItemLevel
        {
            Parent,
            Child,
            GrandChild
        }
    }
}