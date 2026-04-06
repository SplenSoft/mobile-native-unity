using UnityEngine;
using System.Collections;
using System;
using System.Threading.Tasks;
using Protorius42.NativeDateTimePicker;
using Cysharp.Threading.Tasks;
using Protorius42.NativeDialog;

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
            //MobileDialogInfo.Create(title, message, ok, okAction);

            using NativeAlertDialog dialog = new NativeAlertDialog();

            DialogParams oneButtonDialog = new DialogParams(
                title, 
                message, 
                buttonPositiveText: ok, 
                cancelable: false);

            dialog.ShowNativeDialogAsync(oneButtonDialog)
                .ContinueWith(t =>
                {
                    if (t.Result.Item2 != ButtonErrorCode.NoError)
                    {
                        //your error handling here 
                        Debug.LogError($"NativeDialog.OnOneButtonClick done with error status code={t.Result.Item2}");
                    }
                    else
                    {
                        //check your the pressed button here
                        //Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");

                        if (t.Result.Item1 == ButtonType.Positive)
                        {
                            okAction?.Invoke();
                        }
                    }
                });

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
            //MobileDialogConfirm.Create(title, message, yes, no, yesAction, noAction);

            using NativeAlertDialog dialog = new NativeAlertDialog();

            DialogParams oneButtonDialog = new DialogParams(
                title,
                message,
                buttonPositiveText: yes,
                buttonNegativeText: no,
                cancelable: false);

            dialog.ShowNativeDialogAsync(oneButtonDialog)
                .ContinueWith(t =>
                {
                    if (t.Result.Item2 != ButtonErrorCode.NoError)
                    {
                        //your error handling here 
                        Debug.LogError($"NativeDialog.OnOneButtonClick done with error status code={t.Result.Item2}");
                    }
                    else
                    {
                        //check your the pressed button here
                        //Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");

                        if (t.Result.Item1 == ButtonType.Positive)
                        {
                            yesAction?.Invoke();
                        }
                        else if (t.Result.Item1 == ButtonType.Negative)
                        {
                            noAction?.Invoke();
                        }
                    }
                });
        }
        public static void OpenDialog(string title, string message, string accept, string neutral, string decline, Action acceptAction = null, Action neutralAction = null, Action declineAction = null)
        {
            //MobileDialogNeutral.Create(title, message, accept, neutral, decline, acceptAction, neutralAction, declineAction);

        }
        public static void OpenDatePicker(int year, int month, int day, Action<DateTime> onChange = null, Action<DateTime> onClose = null)
        {
            //MobileDateTimePicker.CreateDate(year, month, day, onChange, onClose);

            using NativeDateTimePickerDialog dialog = new NativeDateTimePickerDialog();

            var dateTime = new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Local);
            var milliseconds = new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            
            var dateTimeParam = new DialoDateTimeParam(
                "Date Picker",
                DateTimePickerMode.UIDatePickerModeDate,
                "OK",
                "Cancel",
                milliseconds);

            dialog.ShowNativeDateTimeDialogAsync(dateTimeParam)
                .ContinueWith(t =>
                {
                    DateTime result = DateTimeOffset.FromUnixTimeSeconds(t.Result.Item1).LocalDateTime;

                    if (t.Result.Item2 == DateTimeErrorCode.UserCancelled)
                    {
                        Debug.Log($"DateTimeDialog.OnDateButtonClick cancelled");
                        onClose?.Invoke(result);
                    }
                    else if (t.Result.Item2 != DateTimeErrorCode.NoError)
                    {
                        Debug.LogError($"DateTimeDialog.OnDateButtonClick done with error status code={t.Result.Item2}");

                        onClose?.Invoke(result);
                    }
                    else
                    {
                        //string formatted = FormatTimestamp(t.Result.Item1, dateTimeParam.PickerMode);
                        //Debug.Log($"DateTimeDialog.OnDateButtonClick, date={formatted}!");
                        //this.text.text = formatted;

                        onChange?.Invoke(result);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        //public static void OpenTimePicker(Action<DateTime> onChange = null, Action<DateTime> onClose = null)
        //{
        //    MobileDateTimePicker.CreateTime(onChange, onClose);
        //}
    }
}