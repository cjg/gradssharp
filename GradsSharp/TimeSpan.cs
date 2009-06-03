using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Descrizione di riepilogo per TimeSpan
/// </summary>
public class TimeSpan
{
    private long hours;
    private long minutes;
    private long seconds;

    private String result;


    public TimeSpan()
	{
		//
		// TODO: aggiungere qui la logica del costruttore
		//
	}

    public TimeSpan(DateTime start, DateTime end)
    {
        System.TimeSpan diff = end.Subtract(start);
        this.hours = diff.Hours;
        this.minutes = diff.Minutes;
        this.seconds = diff.Seconds;

        System.TimeSpan  tmp = new System.TimeSpan(diff.Hours, diff.Minutes, diff.Seconds);
        result = tmp.ToString();
    }

    override public String ToString()
    {
        return result;
    }
}
