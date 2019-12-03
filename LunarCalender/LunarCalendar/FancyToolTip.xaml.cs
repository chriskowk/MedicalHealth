using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LunarCalendar
{
    /// <summary>
    /// Interaction logic for FancyToolTip.xaml
    /// </summary>
    public partial class FancyToolTip
    {
        #region dependency property

        /// <summary>
        /// The tooltip title.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title",
                                        typeof(string),
                                        typeof(FancyToolTip),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="TitleProperty"/>
        /// dependency property:<br/>
        /// The tooltip title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// The tooltip details.
        /// </summary>
        public static readonly DependencyProperty InfoTextProperty =
            DependencyProperty.Register("InfoText",
                                        typeof(string),
                                        typeof(FancyToolTip),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="InfoTextProperty"/>
        /// dependency property:<br/>
        /// The tooltip details.
        /// </summary>
        public string InfoText
        {
            get { return (string)GetValue(InfoTextProperty); }
            set { SetValue(InfoTextProperty, value); }
        }

        /// <summary>
        /// The tooltip footer.
        /// </summary>
        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer",
                                        typeof(string),
                                        typeof(FancyToolTip),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="FooterProperty"/>
        /// dependency property:<br/>
        /// The tooltip footer.
        /// </summary>
        public string Footer
        {
            get { return (string)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        #endregion



        public FancyToolTip()
        {
            this.InitializeComponent();
        }

    }
}