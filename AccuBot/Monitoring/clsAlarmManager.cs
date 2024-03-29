﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AccuBot
{
    public class clsAlarmManager
    {
       
        List<clsAlarm> AlarmList = new List<clsAlarm>();
    
        public clsAlarmManager()
        {
        }
        
        public void New(clsAlarm Alarm)
        {
           Alarm.Process();
           AlarmList.Add(Alarm);
        }
        
        public void Clear(clsAlarm Alarm, String message = null)
        {
            if (AlarmList.Contains(Alarm))
            {
                Alarm.Clear(message);
                AlarmList.Remove(Alarm);
            }
            
        }
        
        public void Process()
        {
            //Process all Alarms, removing expired alarms.
          //  AlarmList.RemoveAll(x=>!x.Process());
          
          foreach (var alarm in AlarmList)
          {
            try
            {
                alarm.Process();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Alarm Process: {ex.Message}");
            }
          }
          
        }
        
        public new String ToString()
        {
            var sb = new StringBuilder();
            
            if (AlarmList.Count==0)
            {
                sb.AppendLine("no alarms");
            }
            else
            {
                sb.Append("```");
                foreach (var alarm in AlarmList)
                {
                    sb.AppendLine(alarm.ToString());
                }
                sb.Append("```");
            }
            return sb.ToString();
        }

		internal void Remove(clsAlarm Alarm)
		{
			if (Alarm != null && AlarmList.Contains(Alarm))
            {
                AlarmList.Remove(Alarm);
            }
		}
	}
}
