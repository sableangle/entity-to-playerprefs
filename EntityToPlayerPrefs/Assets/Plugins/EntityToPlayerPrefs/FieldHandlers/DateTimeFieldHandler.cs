﻿using System;

namespace Assets.Plugins.EntityToPlayerPrefs.FieldHandlers
{
    public class DateTimeFieldHandler : PlayerPrefsFieldHandler
    {
        private const string ValuePrefix = "__dt.";

        public override object GetValue(string fieldKey)
        {
            return GetDateTime(fieldKey);
        }

        private static DateTime GetDateTime(string fieldKey)
        {
            string valueString = PlayerPrefsProvider.GetString(fieldKey);
            string binaryString;
            if (valueString.StartsWith(ValuePrefix))
                binaryString = valueString.Substring(5);
            else //version 0.1 format without prefix
                binaryString = valueString;

            DateTime dateTime = string.IsNullOrEmpty(binaryString)
                ? new DateTime()
                : DateTime.FromBinary(long.Parse(binaryString));
            return dateTime;
        }

        public override void SetValue(string fieldKey, DataMemberInfo dataMemberInfo, object entity)
        {
            SetDateTimeValue(fieldKey, dataMemberInfo.GetValue<DateTime>(entity));
        }

        private static void SetDateTimeValue(string fieldKey, DateTime dateTime)
        {
            PlayerPrefsProvider.SetString(fieldKey, ValuePrefix + dateTime.ToBinary());
        }

#if UNITY_EDITOR

        public static bool IsDateTimeRecord(string fieldKey)
        {
            return PlayerPrefsProvider.GetString(fieldKey).StartsWith(ValuePrefix);
        }

        public static bool DrawEditor(string fieldKey)
        {
            string dateTimeString = GetDateTime(fieldKey).ToString();
            string newDateTimeString = UnityEditor.EditorGUILayout.TextField(dateTimeString);
            if (newDateTimeString != dateTimeString)
            {
                DateTime newDateTime;
                if (DateTime.TryParse(newDateTimeString, out newDateTime))
                {
                    SetDateTimeValue(fieldKey, newDateTime);
                    PlayerPrefsProvider.Save();
                    return true;
                }
            }
            return false;
        }
#endif

    }
}
