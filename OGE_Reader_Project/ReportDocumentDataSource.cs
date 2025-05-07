using QuestPDF.Helpers;

public static class ReportDocumentDataSource
{

    public static ReportModel GetReportDetails()
    {

        return new ReportModel
        {

            mostActiveUser = FileStorage.mostActiveHashID,
            mostActiveUserNumberOfScans = FileStorage.mostActiveHashIDScans,

            mostActiveReader = FileStorage.mostActiveReader,
            mostActiveReaderNumberOfScans = FileStorage.mostActiveReaderEventCount,

            busiestDay = FileStorage.busiestDay,
            busiestDayAverageScans = FileStorage.busiestDayAverageScans,

            averageUniqueVisitorsPerDay = FileStorage.averageUniqueVisitorsPerDay.ToString(),

            numberOfAlerts = AlertSystem.masterAlertList.Count().ToString()

        };
    }

}