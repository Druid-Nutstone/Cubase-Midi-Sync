using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.UI.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public static class RaisedButtonFactory
{


    public record RaisedButton(Button Button);

    public static RaisedButton Create(string text, SerializableColour backgroundColour, SerializableColour textColour,  EventHandler onClicked, bool toggleMode = false, string? id = null)
    {
        var displayInfo = DeviceDisplay.MainDisplayInfo;
        var screenWidth = displayInfo.Width / displayInfo.Density;   // in DIPs
        var screenHeight = displayInfo.Height / displayInfo.Density; // in DIPs

        // Example: make button 1/10 of screen width and scale font accordingly
        var buttonWidth = screenWidth / 2.5;   // wider
        var buttonHeight = screenHeight / 9;   // taller
        var shortestSide = Math.Min(screenWidth, screenHeight);
        // var fontSize = shortestSide / 20;

        var button = new Button
        {
            Text = text,
            HeightRequest = buttonHeight,
            WidthRequest = double.NaN,
            HorizontalOptions = LayoutOptions.Start,
            BackgroundColor = backgroundColour.ToMauiColour(),
            TextColor = textColour.ToMauiColour(),
            FontAttributes = FontAttributes.Bold,
            CornerRadius = 4,
            Margin = new Thickness(3),
            BorderWidth = 1, // remove platform border
            LineBreakMode = LineBreakMode.WordWrap, // allows wrapping
            TextTransform = TextTransform.None,     // preserves your arrow character
            AutomationId = id,
            Visual = VisualMarker.Default, // makes no difference?
            FontAutoScalingEnabled = false // makes no difference? 
        };

        // Visual states
        if (!toggleMode)
        {
            VisualStateManager.SetVisualStateGroups(button, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                    Name = "CommonStates",
                    States =
                    {
                        new VisualState
                        {
                            Name = "Normal",
                            Setters =
                            {
                                new Setter { Property = Button.BackgroundColorProperty, Value = backgroundColour.ToMauiColour() },
                                new Setter { Property = Button.TranslationYProperty, Value = 0 }
                            }
                        },
                        new VisualState
                        {
                            Name = "Pressed",
                            Setters =
                            {
                                new Setter { Property = Button.BackgroundColorProperty, Value = backgroundColour.ToMauiColour().AddLuminosity(40) },
                                new Setter { Property = Button.TranslationYProperty, Value = 2 }
                            }
                        }
                    }
                }
            });

        }
        else
        {
            // Toggle mode: button color controlled manually
            VisualStateManager.SetVisualStateGroups(button, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                    Name = "CommonStates",
                    States =
                    {
                        new VisualState
                        {
                            Name = "Normal",
                            Setters = { new Setter { Property = Button.TranslationYProperty, Value = 0 } }
                        },
                        new VisualState
                        {
                            Name = "Pressed",
                            Setters = { new Setter { Property = Button.TranslationYProperty, Value = 2 } }
                        }
                    }
                }
            });

       }

        if (onClicked != null)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += onClicked.Invoke;
            button.GestureRecognizers.Add(tapGesture);
        }

        button.SizeChanged += (s, e) =>
        {
            SetButtonSize();
        };

        button.Loaded += (s, e) =>
        {
            SetButtonSize();
        };

        void SetButtonSize()
        {
            var displayInfo = DeviceDisplay.MainDisplayInfo;
            var shortestSide = Math.Min(displayInfo.Width / displayInfo.Density,
                                        displayInfo.Height / displayInfo.Density);

            button.FontSize = shortestSide / 25;
        } 

        return new RaisedButton(button);
    }
}
