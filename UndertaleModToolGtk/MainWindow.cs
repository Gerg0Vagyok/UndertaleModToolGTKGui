using System;
using Gtk;
using UI = Gtk.Builder.ObjectAttribute;

namespace UndertaleModToolGtk {
	class MainWindow : Window {
		public MainWindow() : base("Idk") {
			SetDefaultSize(1200, 650);


			VBox vbox = new VBox(false, 0);
			Add(vbox);

			Frame frame1 = new Frame("Top");
			frame1.SetSizeRequest(-1, -1);
			frame1.Expand = true;
			vbox.PackStart(frame1, true, true, 0);

			// Second container
			Frame frame2 = new Frame("Bottom");
			frame2.Expand = true;
			vbox.PackStart(frame2, true, true, 0);
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) {
			Application.Quit();
		}
	}
}
