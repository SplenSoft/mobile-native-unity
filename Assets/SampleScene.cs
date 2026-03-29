using Protorius42.NativeDialog;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    public void OnOneButtonClick()
    {
        using (NativeAlertDialog dialog = new NativeAlertDialog())
        {
            DialogParams oneButtonDialog = new DialogParams("Confirm Dialog", "Please confirm", cancelable: false);
           
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
                        Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");
                    }
                });
        }
    }
    
    public void OnTwoButtonClick()
    {
        using (NativeAlertDialog dialog = new NativeAlertDialog())
        {
            DialogParams twoButtonsDialog = new DialogParams("Do you want to delete this account?", "You cannot undo this action.", "Delete", "Cancel");
         
            dialog.ShowNativeDialogAsync(twoButtonsDialog)
                  .ContinueWith(t =>
                                {
                                    if (t.Result.Item2 != ButtonErrorCode.NoError)
                                    {
                                        //your error handling here 
                                        Debug.LogError($"NativeDialog.OnTwoButtonClick done with error status code={t.Result.Item2}");
                                    }
                                    else
                                    {
                                        //check your the pressed button here
                                        Debug.Log($"NativeDialog.OnTwoButtonClick done, button {t.Result.Item1} pressed!");
                                    }
                                });
        }
    }
    
    public void OnThreeButtonClick()
    {
        using (NativeAlertDialog dialog = new NativeAlertDialog())
        {
            DialogParams threeButtonsDialog = new DialogParams("Rate my App", "Would you mind spending a moment to rate my App?", "Rate Now!", "Remind Me Later", "Don't Ask Again");
            dialog.ShowNativeDialogAsync(threeButtonsDialog)
                  .ContinueWith(t =>
                                {
                                    if (t.Result.Item2 != ButtonErrorCode.NoError)
                                    {
                                        //your error handling here 
                                        Debug.LogError($"NativeDialog.OnThreeButtonClick done with error status code={t.Result.Item2}");
                                    }
                                    else
                                    {
                                        //check your the pressed button here
                                        Debug.Log($"NativeDialog.OnThreeButtonClick done, button {t.Result.Item1} pressed!");
                                    }
                                });
        }
    }
    
    public void OnOneButtonAlertIconClick()
    {
        using (NativeAlertDialog dialog = new NativeAlertDialog())
        {
            DialogParams alertIconButtonDialog = new DialogParams("<<Input your TITLE here>>", "<<Input your MESSAGE here>>", "OK", null, null, DialogIconStatus.Alert);
           
            dialog.ShowNativeDialogAsync(alertIconButtonDialog)
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
                                        Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");
                                    }
                                });
        }
    }

    public void OnOneButtonInfoIconClick()
    {
        using (NativeAlertDialog dialog = new NativeAlertDialog())
        {
            DialogParams infoIconButtonDialog = new DialogParams("Input your TITLE here", "Input your MESSAGE here", "OK", null, null, DialogIconStatus.Info, cancelable: true);

            dialog.ShowNativeDialogAsync(infoIconButtonDialog)
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
                                        Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");
                                    }
                                });
        }
    }
    
    public void OnOneButtonWithHtmlTextClick()
    {
        using var dialog = new NativeAlertDialog();
        const string textWithHtmlTags = "Welcome to Native Dialog App!\n\n<br><br>\n\nBefore you proceed, please take a moment to review our legal documents:\n<br>\n- <a href=\"https://yourwebsite.com/privacy\">Privacy Policy</a>\n<br>\n- <a href=\"https://yourwebsite.com/terms\">Terms of Service</a>\n\n<br><br>\n\nBy clicking \"Agree\", you confirm that you have read and accepted these terms.";
        var oneButtonDialog = new DialogParams("Legal Agreements", textWithHtmlTags, fromHtml:true, buttonPositiveText: "Agree", buttonNegativeText: "Cancel");
           
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
                    //check the pressed button here (agree or cancel)
                    Debug.Log($"NativeDialog.OnOneButtonClick done, button {t.Result.Item1} pressed!");
                }
            });
    }
}


      