using Newtonsoft.Json;

HttpClient httpClient = new HttpClient();
string stringAPI = "https://api.wallex.ir/v1/currencies/stats";

await RunInBackground(TimeSpan.FromSeconds(10), () => InitAsync());

async Task RunInBackground(TimeSpan timeSpan, Action action)
{
    var periodicTimer = new PeriodicTimer(timeSpan);
    do { action(); }
    while (await periodicTimer.WaitForNextTickAsync());                                                                   
    {
        action();
    }
}



async Task InitAsync()
{
    HttpResponseMessage response = await httpClient.GetAsync(stringAPI);
    if (response.IsSuccessStatusCode)
    {
        string apiresponse = await response.Content.ReadAsStringAsync();
        ApiResponseWrapper apiWrapper = JsonConvert.DeserializeObject<ApiResponseWrapper>(apiresponse);
        List<ResultItem> ResultItems = apiWrapper.Result;
        foreach (var item in ResultItems)
        {
            Console.WriteLine($"Key: {item.key}");
            Console.WriteLine($"Name: {item.name_en}");
            Console.WriteLine($"Price: {item.price}");
            Console.WriteLine(item.prediction(24));
            Console.WriteLine(item.prediction(168));
            Console.WriteLine();
        }
    }
}
Console.ReadLine();




public class ApiResponseWrapper
{
    public List<ResultItem> Result { get; set; }
}


public class ResultItem
{
    public string key { get; set; }
    public string name_en { get; set; }
    public double price { get; set; }
    public double? daily_high_price { get; set; }
    public double? weekly_high_price { get; set; }



    public string prediction()
    {
        double newPrice = 0;
        newPrice = price + (price * Convert.ToDouble(daily_high_price) / 100);
        string output = $"Possible price in 24 hour is: {newPrice}";
        return output;
    }

    public string prediction(int hour)
    {
        double? newPrice = 0;
        newPrice = (price + (price * hour * daily_high_price / 100));
        string output = $"Possible price in {hour} hours is: {newPrice}";
        return output;
    }

    public string Prediction()
    {
        double newPrice = 0;
        newPrice = price + (price * Convert.ToDouble(weekly_high_price) / 100);
        string output = $"Possible price in 24 hour is: {newPrice}";
        return output;
    }

    public string Prediction(int hour)
    {
        double? newPrice = 0;
        newPrice = (price + (price * hour * weekly_high_price / 100));
        string output = $"Possible price in {hour} hours is: {newPrice}";
        return output;
    }
}

