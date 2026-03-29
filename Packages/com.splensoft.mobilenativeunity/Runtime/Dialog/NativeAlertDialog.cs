using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace Protorius42.NativeDialog 
{
    public class NativeAlertDialog : IDisposable
    {
        public const int IC_DIALOG_ALERT = 0x01080027;
        public const int IC_DIALOG_INFO = 0x0108009b;
        
#if UNITY_IOS && !UNITY_EDITOR
        public static TaskCompletionSource<(ButtonType, ButtonErrorCode)> tcs;
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void NativeDialogCallback(int result, int error);

        [DllImport("__Internal")]
        private static extern int _ShowNativeDialog(ref DialogParams conf, [MarshalAs(UnmanagedType.FunctionPtr)] NativeDialogCallback callback);

        // Note: This callback has to be static because Unity's il2Cpp doesn't support marshalling instance methods.
        [AOT.MonoPInvokeCallback(typeof(NativeDialogCallback))]
        private static void OnDialogClick(int result, int error)
        {
            tcs.TrySetResult(((ButtonType)result, (ButtonErrorCode)error));
        }
#elif UNITY_ANDROID && !UNITY_EDITOR
        private class NativeDialogCallback : AndroidJavaProxy
        {
            private readonly Action callback;

            public NativeDialogCallback(Action callback) : base("android.content.DialogInterface$OnClickListener")
            {
                this.callback = callback;
            }

            // Method must be lowercase to match Android method signature.
            public void onClick(AndroidJavaObject dialog, int id) {
                this.callback?.Invoke();
            }
        }
#endif
        
        #pragma warning disable 1998
        public async Task<(ButtonType, ButtonErrorCode)> ShowNativeDialogAsync(DialogParams conf)
        {
            try
            {
                if (string.IsNullOrEmpty(conf.Title))
                {
                    Debug.LogError("NativeAlertDialog.ShowNativeDialogAsync title is empty!");
                    return (ButtonType.Unknown, ButtonErrorCode.TitleIsEmpty);
                }
                
                if (string.IsNullOrEmpty(conf.Message))
                {
                    Debug.LogError("NativeAlertDialog.ShowNativeDialogAsync message is empty!");
                    return (ButtonType.Unknown, ButtonErrorCode.MessageIsEmpty);
                }

#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(conf.ButtonNeutralText))
                {
                    int option = UnityEditor.EditorUtility.DisplayDialogComplex(conf.Title, conf.Message, conf.ButtonPositiveText, conf.ButtonNegativeText, conf.ButtonNeutralText);
                    if (option == 0)
                        return (ButtonType.Positive, ButtonErrorCode.NoError);
                    if (option == 1)
                        return (ButtonType.Negative, ButtonErrorCode.NoError);
                    return (ButtonType.Neutral, ButtonErrorCode.NoError);
                }
                
                if (!string.IsNullOrEmpty(conf.ButtonNegativeText))
                {
                    return UnityEditor.EditorUtility.DisplayDialog(conf.Title, conf.Message, conf.ButtonPositiveText, conf.ButtonNegativeText)
                        ? (ButtonType.Positive, ButtonErrorCode.NoError)
                        : (ButtonType.Negative, ButtonErrorCode.NoError);
                }
                
                if (UnityEditor.EditorUtility.DisplayDialog(conf.Title, conf.Message, conf.ButtonPositiveText))
                {
                    return (ButtonType.Positive, ButtonErrorCode.NoError);
                }
                return (ButtonType.Unknown, ButtonErrorCode.NoImplemented);
#elif UNITY_ANDROID
                AndroidJavaObject activity = null;
                using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                    activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                }

                var tcs = new TaskCompletionSource<(ButtonType, ButtonErrorCode)>();
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    AndroidJavaObject dialog = null;
                    using (AndroidJavaObject builder = new AndroidJavaObject("android.app.AlertDialog$Builder", activity))
                    {
                        builder.Call<AndroidJavaObject>("setCancelable", conf.Cancelable);
                        builder.Call<AndroidJavaObject>("setTitle", conf.Title).Dispose();

                        // Support for HTML links
                        if (conf.FromHtml)
                        {
                            using var textView = new AndroidJavaObject("android.widget.TextView", activity);
                            AndroidJavaObject spannedText;
                            using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
                            {
                                var sdkInt = version.GetStatic<int>("SDK_INT");
                                if (sdkInt >= 24) // Android N (7.0) and above
                                {
                                    using var html = new AndroidJavaClass("android.text.Html");
                                    var flag = html.GetStatic<int>("FROM_HTML_MODE_LEGACY");
                                    spannedText = html.CallStatic<AndroidJavaObject>("fromHtml", conf.Message, flag);
                                }
                                else // Older Android versions
                                {
                                    using var html = new AndroidJavaClass("android.text.Html");
                                    spannedText = html.CallStatic<AndroidJavaObject>("fromHtml", conf.Message);
                                }
                            }
                                
                            textView.Call("setText", spannedText);
                            spannedText.Dispose();

                            using (var movementMethod = new AndroidJavaClass("android.text.method.LinkMovementMethod").CallStatic<AndroidJavaObject>("getInstance"))
                            {
                                textView.Call("setMovementMethod", movementMethod);
                            }

                            float density;
                            using (var resources = activity.Call<AndroidJavaObject>("getResources"))
                            using (var displayMetrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
                            {
                                density = displayMetrics.Get<float>("density");
                            }
                            int paddingH = (int)(22 * density);
                            int paddingV = (int)(16 * density);
                            textView.Call("setPadding", paddingH, paddingV, paddingH, paddingV);
                            builder.Call<AndroidJavaObject>("setView", textView).Dispose();
                        } else {
                            builder.Call<AndroidJavaObject>("setMessage", conf.Message).Dispose();
                        }
                        
                        if (!string.IsNullOrEmpty(conf.ButtonPositiveText))
                        {
                            builder.Call<AndroidJavaObject>("setPositiveButton", conf.ButtonPositiveText, new NativeDialogCallback(() => {
                                                                Debug.Log("OK/Yes Button pressed");
                                                                tcs.SetResult((ButtonType.Positive, ButtonErrorCode.NoError));
                                                            })).Dispose();
                        }

                        if (!string.IsNullOrEmpty(conf.ButtonNegativeText))
                        {
                            builder.Call<AndroidJavaObject>("setNegativeButton", conf.ButtonNegativeText, new NativeDialogCallback(() => {
                                                                Debug.Log("No Button pressed");
                                                                tcs.SetResult((ButtonType.Negative, ButtonErrorCode.NoError));
                                                            })).Dispose();
                        }

                        if (!string.IsNullOrEmpty(conf.ButtonNeutralText))
                        {
                            builder.Call<AndroidJavaObject>("setNeutralButton", conf.ButtonNeutralText, new NativeDialogCallback(() => {
                                                                Debug.Log("Cancel Button pressed");
                                                                tcs.SetResult((ButtonType.Neutral, ButtonErrorCode.NoError));
                                                            })).Dispose();
                        }

                        switch (conf.IconStatus)
                        {
                            case DialogIconStatus.Info:
                                builder.Call<AndroidJavaObject>("setIcon", IC_DIALOG_INFO).Dispose();
                                break;
                            case DialogIconStatus.Alert:
                                builder.Call<AndroidJavaObject>("setIcon", IC_DIALOG_ALERT).Dispose();
                                break;
                            case DialogIconStatus.No:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        
                        dialog = builder.Call<AndroidJavaObject>("create");
                    }
                    dialog.Call("show");
                    dialog.Dispose();
                    activity.Dispose();
                }));
                return await tcs.Task;
#elif UNITY_IOS
                tcs = new TaskCompletionSource<(ButtonType, ButtonErrorCode)>();
                _ShowNativeDialog(ref conf, OnDialogClick);
                return await tcs.Task;
#else
                Debug.LogError($"ShowNativeDialogAsync is not implemented for {Application.platform} platform");
                return (ButtonType.Unknown, ButtonErrorCode.NoImplemented);
#endif
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return (ButtonType.Unknown, ButtonErrorCode.ExceptionThrows);
            }
        }
        #pragma warning restore 1998

        #region IDisposable

        public void Dispose()
        {
        }

        #endregion
    }
}