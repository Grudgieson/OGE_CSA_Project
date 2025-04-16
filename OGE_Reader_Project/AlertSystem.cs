using System.Collections.Generic;
using MudBlazor;

public class AlertSystem
{

    // Holds every alert detected by the system
    public static List<DataAlert> masterAlertList = new List<DataAlert>();

    List<DataAlert> anomolyList = new List<DataAlert>();

    
    // Calls all the methods that scan for anomolies
    public static void ScanForAnomoly()
    {

        LowUserActivity();
        OffHoursScan();

    }
    static void LowUserActivity()
    {

        // Find all users who have less than 10 total events and report the anomoly
        foreach(var entry in FileStorage.eventDictionaryFilteredHashID)
        {

            if(entry.Value.Count() <= 9)
            {

                masterAlertList.Add(new DataAlert("Low User Activity", $"User {entry.Key} has a total of {entry.Value.Count()} Scans"));

            }

        }

    }
    static void OffHoursScan()
    {

        foreach(var reader in FileStorage.theMasterList)
        {

            if(reader.GetEventTime().TimeOfDay <= new TimeSpan(0, 0, 0) && reader.GetEventTime().TimeOfDay >= new TimeSpan(6, 0, 0))
            {

                masterAlertList.Add(new DataAlert("Off Hours Scan", $"{reader.GetEventHashID} scanned in at {reader.GetEventTime().TimeOfDay} on {reader.GetEventTime().Date}"));

            }

        }

    }


    public class DataAlert
    {

        public string alertType;
        public string alertDescription;

        public DataAlert()
        {

            alertType = "";
            alertDescription = "";

        }
        public DataAlert(string newAlertType, string newAlertDescription)
        {

            alertType = newAlertType;
            alertDescription = newAlertDescription;

        }

    }

}