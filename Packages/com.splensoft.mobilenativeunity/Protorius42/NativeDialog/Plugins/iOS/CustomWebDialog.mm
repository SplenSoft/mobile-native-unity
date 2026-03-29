//
//  CustomWebDialog.mm
//
//  Created by Protorius42 on 23/07/2025.
//  Copyright © 2025 Protorius42. All rights reserved.
//

#import "CustomWebDialog.h"

@interface CustomWebDialogViewController ()

@property (nonatomic, strong) NSString *dialogTitle;
@property (nonatomic, strong) NSString *dialogMessage;
@property (nonatomic, strong) NSString *buttonPositiveText;
@property (nonatomic, strong) NSString *buttonNeutralText;
@property (nonatomic, strong) NSString *buttonNegativeText;
@property (nonatomic, assign) BOOL isCancelable;
@property (nonatomic, assign) protorius::NativeDialogCallback callback;

@end

@implementation CustomWebDialogViewController

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
                     callback:(protorius::NativeDialogCallback)callback
{
    self = [super init];
    if (self) {
        _dialogTitle = [title copy];
        _dialogMessage = [message copy];
        _buttonPositiveText = [positiveText copy];
        _buttonNeutralText = [neutralText copy];
        _buttonNegativeText = [negativeText copy];
        _isCancelable = cancelable;
        _callback = callback;
    }
    return self;
}

- (void)viewDidLoad {
    [super viewDidLoad];
    
    // Set up the background for the modal presentation
    self.view.backgroundColor = [[UIColor blackColor] colorWithAlphaComponent:0.4];
    
    // Create the main container view for the dialog content
    UIView *dialogView = [[UIView alloc] init];
    dialogView.backgroundColor = [UIColor whiteColor];
    dialogView.layer.cornerRadius = 10; // Rounded corners for the dialog box
    dialogView.translatesAutoresizingMaskIntoConstraints = NO;
    [self.view addSubview:dialogView];
    
    // Title Label: Displays the dialog title
    UILabel *titleLabel = [[UILabel alloc] init];
    titleLabel.text = self.dialogTitle;
    titleLabel.font = [UIFont boldSystemFontOfSize:20];
    titleLabel.textAlignment = NSTextAlignmentCenter;
    titleLabel.numberOfLines = 0; // Allow multiple lines for long titles
    titleLabel.translatesAutoresizingMaskIntoConstraints = NO;
    [dialogView addSubview:titleLabel];
    
    // Message TextView: Displays the HTML message with clickable links
    UITextView *messageTextView = [[UITextView alloc] init];
    messageTextView.editable = NO; // Prevent user from editing text
    messageTextView.selectable = YES; // Allow text selection and link tapping
    messageTextView.dataDetectorTypes = UIDataDetectorTypeLink; // Automatically detect URLs and make them tappable
    messageTextView.delegate = self; // Set delegate to handle link taps
    messageTextView.textAlignment = NSTextAlignmentCenter; // Center the text within the text view
    messageTextView.translatesAutoresizingMaskIntoConstraints = NO;
    messageTextView.backgroundColor = [UIColor clearColor]; // Make background transparent
    messageTextView.textContainerInset = UIEdgeInsetsMake(0, 0, 0, 0); // Remove default padding
    messageTextView.scrollEnabled = NO;
    messageTextView.font = [UIFont systemFontOfSize:18];
    [dialogView addSubview:messageTextView];
    
    // Convert HTML string to NSAttributedString for display in UITextView
    NSData *data = [self.dialogMessage dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *options = @{NSDocumentTypeDocumentAttribute: NSHTMLTextDocumentType,
                              NSCharacterEncodingDocumentAttribute: @(NSUTF8StringEncoding)};
    NSError *error = nil;
    
    NSMutableAttributedString *attributedMessage = [[NSMutableAttributedString alloc] initWithData:data
                                                                             options:options
                                                                  documentAttributes:nil
                                                                               error:&error];
    if (attributedMessage && !error) {
          [attributedMessage beginEditing];
          [attributedMessage enumerateAttribute:NSFontAttributeName inRange:NSMakeRange(0, attributedMessage.length) options:0 usingBlock:^(id value, NSRange range, BOOL *stop) {
              UIFont *currentFont = (UIFont *)value;
              UIFontDescriptor *fontDescriptor = currentFont.fontDescriptor;
              UIFont *newFont = [UIFont fontWithDescriptor:fontDescriptor size:18]; // Set desired font size here (e.g., 18 for AlertDialog default)
              [attributedMessage addAttribute:NSFontAttributeName value:newFont range:range];
          }];
          [attributedMessage endEditing];
        messageTextView.attributedText = attributedMessage;
    } else {
        NSLog(@"Error parsing HTML message for CustomWebDialog: %@. Displaying as plain text.", error);
        messageTextView.text = self.dialogMessage; // Fallback to plain text if HTML parsing fails
    }
    
    // Stack View for arranging buttons horizontally
    UIStackView *buttonStackView = [[UIStackView alloc] init];
    buttonStackView.axis = UILayoutConstraintAxisHorizontal;
    buttonStackView.distribution = UIStackViewDistributionFillEqually; // Distribute space evenly
    buttonStackView.spacing = 10; // Space between buttons
    buttonStackView.translatesAutoresizingMaskIntoConstraints = NO;
    [dialogView addSubview:buttonStackView];
    
    // Add buttons based on provided text
    if (self.buttonNegativeText) {
        UIButton *negativeButton = [UIButton buttonWithType:UIButtonTypeSystem];
        [negativeButton setTitle:self.buttonNegativeText forState:UIControlStateNormal];
        negativeButton.tintColor = [UIColor systemRedColor]; // Style for destructive action
        [negativeButton addTarget:self action:@selector(negativeButtonTapped) forControlEvents:UIControlEventTouchUpInside];
        [buttonStackView addArrangedSubview:negativeButton];
    } else if (self.isCancelable) {
        // If cancelable and no explicit negative button, add a generic "Cancel" button
        UIButton *cancelButton = [UIButton buttonWithType:UIButtonTypeSystem];
        [cancelButton setTitle:@"Cancel" forState:UIControlStateNormal];
        [cancelButton addTarget:self action:@selector(negativeButtonTapped) forControlEvents:UIControlEventTouchUpInside];
        [buttonStackView addArrangedSubview:cancelButton];
    }
    
    if (self.buttonNeutralText) {
        UIButton *neutralButton = [UIButton buttonWithType:UIButtonTypeSystem];
        [neutralButton setTitle:self.buttonNeutralText forState:UIControlStateNormal];
        [neutralButton addTarget:self action:@selector(neutralButtonTapped) forControlEvents:UIControlEventTouchUpInside];
        [buttonStackView addArrangedSubview:neutralButton];
    }
    
    if (self.buttonPositiveText) {
        UIButton *positiveButton = [UIButton buttonWithType:UIButtonTypeSystem];
        [positiveButton setTitle:self.buttonPositiveText forState:UIControlStateNormal];
        [positiveButton addTarget:self action:@selector(positiveButtonTapped) forControlEvents:UIControlEventTouchUpInside];
        [buttonStackView addArrangedSubview:positiveButton];
    }
    
    // Auto Layout Constraints for `dialogView` (centered and sized)
    [NSLayoutConstraint activateConstraints:@[
        [dialogView.centerXAnchor constraintEqualToAnchor:self.view.centerXAnchor],
        [dialogView.centerYAnchor constraintEqualToAnchor:self.view.centerYAnchor],
        // Max width to prevent it from being too wide on iPad, etc.
        [dialogView.widthAnchor constraintLessThanOrEqualToAnchor:self.view.widthAnchor multiplier:0.8],
        // Minimum width for readability
        [dialogView.widthAnchor constraintGreaterThanOrEqualToConstant:280],
        // Max height to prevent it from being too tall
        [dialogView.heightAnchor constraintLessThanOrEqualToAnchor:self.view.heightAnchor multiplier:0.8]
    ]];
    
    // Auto Layout Constraints for `titleLabel`
    [NSLayoutConstraint activateConstraints:@[
        [titleLabel.topAnchor constraintEqualToAnchor:dialogView.topAnchor constant:20],
        [titleLabel.leadingAnchor constraintEqualToAnchor:dialogView.leadingAnchor constant:20],
        [titleLabel.trailingAnchor constraintEqualToAnchor:dialogView.trailingAnchor constant:-20]
    ]];
    
    // Auto Layout Constraints for `messageTextView`
    [NSLayoutConstraint activateConstraints:@[
        [messageTextView.topAnchor constraintEqualToAnchor:titleLabel.bottomAnchor constant:10],
        [messageTextView.leadingAnchor constraintEqualToAnchor:dialogView.leadingAnchor constant:15],
        [messageTextView.trailingAnchor constraintEqualToAnchor:dialogView.trailingAnchor constant:-15],
        [messageTextView.heightAnchor constraintGreaterThanOrEqualToConstant:50] // Ensure minimum height for message
    ]];
    
    // Auto Layout Constraints for `buttonStackView`
    [NSLayoutConstraint activateConstraints:@[
        [buttonStackView.topAnchor constraintEqualToAnchor:messageTextView.bottomAnchor constant:20],
        [buttonStackView.leadingAnchor constraintEqualToAnchor:dialogView.leadingAnchor constant:20],
        [buttonStackView.trailingAnchor constraintEqualToAnchor:dialogView.trailingAnchor constant:-20],
        [buttonStackView.bottomAnchor constraintEqualToAnchor:dialogView.bottomAnchor constant:-20],
        [buttonStackView.heightAnchor constraintEqualToConstant:44] // Standard button height
    ]];
}

#pragma mark - Button Action Handlers

/**
 * Handles the tap on the positive action button.
 * Invokes the callback with `protorius::Positive` and dismisses the dialog.
 */
- (void)positiveButtonTapped {
    if (self.callback) {
        self.callback(protorius::Positive, protorius::NoError);
    }
    [self dismissViewControllerAnimated:YES completion:nil];
}

/**
 * Handles the tap on the neutral action button.
 * Invokes the callback with `protorius::Neutral` and dismisses the dialog.
 */
- (void)neutralButtonTapped {
    if (self.callback) {
        self.callback(protorius::Neutral, protorius::NoError);
    }
    [self dismissViewControllerAnimated:YES completion:nil];
}

/**
 * Handles the tap on the negative action button (or generic "Cancel").
 * Invokes the callback with `protorius::Negative` and dismisses the dialog.
 */
- (void)negativeButtonTapped {
    if (self.callback) {
        self.callback(protorius::Negative, protorius::NoError);
    }
    [self dismissViewControllerAnimated:YES completion:nil];
}

#pragma mark - UITextViewDelegate

/**
 * Called when the user interacts with a URL in the UITextView.
 * This method is crucial for handling clickable links.
 *
 * @param textView The text view containing the URL.
 * @param URL The URL that was interacted with.
 * @param characterRange The range of the URL in the text view's text.
 * @param interaction The type of interaction.
 * @return YES if the system should handle the interaction, NO if the delegate handled it.
 */
- (BOOL)textView:(UITextView *)textView shouldInteractWithURL:(NSURL *)URL inRange:(NSRange)characterRange interaction:(UITextItemInteraction)interaction {
    
    if ([[UIApplication sharedApplication] canOpenURL:URL]) {
        [[UIApplication sharedApplication] openURL:URL options:@{} completionHandler:^(BOOL success) {
            if (!success) {
                NSLog(@"Failed to open URL in external browser: %@", URL);
            }
        }];
        return NO; // Indicate that we have handled the URL interaction
    } else {
        NSLog(@"Cannot open URL in external browser: %@", URL);
        return NO; // Failed to open, but we tried to handle it
    }
    // For other data detector types (e.g., mailto, tel), let the system handle them
    return YES;
}

@end
