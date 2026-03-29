//
//  CustomWebDialog.h
//
//  Created by Protorius42 on 23/07/2025.
//  Copyright © 2025 Protorius42. All rights reserved.
//

#import <UIKit/UIKit.h>

namespace protorius {

    typedef void (*NativeDialogCallback)(int result, int error);
    
    typedef NS_ENUM(NSInteger, ButtonType) {
      Unknown = -1,
      Positive = 0,
      Neutral = 1,
      Negative = 2,
    };

    typedef NS_ENUM(NSInteger, ButtonErrorCode) {
        NoError = 0,
        UnknownError = 100,
        NoImplemented = 101,
        ExceptionThrows = 102,
        TitleIsEmpty = 103,
        MessageIsEmpty = 104
    };

    struct NativeDialogStruct {
        char* title;
        char* message;
        int iconStatus;
        char* buttonPositiveText;
        char* buttonNeutralText;
        char* buttonNegativeText;
        bool cancelable;
        bool fromHtml;
    };
}


namespace protorius {
    typedef void (*NativeDialogCallback)(int result, int error);
    enum ButtonType : NSInteger;
    enum ButtonErrorCode : NSInteger;
}


@interface CustomWebDialogViewController : UIViewController <UITextViewDelegate>

/**
 * Initializes a new custom web dialog view controller.
 *
 * @param title The title of the dialog.
 * @param message The message content, which can contain HTML tags.
 * @param positiveText The text for the positive action button (e.g., "OK", "Agree").
 * @param neutralText The text for the neutral action button (e.g., "Later", "More Info").
 * @param negativeText The text for the negative action button (e.g., "Cancel", "Decline").
 * @param cancelable A boolean indicating if the dialog can be dismissed without a specific button press (adds a "Cancel" button if true and no negative button is present).
 * @param callback The C-style callback function to be invoked when a button is tapped.
 */
- (instancetype)initWithTitle:(NSString *)title
                      message:(NSString *)message
           buttonPositiveText:(NSString *)positiveText
            buttonNeutralText:(NSString *)neutralText
           buttonNegativeText:(NSString *)negativeText
                   cancelable:(BOOL)cancelable
                     callback:(protorius::NativeDialogCallback)callback;

@end
