using MudBlazor;

public class AlertSystem
{

    public List<DataAlert> alertsList = new List<DataAlert>();

    

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