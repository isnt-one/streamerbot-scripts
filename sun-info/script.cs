using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

/*
 * Description: This action uses geographical coordinates of a specified location to find times related to sun light.
 * Note: This script is made with Tokyo, Japan in mind.
 */

class Results {
    public string sunrise;
    public string sunset;
    public string first_light;
    public string last_light;
    public string dawn;
    public string dusk;
    public string solar_moon;
    public string golden_hour;
    public string day_length;
    public string timezone;
}

class JsonResponse {
    public Results results;
    public string status;
}

public class CPHInline {
    public string ModifyTime(string time) {
        int last = time.LastIndexOf(":");
        return time.Remove(last, 3);
    }

    public bool Execute() {
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        DateTime localizedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
        string date = localizedTime.ToString("yyyy-MM-d");

        using(HttpClient client = new HttpClient()) {
            string url = @"https://api.sunrisesunset.io";
            url = string.Format("{0}/json?lat={1}&lng={2}&timezone=JST&date={3}", url, "35.6762", "139.6503", date);

            HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode) {
                string responseBody = response.Content.ReadAsStringAsync().Result;
                JsonResponse content = JsonConvert.DeserializeObject<JsonResponse>(responseBody);

                Results results = content.results;

                string sunrise = ModifyTime(results.sunrise);
                string dawn = ModifyTime(results.dawn);
                string dusk = ModifyTime(results.dusk);
                string sunset = ModifyTime(results.sunset);
                string golden_hour = ModifyTime(results.golden_hour);

                string data = string.Format("Sun info for Tokyo: Sunrise: {0}, Sunset: {1}, Dawn: {2}, Dusk: {3}, Golden hour: {4}", sunrise, sunset, dawn, dusk, golden_hour);

                CPH.SendMessage(data);
            } else {
                CPH.SendMessage("Failed to get sun data");
            }
        }

        return true;
    }
}
