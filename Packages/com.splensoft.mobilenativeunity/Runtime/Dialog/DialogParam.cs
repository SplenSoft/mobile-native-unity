using System.Runtime.InteropServices;

namespace Protorius42.NativeDialog
{
    public enum ButtonType
    {
        Unknown = -1,
        Positive = 0,
        Neutral = 1,
        Negative = 2,
    }

    public enum ButtonErrorCode
    {
        NoError = 0,
        UnknownError = 100,
        NoImplemented = 101,
        ExceptionThrows = 102,
        TitleIsEmpty = 103,
        MessageIsEmpty = 104
    }
    
    public enum DialogIconStatus
    {
        No = -1,
        Info = 0,
        Alert = 1
    }
    
    public struct DialogParams
    {
        public string Title;
        public string Message;
        public DialogIconStatus IconStatus;
        public string ButtonPositiveText;
        public string ButtonNeutralText;
        public string ButtonNegativeText;
        [MarshalAs(UnmanagedType.I1)] public bool Cancelable;
        [MarshalAs(UnmanagedType.I1)] public bool FromHtml;

        /// <summary>
        /// Displays a modal dialog with one, two or three buttons
        /// </summary>
        /// <param name="title">Title for dialog, should not be empty or null [required]</param>
        /// <param name="message">Purpose for the dialog [required]</param>
        /// <param name="buttonPositiveText">Label text for ButtonType.Positive (Ok, Yes)</param>
        /// <param name="buttonNegativeText">Label text for ButtonType.Negative (No, back, cancel)</param>
        /// <param name="buttonNeutralText">Label text for ButtonType.Neutral (don't say, more,...)</param>
        /// <param name="iconStatus">Icon for dialog if supported by platform. See DialogIconStatus</param>
        /// <param name="cancelable">If true, the dialog can be canceled by tapping outside the dialog or by using the back button (on Android)</param>
        /// <param name="fromHtml">If the message is HTML with links</param>
        public DialogParams(string title, 
                            string message,
                            string buttonPositiveText = "OK", 
                            string buttonNegativeText = null,
                            string buttonNeutralText = null,
                            DialogIconStatus iconStatus = DialogIconStatus.No,
                            bool cancelable = false,
                            bool fromHtml = false)
        {
            this.Title = title;
            this.Message = message;
            this.IconStatus = iconStatus;
            this.ButtonPositiveText = buttonPositiveText;
            this.ButtonNeutralText = buttonNeutralText;
            this.ButtonNegativeText = buttonNegativeText;
            this.Cancelable = cancelable;
            this.FromHtml = fromHtml;
        }
    }
}