using MudBlazor;
using System.Linq;

public class FileStorage
{

    public static string FileName { get; set; } = "";
    public static string ErrorMessage { get; set; } = "";

    public static List<ReaderEvent> theMasterList = new List<ReaderEvent>();
    public static int masterListCount = 0;


    // Dictionary of ReaderEvents that uses the Unique ID as a key
    public static Dictionary<string, List<ReaderEvent>> eventDictionary = new Dictionary<string, List<ReaderEvent>>();


    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredByTime = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredByDay = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredHashID = new Dictionary<string, List<ReaderEvent>>();
    public static Dictionary<string, List<ReaderEvent>> eventDictionaryFilteredPanelID = new Dictionary<string, List<ReaderEvent>>();


    // Highlight Variables
    public static string mostActiveHashID = "(Upload a file)";
    public static string mostActiveHashIDScans = "0";
    public static string mostActiveReader = "(Upload a file)";
    public static string mostActiveReaderEventCount = "0";
    public static string busiestDay = "(Upload a file)";
    public static string busiestDayAverageScans = "0";
    public static int averageUniqueVisitorsPerDay = 0;

    // Chart Variables
    public static ChartOptions Options = new ChartOptions();
    public static AxisChartOptions axisOptions = new AxisChartOptions();
    
    public static List<ChartSeries> Series = new List<ChartSeries>();
    public static string[] XAxisLabels = Array.Empty<string>();

    public static string chartFilter = "";

    // Alert System Variables
    public static IEnumerable<AlertSystem.DataAlert> alertsList = new List<AlertSystem.DataAlert>();
    public static Dictionary<string, int> duplicateEntries = new Dictionary<string, int>(); // Contains the readers (unique ID for a key) and the number of duplicate entries removed caused by that reader

    public static int test = 0;

    public static async Task ProcessFile()
    {

        // Create a stream reader that reads from the stream when a file is uploaded & make an masterList to keep track
        var sr = new StreamReader(FileName);
        List<ReaderEvent> masterList = new List<ReaderEvent>();
        theMasterList = masterList;

        // The holds the most recent line read to compare the next one for a possible duplicate
        string[] previousReadEntry = ["", "", "", "", "", ""];

        // Check every line of the stream in StreamReader
        string? line = sr.ReadLine();
        while((line = sr.ReadLine()) != null) // Until there is no remaining lines left
        {   

            // Break each line into an array with Reader Data
            string[] data = line.Split(",");

            // Checks if the new line is a duplicate of the previous line
            if(data[0] == previousReadEntry[0]
            && data[1] == previousReadEntry[1]
            && data[2] == previousReadEntry[2]
            && data[3] == previousReadEntry[3]
            && data[4] == previousReadEntry[4]
            && data[5] == previousReadEntry[5])
            {

                test++;

                // Add the reader to the duplicate entries dictionary with 1 duplicate entry
                if(!duplicateEntries.ContainsKey($"{data[4]}-{data[5]}"))
                {

                    duplicateEntries[$"{data[4]}-{data[5]}"] = 1;

                }
                else
                {

                    // Add one to a reader's duplicates in the duplicate entry
                    duplicateEntries[$"{data[4]}-{data[5]}"]++;

                }

            }
            else
            {

                // Create a new Reader Event using the string array of the current line
                masterList.Add(new ReaderEvent(
                    data[0], // DateTime
                    data[1], // Location
                    data[2], // Description
                    data[3], // HashID
                    data[4], // DevID
                    data[5]  // Machine
                ));

                previousReadEntry = data;
                Console.WriteLine(previousReadEntry);

            }

            

        }

        masterListCount = masterList.Count;

        // Returns a dictionary with Readers and their Respective Events
        eventDictionary = OrganizeDictionData(masterList);
        eventDictionaryFilteredByTime = OrganizeDictionByDayOfTheWeek(masterList);
        eventDictionaryFilteredByDay = OrganizeDictionByDay(masterList);
        eventDictionaryFilteredHashID = OrganizeDictionByHashID(masterList);
        eventDictionaryFilteredPanelID = OrganizeDictionByPanel(masterList);

        // Get highlights after all the dictionaries have been organized
        GetMostActiveHashID();
        GetMostActiveReader();
        GetBusiestDay();
        GetAverageUniqueVisitorsPerDay();

        // Get Data for graph
        GetChartDataForDashboard();

        // Scan for Anomolies
        AlertSystem.ScanForAnomoly();
        alertsList = AlertSystem.masterAlertList;

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
    public static Dictionary<string, List<ReaderEvent>> OrganizeDictionByPanel(List<ReaderEvent> masterList)
    {

        Dictionary<string, List<ReaderEvent>> resultDict = new Dictionary<string, List<ReaderEvent>>();

        // Goes through each recorded ReaderEvent in the raw Data list
        foreach(ReaderEvent rawReaderEvent in masterList)
        {

            // Get the reader's ID that captured the event and add or create a section in the dictionary accordingly
            string panelID = rawReaderEvent.GetEventDevID();
            if(!resultDict.ContainsKey(panelID))
            {

                resultDict[panelID] = new List<ReaderEvent>(); // Create a entry when an event occured on a unique time is found

            }
            resultDict[panelID].Add(rawReaderEvent); // Add an event to the respective reader

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
        mostActiveHashIDScans = eventDictionaryFilteredHashID[mostActiveHashID].Count().ToString();
        
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
        mostActiveReaderEventCount = eventDictionary[currentHighestActiveReader].Count().ToString();
        
    }
    public static void GetBusiestDay()
    {

        // Dictionary for keeping track of the number of dayOfWeekOccurrences
        Dictionary<string, int> dayOfWeekOccurrences = new Dictionary<string, int>();
        DateTime lastDateChecked = DateTime.MinValue;

        foreach(var entry in theMasterList)
        {

            // Checks if a new day is detected
            if(entry.GetEventTime().Date != lastDateChecked.Date)
            {

                // Set the last date checked to the new current date
                lastDateChecked = entry.GetEventTime().Date;

                // If the day of the week of the date being checked has already been seen add 1 to the number of occurrences
                if(dayOfWeekOccurrences.ContainsKey(entry.GetEventTime().Date.DayOfWeek.ToString()))
                {

                    dayOfWeekOccurrences[entry.GetEventTime().Date.DayOfWeek.ToString()]++;

                }
                else
                {

                    // other wise add a new day of the week to the occurrences dictionary
                    dayOfWeekOccurrences.Add(entry.GetEventTime().Date.DayOfWeek.ToString(), 1);

                }

            }

        }


        // Sets the currentBusiestDay to the first ID in the dictionary
        string currentBusiestDay = eventDictionaryFilteredByTime.Keys.First();

        // Check each day of the week in the dictionary and if the curret day has a greater number of events compared to the current highest day of the week set the current highest to that day of the week
        foreach(var day in eventDictionaryFilteredByTime)
        {

            if(day.Value.Count/dayOfWeekOccurrences[day.Key] >= eventDictionaryFilteredByTime[currentBusiestDay].Count/dayOfWeekOccurrences[currentBusiestDay])
            {

                currentBusiestDay = day.Key;

            }

        }

        busiestDay = currentBusiestDay;
        busiestDayAverageScans = (eventDictionaryFilteredByTime[busiestDay].Count/dayOfWeekOccurrences[busiestDay]).ToString();
        
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

        public string GetEventUniqueID() => $"{devID}-{machine}";

        public override bool Equals(object obj)
        {
            
            if(time == ((ReaderEvent)obj).time && location == ((ReaderEvent)obj).location && description == ((ReaderEvent)obj).description && hashID == ((ReaderEvent)obj).hashID && devID == ((ReaderEvent)obj).devID && machine == ((ReaderEvent)obj).machine)
            {

                return true;

            }
            else
            {

                return false;

            }

        }

    }

}