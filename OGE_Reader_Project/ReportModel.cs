public class ReportModel
{

    // Dashboard Data
    public string mostActiveUser { get; set; }
    public string mostActiveUserNumberOfScans { get; set; }

    public string mostActiveReader { get; set; }
    public string mostActiveReaderNumberOfScans { get; set; }

    public string busiestDay { get; set; }
    public string busiestDayAverageScans { get; set; }

    public string averageUniqueVisitorsPerDay { get; set; }
    

    // Alert System Data
    public string numberOfAlerts { get; set; }


}