using System.Collections.Generic;
using MudBlazor;

public static class AlertSystem
{

    // Adjustable Threashold
    public static int lowUserActivityThreashold = 9;
    public static int duplicateThreashold = 50;
    public static int numberOfScansForUsageAlert = 1500;


    // Holds every alert detected by the system
    public static List<DataAlert> masterAlertList = new List<DataAlert>();

    static List<DataAlert> anomolyList = new List<DataAlert>();

    
    // Calls all the methods that scan for anomolies
    public static void ScanForAnomoly()
    {

        masterAlertList = new List<DataAlert>();
        LowUserActivity();
        UsageAlert();

    }
    static void LowUserActivity()
    {

        // Find all users who have less than 10 total events and report the anomoly
        foreach(var entry in FileStorage.eventDictionaryFilteredHashID)
        {

            if(entry.Value.Count() <= lowUserActivityThreashold)
            {

                masterAlertList.Add(new DataAlert("Low User Activity", $"User {entry.Key} has a total of {entry.Value.Count()} Scans", Severity.Info));

            }

        }

    }
    public static void CheckForDuplicateAlert(Dictionary<string, int> duplicateDictionary)
    {

        foreach(var dup in duplicateDictionary)
        {

            if(dup.Value >= duplicateThreashold)
            {

                masterAlertList.Add(new DataAlert("High Number Duplicates", $"Reader {dup.Key} has reported {dup.Value} exact duplicate scan entries.", Severity.Warning));


            }

        }

    }
    static void UsageAlert()
    {

        foreach(var entry in FileStorage.eventDictionary)
        {

            if(entry.Value.Count >= numberOfScansForUsageAlert)
            {

                masterAlertList.Add(new DataAlert("High Usage", $"Reader {entry.Key} at {entry.Value[0].GetEventDescription()} has captured {entry.Value.Count} scans. Maintenance may be required soon.", Severity.Error));


            }

        }

    }
    static void CheckForGhost()
    {

        

    }
    static void ActivitySpike()
    {

        

    }
    static void OffHoursScan()
    {

        foreach(var reader in FileStorage.theMasterList)
        {

            if(reader.GetEventTime().TimeOfDay >= new TimeSpan(0, 0, 0) && reader.GetEventTime().TimeOfDay <= new TimeSpan(6, 0, 0))
            {

                masterAlertList.Add(new DataAlert("Off Hours Scan", $"{reader.GetEventHashID()} scanned in at {reader.GetEventTime():h:mm tt} on {reader.GetEventTime().Date:d MMM yyyy}", Severity.Info));

            }

        }

    }

    public class DataAlert
    {

        public string alertType;
        public string alertDescription;
        public Severity alertSeverity;

        public DataAlert()
        {

            alertType = "";
            alertDescription = "";
            alertSeverity = 0;

        }
        public DataAlert(string newAlertType, string newAlertDescription, Severity newAlertSeverity)
        {

            alertType = newAlertType;
            alertDescription = newAlertDescription;
            alertSeverity = newAlertSeverity;

        }

    }

}