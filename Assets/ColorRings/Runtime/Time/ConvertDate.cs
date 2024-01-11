using System;

public struct ConvertDate
{
    public DateTime ParseStringToData(string dateString, out bool canParse)
    {
        var format = "MM/dd/yyyy";
        try
        {
            // Parse the string to DateTime
            DateTime dateTime = DateTime.ParseExact(dateString, format, System.Globalization.CultureInfo.InvariantCulture);
            canParse = true;
            return dateTime;
        }
        catch (FormatException ex)
        {
            // Handle the format exception
            DevLog.Log("Error parsing the date: " , ex.Message);
            canParse = false;
            return new DateTime();
        }
    }
}