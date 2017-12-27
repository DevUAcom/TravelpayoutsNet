# TravelpayoutsNet
TravelpayoutsNet is a .Net wrapper of the [Travelpayouts API](https://support.travelpayouts.com/hc/en-us/articles/203956163-Travel-insights-with-Travelpayouts-Data-API).

````csharp
  string token = "your_api_token";
  var apiClient = new TicketsCacheApiClient(token);
  
  var tickets = await apiClient.GetLatestAsync(); // Returns the list of prices found by users during the most recent 48 hours

````
