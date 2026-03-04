using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SplenSoft.Unity.MobileNative
{
    public class NativeDialog
    {
        public NativeDialog() { }

        public static void OpenDialog(string title, string message, string ok = "Ok", Action okAction = null)
        {
#if UNITY_EDITOR
            if (EditorUtility.DisplayDialog(title, message, ok))
            {
                okAction?.Invoke();
            }
#endif
            MobileDialogInfo.Create(title, message, ok, okAction);
        }
        public static void OpenDialog(string title, string message, string yes, string no, Action yesAction = null, Action noAction = null)
        {
#if UNITY_EDITOR
            if (EditorUtility.DisplayDialog(title, message, yes, no))
            {
                yesAction?.Invoke();
            }
            else
            {
                noAction?.Invoke();
            }
#endif
            MobileDialogConfirm.Create(title, message, yes, no, yesAction, noAction);
        }
        public static void OpenDialog(string title, string message, string accept, string neutral, string decline, Action acceptAction = null, Action neutralAction = null, Action declineAction = null)
        {
            MobileDialogNeutral.Create(title, message, accept, neutral, decline, acceptAction, neutralAction, declineAction);
        }
        public static void OpenDatePicker(int year , int month, int day, Action<DateTime> onChange = null, Action<DateTime> onClose = null)
        {
            MobileDateTimePicker.CreateDate(year, month, day, onChange, onClose);
        }
        public static void OpenTimePicker(Action<DateTime> onChange = null, Action<DateTime> onClose = null)
        {
            MobileDateTimePicker.CreateTime(onChange, onClose);
        }
    }
}