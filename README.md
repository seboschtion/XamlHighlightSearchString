# XamlHighlightSearchString

## In short
A behaviour for TextBlocks that allows them to highlight a bindable substring. Usable with the Windows 10 SDK.
![Example 1](https://github.com/seboschtion/XamlHighlightSearchString/blob/master/example.png)

## In long
Bind a text to this behaviour and the matching text in the TextBlock will be highlighted.
To use this behaviour you must install the [Microsoft.Xaml.Behaviours.Uwp](https://blogs.windows.com/buildingapps/2015/11/30/xaml-behaviors-open-source-and-on-uwp/) Nuget package and rename the namespace of the classes.
You can use the behaviour on a TextBlock like this:
```xml
<TextBlock Text="{Binding}">
    <interactivity:Interaction.Behaviors>
        <behaviours:HighlightSearchStringBehaviour HighlightForeground="DodgerBlue"
                                                   SearchString="{Binding ...}"/>
    </interactivity:Interaction.Behaviors>
</TextBlock>
```
Of course this can be used in a ListView, too.

### Properties
#### HighlightForeground `Brush`
default: `null`
The color of the matching text.

#### SearchString `string`
default: `string.Empty`
The (sub)string to highlight.

#### ImproperMatch `bool`
default: `true`
If true, a space in the SearchString will separate different search strings. If false, the whole SearchString must match to highlight some text.
![Example 2](https://github.com/seboschtion/XamlHighlightSearchString/blob/master/example2.png)

#### ImproperMatch `RegexOptions`
default: `RegexOptions.IgnoreCase`
Search is case insensitive by default, but you can give other or more RegexOptions.