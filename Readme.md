
Simple Theming support for a Winforms application.
This approach will make the WinForms project a little themeable, but it's not perfect.
Because Winforms does not support theming very well.

There are several issues with theming WinForms applications.
1. System Dialogs are not theme-able.
2. Some controls do not support theming very well.
3. Scrollbars, CheckBoxes, RadioButtons ... not themed


## In Code Usage:
```C#
   new WinFormThemer(myForm) // apply the default dark theme to 'myForm'

   new WinFormThemer(myForm,themeConfig) // apply custom theme config to 'myForm'

   var themer = new WinFormThemer(); // uses default dark theme
   themer.ApplyTheme(myForm);

   var themer = new WinFormThemer(); // uses default dark theme
   themer.ApplyTheme(myForm);

   var themer = new WinFormThemer(themeConfig); // specify your custom theme
   themer.ApplyTheme(myForm);
```
## MessageForm
Since showing a message with 'MessageBox.Show(message)' is so common, an alternate messaging form is included.

```C#
    // API
        public static DialogResult ShowMessage(string message){}

        public static DialogResult ShowMessage(string message, string title){}

        public static DialogResult ShowMessage(string message, string title, bool showCancel, Icon icon){}

        public static String GetString(string initialText, string title){}

    // Usage:
    String userResponse = WinformThemer.MessageForm.GetString("Chocolate fudge", "What desert do you like?");

    WinformThemer.MessageForm.ShowMessage("You can't do that");

    if( WinformThemer.MessageForm.ShowMessage("Desert is a sweet after-dinner dish", "Do you want desert?") == DialogResult.OK)
    {
        Console.WriteLine("User wants desert");
    }
```

## VB.net support
copy and paste the code into the following web page to convert to VB.net
https://converter.telerik.com/

Note:
Event handlers may need to be corrected
```C#
form.HandleCreated += Form_HandleCreated
```
Should become:
```VB
AddHandler form.HandleCreated, AddressOf Form_HandleCreated
```