using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
	private static readonly HttpClient _httpClient = new HttpClient{Timeout = TimeSpan.FromSeconds(30)};
	
	public void GetLocalizedTime(string timeZone, out string date) {
		TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        DateTime localizedTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezone);
		date = localizedTime.ToString("yyyy-MM-d");
	}

    public string ModifyTime(string time) {
        int last = time.LastIndexOf(":");
        return time.Remove(last, 3);
    }

	public void Dispose() {
		_httpClient.Dispose();
	}
	
	public bool Execute() {
		this.GetLocalizedTime("Tokyo Standard Time", out string date);
        string url = string.Format("https://api.sunrisesunset.io/json?lat={0}&lng={1}&timezone=JST&date={2}", "35.6762", "139.6503", date);

		HttpResponseMessage response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
		if(!response.IsSuccessStatusCode) {
			CPH.SendMessage("Error: Could not get sun info");
			return false;
		}

		string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
		JsonResponse content = JsonConvert.DeserializeObject<JsonResponse>(responseBody);

		Results results = content.results;
        string sunrise = ModifyTime(results.sunrise);
        string dawn = ModifyTime(results.dawn);
        string dusk = ModifyTime(results.dusk);
        string sunset = ModifyTime(results.sunset);
        string golden_hour = ModifyTime(results.golden_hour);

		CPH.SendMessage($"Sun info for Tokyo: Sunrise: {sunrise}, Sunset: {sunset}, Dawn: {dawn}, Dusk: {dusk}, Golden hour: {golden_hour}");
        
		return true;
	}
}
