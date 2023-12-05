using System.ComponentModel;
using Newtonsoft.Json;
using System.Globalization;


public class MainViewModel : INotifyPropertyChanged
{

    private bool _isRefreshing;

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }

    public Command RefreshCommand { get; }

    public MainViewModel()
    {
        RefreshCommand = new Command(ExecuteRefreshCommand);
    }

    private async void ExecuteRefreshCommand()
    {
        System.Diagnostics.Debug.WriteLine("Refresh command executed.");

        // When the User refresh the app, we will make a query to the server
        // to get the newest data
        await MakeRequest();


        // After refreshing, set IsRefreshing to false
        IsRefreshing = false;
    }

    public void DefaultText()
    {
        // This methods was used to debugging
        UpdateTemperature("No temperature yet...");
        UpdateHumidity("No humidity yet...");
        UpdateDate("No TS yet...");
    }

    public async Task MakeRequest()
    {
        // Replace this with your actual Thingspeak API key
        string apiKey = "<>";
        int results = 1;
        string apiUrl = $"https://api.thingspeak.com/channels/2156442/feeds.json?api_key={apiKey}&results={results}";


        using (HttpClient client = new HttpClient())
        {
            // Display to the User that a request in on going
            UpdateDate("Making the request");

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // JSON response from Thingspeak
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Deserialize the JSON string into a dynamic object, which
                    // allows us to search more easily for specific fields.
                    dynamic jsonData = JsonConvert.DeserializeObject(responseBody);

                    // Update of the temperature field
                    string temperature = jsonData.feeds[0].field1;
                    UpdateTemperature($"{temperature}°C");

                    // Update of the humidty field
                    string humidity = jsonData.feeds[0].field2;
                    UpdateHumidity($"{humidity}%");

                    // The parsing will yield a string in the format "MM/dd/yyyy HH:mm:ss".
                    // Since this step can be more error-prone than the previous one, additional error handling is implemented.
                    // For more efficient debugging consider logging the exception instead of displaying them in the
                    // the user interface
                    string date = jsonData.feeds[0].created_at;

                    try
                    {
                        // Parse the UTC string to a DateTime object with the updated format
                        DateTime utcDateTime;

                        // Adjust the format based on the actual format of the 'created_at' field in your JSON
                        if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out utcDateTime))
                        {
                            // Convert to GMT+1
                            DateTime gmtPlusOneDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam"));

                            // Update of the date field
                            UpdateDate($"{gmtPlusOneDateTime}");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Convert the exception to a string
                        string exceptionMessage = ex.ToString();

                        // Display the exception message
                        UpdateDate($"Exception occured during date formating: {exceptionMessage}");
                    }
                }
                else
                {
                    // Display the exception message
                    UpdateDate($"Failed to make the request. Status code: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                // Display the exception message
                UpdateDate($"Request failed: {ex.Message}");
            }
        }
    }

    // Check if there are any subscribers to the PropertyChanged event.
    // If there are, invoke the event, notifying subscribers of the property change.
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Temperature related attributes and methods
    private string _temperatureText;

    public string TemperartureText
    {
        get { return _temperatureText; }
        set
        {
            if (_temperatureText != value)
            {
                _temperatureText = value;
                OnPropertyChanged(nameof(TemperartureText));
            }
        }
    }

    public void UpdateTemperature(string text)
    {
        TemperartureText = text;
    }

    // Humidity related attributes and methods
    private string _humidityText;

    public string HumidityText
    {
        get { return _humidityText; }
        set
        {
            if (_humidityText != value)
            {
                _humidityText = value;
                OnPropertyChanged(nameof(HumidityText));
            }
        }
    }

    public void UpdateHumidity(string text)
    {
        HumidityText = text;
    }

    // Date related attributes and methods
    private string _dateText;

    public string DateText
    {
        get { return _dateText; }
        set
        {
            if (_dateText != value)
            {
                _dateText = value;
                OnPropertyChanged(nameof(DateText));
            }
        }
    }

    public void UpdateDate(string text)
    {
        DateText = text;
    }

    static DateTime ConvertToTimeZone(DateTime dateTime, string timeZoneId)
    {
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        return TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZone);
    }
}
