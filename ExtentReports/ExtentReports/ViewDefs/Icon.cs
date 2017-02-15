using System;

namespace AventStack.ExtentReports.ViewDefs
{
    public static class Icon
    {
        public static String GetIcon(Status status)
        {
            switch (status)
            {
                case Status.Fail:
                case Status.Fatal:
                    return "cancel";
                case Status.Error:
                    return "error";
                case Status.Warning:
                    return "warning";
                case Status.Skip:
                    return "redo";
                case Status.Pass:
                    return "check_circle";
                case Status.Info:
                    return "info_outline";
                case Status.Debug:
                    return "low_priority";
                default:
                    return "help";
            }
        }
    }
}
