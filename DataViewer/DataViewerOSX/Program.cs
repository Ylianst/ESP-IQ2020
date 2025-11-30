using System;
using AppKit;

namespace DataViewerOSX
{
    class Program
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.SharedApplication.Delegate = new AppDelegate();
            NSApplication.Main(args);
        }
    }

    class AppDelegate : NSApplicationDelegate
    {
        private MainWindowController? mainWindowController;

        public override void DidFinishLaunching(Foundation.NSNotification notification)
        {
            mainWindowController = new MainWindowController();
            mainWindowController.Window.MakeKeyAndOrderFront(null);
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender)
        {
            return true;
        }
    }
}
