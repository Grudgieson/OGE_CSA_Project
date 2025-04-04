using MudBlazor;

public class FileStorage
{

    public static string FileName { get; set; } = "";
    public static string ErrorMessage { get; set; } = "";

    public static int masterListCount = 0;


    // Dictionary of ReaderEvents that uses the Unique ID as a key
    public static Dictionary<string, List<ReaderEvent>> eventDictionary = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredByTime = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredByDay = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredHashID = new Dictionary<string, List<ReaderEvent>>();

    // Highlight Variables
    public static string mostActiveHashID = "N/A";
    public static string mostActiveReader = "N/A";
    public static string busiestDay = "N/A";
    public static int averageUniqueVisitorsPerDay = 0;

    // Chart Variables
    public static ChartOptions Options = new ChartOptions();
    public static AxisChartOptions axisOptions = new AxisChartOptions();
    
    public static List<ChartSeries> Series = new List<ChartSeries>();
    public static string[] XAxisLabels = Array.Empty<string>();

    public static string chartFilter = "";

    // Alert System Variables
    public static IEnumerable<DataAlert> alertsList = new List<DataAlert>()
    {
        new DataAlert("Impossible Movement", "User detecting using two readers at the same time"),
        new DataAlert("Off Hours Access", "User scanned into this place at 2:00am on 6/23/2024"),
        new DataAlert("Missed Exit Scan", "User scanned in to this place at this time but didn't scan out"),
        new DataAlert("Ghost User", "User only scan 2 times in the past time"),
        new DataAlert("Ghost User", "User only scan 2 times in the past time"),
        new DataAlert("Ghost User", "User only scan 2 times in the past time"),
    };


    public static async Task ProcessFile()
    {

        // Create a stream reader that reads from the stream when a file is uploaded & make an masterList to keep track
        var sr = new StreamReader(FileName);
        var masterList = new List<ReaderEvent>();

        // Check every line of the stream in StreamReader
        string? line = sr.ReadLine();
        while((line = sr.ReadLine()) != null) // Until there is no remaining lines left
        {   

            // Break each line into an array with Reader Data
            string[] data = line.Split(",");

            // Create a new Reader Event using the string array of the current line
            masterList.Add(new ReaderEvent(
                data[0],
                data[1],
                data[2],
                data[3],
                data[4],
                data[5]
            ));

        }

        masterListCount = masterList.Count;

        // Returns a dictionary with Readers and their Respective Events
        eventDictionary = OrganizeDictionData(masterList);
        eventDictionaryFilteredByTime = OrganizeDictionByDayOfTheWeek(masterList);
        eventDictionaryFilteredByDay = OrganizeDictionByDay(masterList);
        eventDictionaryFilteredHashID = OrganizeDictionByHashID(masterList);

        // Get highlights after all the dictionaries have been organized
        GetMostActiveHashID();
        GetMostActiveReader();
        GetBusiestDay();
        GetAverageUniqueVisitorsPerDay();

        // Get Data for graph
        GetChartDataForDashboard();

        // Close the file stream of the file
        sr.Close();

    }

    public static Dictionary<string, List<ReaderEvent>> OrganizeDictionData(List<ReaderEvent> rawData)
    {

        Dictionary<string, List<ReaderEvent>> resultDict = new Dictionary<string, List<ReaderEvent>>();

        // Goes through each recorded ReaderEvent in the raw Data list
        foreach(ReaderEvent rawReaderEvent in rawData)
        {

            // Get the reader's ID that captured the event and add or create a section in the dictionary accordingly
            var readerID = rawReaderEvent.GetEventUniqueID();
            if(!resultDict.ContainsKey(readerID))
            {

                resultDict[readerID] = new List<ReaderEvent>(); // Create a entry when unique readerID is found

            }
            resultDict[readerID].Add(rawReaderEvent); // Add an event to the respective reader

        }

        return resultDict;
        
    }
    public static Dictionary<string, List<ReaderEvent>> OrganizeDictionByDay(List<ReaderEvent> masterList)
    {

        Dictionary<string, List<ReaderEvent>> resultDict = new Dictionary<string, List<ReaderEvent>>();

        // Goes through each recorded ReaderEvent in the raw Data list
        foreach(ReaderEvent rawReaderEvent in masterList)
        {

            // Get the reader's ID that captured the event and add or create a section in the dictionary accordingly
            string dayOfEvent = rawReaderEvent.GetEventTime().Date.ToString("MMM dd");
            if(!resultDict.ContainsKey(dayOfEvent))
            {

                resultDict[dayOfEvent] = new List<ReaderEvent>(); // Create a entry when an event occured on a unique time is found

            }
            resultDict[dayOfEvent].Add(rawReaderEvent); // Add an event to the respective reader

        }

        return resultDict;

    }
    public static Dictionary<string, List<ReaderEvent>> OrganizeDictionByDayOfTheWeek(List<ReaderEvent> masterList)
    {

        Dictionary<string, List<ReaderEvent>> resultDict = new Dictionary<string, List<ReaderEvent>>();

        // Goes through each recorded ReaderEvent in the raw Data list
        foreach(ReaderEvent rawReaderEvent in masterList)
        {

            // Get the reader's ID that captured the event and add or create a section in the dictionary accordingly
            string dayOfEvent = rawReaderEvent.GetEventTime().DayOfWeek.ToString();
            if(!resultDict.ContainsKey(dayOfEvent))
            {

                resultDict[dayOfEvent] = new List<ReaderEvent>(); // Create a entry when an event occured on a unique time is found

            }
            resultDict[dayOfEvent].Add(rawReaderEvent); // Add an event to the respective reader

        }

        return resultDict;

    }
    public static Dictionary<string, List<ReaderEvent>> OrganizeDictionByHashID(List<ReaderEvent> masterList)
    {

        Dictionary<string, List<ReaderEvent>> resultDict = new Dictionary<string, List<ReaderEvent>>();

        // Goes through each recorded ReaderEvent in the raw Data list
        foreach(ReaderEvent rawReaderEvent in masterList)
        {

            // Get the reader's ID that captured the event and add or create a section in the dictionary accordingly
            string userHashID = rawReaderEvent.GetEventHashID();
            if(!resultDict.ContainsKey(userHashID))
            {

                resultDict[userHashID] = new List<ReaderEvent>(); // Create a entry when an event occured on a unique time is found

            }
            resultDict[userHashID].Add(rawReaderEvent); // Add an event to the respective reader

        }

        return resultDict;

    }

    public static void GetMostActiveHashID()
    {

        // Sets the currentHighestActiveID to the first ID in the dictionary
        string currentHighestActiveID = eventDictionaryFilteredHashID.Keys.First();

        // Check each registered user in the dictionary and if the curret user has a greater number of invoked events compared to the current highest set the current highest to that user
        foreach(var user in eventDictionaryFilteredHashID)
        {

            if(user.Value.Count >= eventDictionaryFilteredHashID[currentHighestActiveID].Count)
            {

                currentHighestActiveID = user.Key;

            }

        }

        mostActiveHashID = currentHighestActiveID;
        
    }
    public static void GetMostActiveReader()
    {

        // Sets the currentHighestActiveReader to the first ID in the dictionary
        string currentHighestActiveReader = eventDictionary.Keys.First();

        // Check each registered reader in the dictionary and if the curret reader has a greater number of events compared to the current highest set the current highest to that reader
        foreach(var reader in eventDictionary)
        {

            if(reader.Value.Count >= eventDictionary[currentHighestActiveReader].Count)
            {

                currentHighestActiveReader = reader.Key;

            }

        }

        // Returns the reader description of the mostActiveReader
        mostActiveReader = eventDictionary[currentHighestActiveReader][0].GetEventDescription();
        
    }
    public static void GetBusiestDay()
    {

        // Sets the currentBusiestDay to the first ID in the dictionary
        string currentBusiestDay = eventDictionaryFilteredByTime.Keys.First();

        // Check each day of the week in the dictionary and if the curret day has a greater number of events compared to the current highest day of the week set the current highest to that day of the week
        foreach(var day in eventDictionaryFilteredByTime)
        {

            if(day.Value.Count >= eventDictionaryFilteredByTime[currentBusiestDay].Count)
            {

                currentBusiestDay = day.Key;

            }

        }

        busiestDay = currentBusiestDay;
        
    }
    public static void GetAverageUniqueVisitorsPerDay()
    {

        List<int> totalUniqueHashIDForEachDayList = new List<int>();

        // Go loop through each day
        foreach(var day in eventDictionaryFilteredByDay.Keys)
        {

            List<string> seenHashIDList = new List<string>();

            // Loop through all the events in that days Event List
            foreach(var readerEvent in eventDictionaryFilteredByDay[day])
            {

                if(!seenHashIDList.Contains(readerEvent.GetEventHashID()))
                {

                    seenHashIDList.Add(readerEvent.GetEventHashID());

                }

            }

            totalUniqueHashIDForEachDayList.Add(seenHashIDList.Count());

        }
        
        // Set AverageUniqueVisitorsPerDay to SeenHashIDList Count divided by the numberOfDays Caught in the eventDictionaryFilteredByDay
        int totalUniqueHashIDOverAllDays = 0;
        foreach(int uniqueHashForThatDay in totalUniqueHashIDForEachDayList)
        {

            totalUniqueHashIDOverAllDays += uniqueHashForThatDay;

        }
        averageUniqueVisitorsPerDay = totalUniqueHashIDOverAllDays/totalUniqueHashIDForEachDayList.Count;
        
    }
    
    public static void GetChartDataForDashboard()
    {

        chartFilter = $"{eventDictionaryFilteredByDay.Keys.First()} - {eventDictionaryFilteredByDay.Keys.Last()}";

        // Sets Graph display settings
        axisOptions.MatchBoundsToSize = true;
        Options.InterpolationOption = InterpolationOption.NaturalSpline;

        // Set the labels of the chart using the keys of the dictionary
        XAxisLabels = eventDictionaryFilteredByDay.Keys.ToArray();

        // Pull the reader event data from the values in the dictionary and put them into a double[]
        List<ReaderEvent>[] pullArrayOfEvents = eventDictionaryFilteredByDay.Values.ToArray();
        List<double> listOfEventsForEachDayOfTheWeek = new List<double>();
        foreach(List<ReaderEvent> reList in pullArrayOfEvents)
        {

            listOfEventsForEachDayOfTheWeek.Add(reList.Count());

        }

        // Add a new chart series for the chart with the data obtained from above
        Series.Add(new ChartSeries { Name = "Number of Scans", Data = listOfEventsForEachDayOfTheWeek.ToArray() } );

    }

    public class ReaderEvent
    {

        // Data of a ReaderEvent
        private DateTime time;
        private string location;
        private string description;
        private string hashID;
        private string devID;
        private string machine;

        // Constructors
        public ReaderEvent()
        {

            time = DateTime.Now;
            location = "N/A";
            description = "N/A";
            hashID = "N/A";
            devID = "N/A";
            machine = "N/A";

        }
        public ReaderEvent(string eTime, string eLocation, string eDescription, string eHashID, string eDevID, string eMachine)
        {

            time = DateTime.Parse(eTime);
            location = eLocation;
            description = eDescription;
            hashID = eHashID;
            devID = eDevID;
            machine = eMachine;

        }

        public DateTime GetEventTime() => time;
        public string GetEventLocation() => location;
        public string GetEventDescription() => description;
        public string GetEventHashID() => hashID;
        public string GetEventDevID() => devID;
        public string GetEventMachine() => machine;

        public string GetEventUniqueID() => devID + machine;

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