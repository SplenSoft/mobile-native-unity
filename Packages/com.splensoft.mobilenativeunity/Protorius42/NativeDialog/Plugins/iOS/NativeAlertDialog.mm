//
//  NativeAlertDialog.mm
//
//  Created by Protorius42 on 08/11/2021.
//  Copyright © 2021 Protorius42. All rights reserved.
//
#import <UIKit/UIKit.h>
#import "CustomWebDialog.h"

#ifdef __cplusplus
extern "C" {
#endif
    UIViewController* UnityGetGLViewController();

    int _ShowNativeDialog(struct protorius::NativeDialogStruct *params, protorius::NativeDialogCallback callback) {
       
        try
        {
            NSString *title = params->title ? [[NSString alloc] initWithUTF8String:params->title] : nil;
            NSString *message = params->message ? [[NSString alloc] initWithUTF8String:params->message] : nil;
            NSString *buttonPositiveText = params->buttonPositiveText ? [[NSString alloc] initWithUTF8String:params->buttonPositiveText] : @"OK";
            NSString *buttonNeutralText = params->buttonNeutralText ? [[NSString alloc] initWithUTF8String:params->buttonNeutralText] : nil;
            NSString *buttonNegativeText = params->buttonNegativeText ? [[NSString alloc] initWithUTF8String:params->buttonNegativeText] : nil;
           
            UIViewController *rootViewController = UnityGetGLViewController();
            if (!rootViewController || !rootViewController.view.window) {
                NSLog(@"_ShowNativeDialog: Root UIViewController is not available or not in window hierarchy. Cannot present alert.");
                callback(protorius::Unknown, protorius::UnknownError);
                return -1;
            }
            
            if (params->fromHtml) {
               // Instantiate and present your custom dialog for HTML content
               CustomWebDialogViewController *customDialog =
                   [[CustomWebDialogViewController alloc] initWithTitle:title
                                                                message:message
                                                     buttonPositiveText:buttonPositiveText
                                                      buttonNeutralText:buttonNeutralText
                                                     buttonNegativeText:buttonNegativeText
                                                             cancelable:params->cancelable
                                                               callback:callback];
               
               // Set modal presentation style for an alert-like appearance
               customDialog.modalPresentationStyle = UIModalPresentationOverFullScreen;
               customDialog.modalTransitionStyle = UIModalTransitionStyleCrossDissolve;
               
                dispatch_async(dispatch_get_main_queue(), ^{
                    UIViewController *rootViewController = UnityGetGLViewController();
                    if (rootViewController) {
                        [rootViewController presentViewController:customDialog animated:YES completion:^{
                            NSLog(@"_ShowNativeDialog custom HTML dialog completion done!");
                        }];
                    } else {
                        NSLog(@"_ShowNativeDialog: Root UIViewController is not available on dispatch.");
                        // Consider calling the callback here to handle the error
                        callback(protorius::Unknown, protorius::UnknownError);
                    }
                });
             
               return 0; // Successfully initiated presentation of custom dialog
                        
            }
            else
            {
                // Fallback to standard UIAlertController for plain text messages
                UIAlertController* alert = [UIAlertController alertControllerWithTitle:title
                                                                               message:message
                                                                        preferredStyle:UIAlertControllerStyleAlert];
                
                if (buttonPositiveText)
                {
                    UIAlertAction* defaultAction = [UIAlertAction actionWithTitle:buttonPositiveText
                                                                            style:UIAlertActionStyleCancel
                                                                          handler:^(UIAlertAction * action) {
                        callback(protorius::Positive, protorius::NoError);
                    }];
                    [alert addAction:defaultAction];
                }
                
                if (buttonNeutralText)
                {
                    UIAlertAction* neutralAction = [UIAlertAction actionWithTitle:buttonNeutralText
                                                                            style:UIAlertActionStyleDefault
                                                                          handler:^(UIAlertAction * action) {
                        callback(protorius::Neutral, protorius::NoError);
                    }];
                    [alert addAction:neutralAction];
                }
                
                if (buttonNegativeText)
                {
                    UIAlertAction *cancelAction = [UIAlertAction actionWithTitle:buttonNegativeText
                                                                           style:UIAlertActionStyleDestructive
                                                                         handler:^(UIAlertAction *action) {
                        callback(protorius::Negative, protorius::NoError);
                    }];
                    [alert addAction:cancelAction];
                }
                
                UIViewController *rootViewController = UnityGetGLViewController();
                [rootViewController presentViewController:alert animated:NO completion:^{
                    NSLog(@"_ShowNativeDialog completion done!");
                }];
                
                if (!alert.isBeingPresented) {
                    NSLog(@"_ShowNativeDialog alert is not presented!");
                    NSMutableString * data = [[NSMutableString alloc] init];
                    [data appendString:@"_ShowNativeDialog alert is not presented!"];
                    callback(protorius::Unknown, protorius::UnknownError);
                    return -1;
                }
            }
        }
        catch (NSException *e)
        {
            NSLog(@"_ShowNativeDialog error %@", [e reason]);
            NSMutableString * data = [[NSMutableString alloc] init];
            [data appendString: [e reason]];
            callback(protorius::Unknown, protorius::ExceptionThrows);
        }
        
        return 0;
    }
   
#ifdef __cplusplus
}
#endif

/*

extern "C" {
    int _ShowNativeDialog(struct protorius::NativeDialogStruct *params, protorius::NativeDialogCallback callback) {
        return protorius::_ShowNativeDialog(params, callback);
    }
}
*/
