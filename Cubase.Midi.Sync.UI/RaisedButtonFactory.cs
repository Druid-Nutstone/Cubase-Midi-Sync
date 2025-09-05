using Cubase.Midi.Sync.Common.Colours;
using Cubase.Midi.Sync.UI.Extensions;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace Cubase.Midi.Sync.UI;

public static class RaisedButtonFactory
{
    public record RaisedButton(Button Button);

    public static RaisedButton Create(string text, SerializableColour backgroundColour, SerializableColour textColour,  EventHandler onClicked, bool toggleMode = false)
    {
        var button = new Button
        {
            Text = text,
            HeightRequest = 60,
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
            button.Clicked += onClicked;

        button.SizeChanged += (s, e) =>
        {
            if (button.Height > 0)
                button.FontSize = Math.Min(button.Height * 0.5, 18);
        };

        return new RaisedButton(button);
    }
}
