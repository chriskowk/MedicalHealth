using System.Reflection;
using System.Text;
using System.Windows.Input;
using CefSharp;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace KnowledgeBase.ViewModels
{
    /// <summary>
    /// AppViewModel manages the appplications state and its main objects.
    /// </summary>
    public class AppViewModel : ViewModelBase
    {
        public const string _testResourceUrl = "http://test/resource/load";
        public const string _testBaiduResourceUrl = "https://www.baidu.com";
        public const string _testUnicodeResourceUrl = "http://test/resource/loadUnicode";

        private readonly string _assemblyTitle;
        private string _browserAddress;

        /// <summary>
        /// Get test Command to browse to a test URL ...
        /// </summary>
        public ICommand TestUrl0Command { get; private set; }

        /// <summary>
        ///  Get test Command to browse to a test URL ...
        /// </summary>
        public ICommand TestUrl1Command { get; private set; }

        /// <summary>
        ///  Get test Command to browse to a test URL 1 ...
        /// </summary>
        public ICommand TestUrl2Command { get; private set; }

        /// <summary>
        ///  AppViewModel Constructor
        /// </summary>
        public AppViewModel()
        {
            _assemblyTitle = Assembly.GetEntryAssembly().GetName().Name;
            BrowserAddress = _testBaiduResourceUrl;

            TestUrl0Command = new RelayCommand(() =>
            {
                // Setting this address sets the current address of the browser
                // control via bound BrowserAddress property
                BrowserAddress = _testResourceUrl;
            });

            TestUrl1Command = new RelayCommand(() =>
            {
                // Setting this address sets the current address of the browser
                // control via bound BrowserAddress property
                BrowserAddress = _testBaiduResourceUrl;
            });

            TestUrl2Command = new RelayCommand(() =>
            {
                // Setting this address sets the current address of the browser
                // control via bound BrowserAddress property
                BrowserAddress = _testUnicodeResourceUrl;
            });
        }

        /// <summary>
        /// Get/set current address of web browser URI.
        /// </summary>
        public string BrowserAddress
        {
            get { return _browserAddress; }

            set
            {
                if (_browserAddress != value)
                {
                    _browserAddress = value;
                    RaisePropertyChanged(() => BrowserAddress);
                    RaisePropertyChanged(() => BrowserTitle);
                }
            }
        }

        /// <summary>
        /// Get "title" - "Uri" string of current address of web browser URI
        /// for display in UI - to let the user know what his looking at.
        /// </summary>
        public string BrowserTitle
        {
            get { return string.Format("{0} - {1}", _assemblyTitle, _browserAddress); }
        }

        /// <summary>
        /// Registers 2 Test URIs with HTML loaded from strings
        /// </summary>
        /// <param name="browser"></param>
        public static void RegisterTestResources(IWebBrowser browser)
        {
            var factory = browser.ResourceHandlerFactory;

            if (factory != null)
            {
                string responseBody =
                    "<html>"
                    + "<body><h1>About</h1>"
                    + "<p>This sample application implements a <b>ResourceHandler</b> "
                    + "which can be used to fullfil custom network requests as explained here:"
                    + "<a href=\"http://www.codeproject.com/Articles/881315/Display-HTML-in-WPF-and-CefSharp-Tutorial-Part 2\">http://www.codeproject.com/Articles/881315/Display-HTML-in-WPF-and-CefSharp-Tutorial-Part 2</a>"
                    + ".</p>"
                    + "<hr/><p>This sample is based on CefSharp <b>39.0.0</b></p>"
                    + "<hr/>"
                    + "<p>See also CefSharp on GitHub: <a href=\"https://github.com/cefsharp\">https://github.com/cefsharp</a><br/>"
                    + "<p>and Cef at Google: <a href=\"https://code.google.com/p/chromiumembedded/wiki/GeneralUsage#Request_Handling\">https://code.google.com/p/chromiumembedded/wiki/GeneralUsage#Request_Handling</a>"
                    + "</body></html>";

                factory.RegisterHandler(_testResourceUrl, ResourceHandler.FromString(responseBody));

                string unicodeResponseBody = "<html><body>整体满意度</body></html>";
                factory.RegisterHandler(_testUnicodeResourceUrl, ResourceHandler.FromString(unicodeResponseBody, Encoding.Unicode));
            }
        }
    }
}