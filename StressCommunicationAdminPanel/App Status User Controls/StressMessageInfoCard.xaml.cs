using FontAwesome.Sharp;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace StressCommunicationAdminPanel.App_Status_User_Controls
{
  /// <summary>
  /// Interaction logic for StressMessageInfoCard.xaml
  /// </summary>
  public partial class StressMessageInfoCard : UserControl
  {
    public StressMessageInfoCard()
    {
      InitializeComponent();
    }

    public string CardTitle
    {
      get
      {
        return (string)GetValue(CardTitleProperty);
      }
      set
      {
        SetValue(CardTitleProperty, value);
      }
    }

    public static readonly DependencyProperty CardTitleProperty = DependencyProperty.Register("CardTitle", typeof(string), typeof(StressMessageInfoCard));

    public string CardDescription
    {
      get
      {
        return (string)GetValue(CardDescriptionProperty);
      }
      set
      {
        SetValue(CardDescriptionProperty, value);
      }
    }

    public static readonly DependencyProperty CardDescriptionProperty = DependencyProperty.Register("CardDescription", typeof(string), typeof(StressMessageInfoCard));

    public IconChar CardIcon
    {
      get
      {
        return (IconChar)GetValue(CardIconProperty);
      }
      set
      {
        SetValue(CardIconProperty, value);
      }
    }

    public static readonly DependencyProperty CardIconProperty = DependencyProperty.Register("CardIcon", typeof(IconChar), typeof(StressMessageInfoCard));

    public Brush CardIconColor
    {
      get
      {
        return (Brush)GetValue(CardIconColorProperty);
      }
      set
      {
        SetValue(CardIconColorProperty, value);
      }
    }

    public static readonly DependencyProperty CardIconColorProperty = DependencyProperty.Register("CardIconColor", typeof(Brush), typeof(StressMessageInfoCard));

    public Color CardBackgroundGradientLeft
    {
      get
      {
        return (Color)GetValue(CardBackgroundGradientLeftProperty);
      }
      set
      {
        SetValue(CardBackgroundGradientLeftProperty, value);
      }
    }

    public static readonly DependencyProperty CardBackgroundGradientLeftProperty = DependencyProperty.Register("CardBackgroundGradientLeft", typeof(Color), typeof(StressMessageInfoCard));

    public Color CardBackgroundGradientRight
    {
      get
      {
        return (Color)GetValue(CardBackgroundGradientRightProperty);
      }
      set
      {
        SetValue(CardBackgroundGradientRightProperty, value);
      }
    }

    public static readonly DependencyProperty CardBackgroundGradientRightProperty = DependencyProperty.Register("CardBackgroundGradientRight", typeof(Color), typeof(StressMessageInfoCard));

    public Color CardEllipseBackgroundGradientLeft
    {
      get
      {
        return (Color)GetValue(CardEllipseBackgroundGradientLeftProperty);
      }
      set
      {
        SetValue(CardEllipseBackgroundGradientLeftProperty, value);
      }
    }

    public static readonly DependencyProperty CardEllipseBackgroundGradientLeftProperty = DependencyProperty.Register("CardEllipseBackgroundGradientLeft", typeof(Color), typeof(StressMessageInfoCard));

    public Color CardEllipseBackgroundGradientRight
    {
      get
      {
        return (Color)GetValue(CardEllipseBackgroundGradientRightProperty);
      }
      set
      {
        SetValue(CardEllipseBackgroundGradientRightProperty, value);
      }
    }

    public static readonly DependencyProperty CardEllipseBackgroundGradientRightProperty = DependencyProperty.Register("CardEllipseBackgroundGradientRight", typeof(Color), typeof(StressMessageInfoCard));

    public Brush CardStarStrokeColor
    {
      get
      {
        return (Brush)GetValue(CardStarStrokeColorProperty);
      }
      set
      {
        SetValue(CardStarStrokeColorProperty, value);
      }
    }

    public static readonly DependencyProperty CardStarStrokeColorProperty = DependencyProperty.Register("CardStarStrokeColor", typeof(Brush), typeof(StressMessageInfoCard));

    public Brush CardStarFillColor
    {
      get
      {
        return (Brush)GetValue(CardStarFillColorProperty);
      }
      set
      {
        SetValue(CardStarFillColorProperty, value);
      }
    }

    public static readonly DependencyProperty CardStarFillColorProperty = DependencyProperty.Register("CardStarFillColor", typeof(Brush), typeof(StressMessageInfoCard));
  }
}