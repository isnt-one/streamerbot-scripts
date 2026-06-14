using System;
using System.Collections.Generic;

/*
 * Description: This action looks up the date a user created their account and sends a formatted span of time to chat
 */

public class CPHInline {
    private (int Years, int Months, int Days, int Hours, int Minutes, double Seconds) CalculatePreciseAge(DateTime createdAt) {
        DateTime now = DateTime.UtcNow;
        int years = now.Year - createdAt.Year;
        int months = now.Month - createdAt.Month;
        int days = now.Day - createdAt.Day;

        if (days < 0) {
            months--;
            days += DateTime.DaysInMonth(now.Year, now.Month == 1 ? 12 : now.Month - 1);
        }
        if (months < 0) {
            years--;
            months += 12;
        }

        TimeSpan timeSpan = now - createdAt;

        return (years, months, days,
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds + timeSpan.Milliseconds / 1000.0);
    }

    private string FormatAge((int Years, int Months, int Days, int Hours, int Minutes, double Seconds) age) {
        List<string> parts = new List<string>();

        if (age.Years > 0)   parts.Add($"{age.Years} {(age.Years == 1 ? "year" : "years")}");
        if (age.Months > 0)  parts.Add($"{age.Months} {(age.Months == 1 ? "month" : "months")}");
        if (age.Days > 0)    parts.Add($"{age.Days} {(age.Days == 1 ? "day" : "days")}");
        if (age.Hours > 0)   parts.Add($"{age.Hours} {(age.Hours == 1 ? "hour" : "hours")}");

        bool isLessThanOneDay = (age.Years == 0 && age.Months == 0 && age.Days == 0);
        if (isLessThanOneDay) {
            if (age.Minutes > 0) parts.Add($"{age.Minutes} {(age.Minutes == 1 ? "minute" : "minutes")}");
            if (age.Seconds > 0) parts.Add($"{age.Seconds:0.##} seconds");
        }

        if (parts.Count == 0) return "0 seconds";

        if (parts.Count > 1)
            return $"{string.Join(", ", parts.GetRange(0, parts.Count - 1))} and {parts[parts.Count - 1]}";

        return parts[0];
    }

    public bool Execute() {
        string targetUser = string.Empty;
        // Get sender from message if a username is not provided
        if (!CPH.TryGetArg<string>("input0", out targetUser) || string.IsNullOrWhiteSpace(targetUser)) {
            CPH.TryGetArg<string>("userName", out targetUser);
        }

        TwitchUserInfoEx userInfo = CPH.TwitchGetExtendedUserInfoByLogin(targetUser);
        if (userInfo != null) {
            DateTime createdAt = userInfo.CreatedAt;
            var age = CalculatePreciseAge(createdAt);
            string timeString = FormatAge(age);
            CPH.SendMessage($"{targetUser} made their account {timeString} ago!");
        } else {
            CPH.SendMessage($"Could not lookup {targetUser}");
        }

        return true;
    }
}
