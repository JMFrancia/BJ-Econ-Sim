using System;
using UnityEngine;

[Serializable]
public class TimeSpan
{
    public int Seconds { get; private set; } = 0;
    public int Minutes { get; private set; } = 0;
    public int Hours { get; private set; } = 0;
    public int Days { get; private set; } = 0;

    public int TotalSeconds {
        get {
            return Seconds + TotalMinutes * 60;
        } 
    }
    public int TotalMinutes { 
        get 
        {
            return Minutes + TotalHours * 60;
        }
    }
    public int TotalHours { 
        get {
            return Hours + Days * 24;
        }
    } 

    public TimeSpan() : this(0, 0, 0, 0) { }

    public TimeSpan(int seconds, int minutes = 0, int hours = 0, int days = 0) {
        Set(seconds, minutes, hours, days);
    }

    public TimeSpan(TimeSpan timeSpan) {
        Set(timeSpan);
    }

    public void Set(int seconds, int minutes = 0, int hours = 0, int days = 0) {
        if(seconds >= 60) {
            minutes += Mathf.FloorToInt(seconds / 60);
        }
        Seconds = Mathf.Max(0, seconds % 60);

        if(minutes >= 60) {
            hours += Mathf.FloorToInt(minutes / 60);
        }
        Minutes = Mathf.Max(0, minutes % 60);


        if(hours >= 24) {
            days += Mathf.FloorToInt(hours / 60);
        }
        Hours = Mathf.Max(0, hours % 24);


        Days = Mathf.Max(days, 0);
    }

    public void Set (TimeSpan timeSpan) {
         Set(timeSpan.Seconds, timeSpan.Minutes, timeSpan.Hours, timeSpan.Days);
    }

    public void Increment(int seconds, int minutes = 0, int hours = 0, int days = 0) {
        Set(Seconds + seconds, Minutes + minutes, Hours + hours, Days + days);
    }

    public void Increment(TimeSpan timeSpan) {
        Increment(timeSpan.Seconds, timeSpan.Minutes, timeSpan.Hours, timeSpan.Days);
    }

    public override string ToString()
    {
        return $"{Days}d {Hours.ToString("00")}h {Minutes.ToString("00")}:{Seconds.ToString("00")}m";
    }
}