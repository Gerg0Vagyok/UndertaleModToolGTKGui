using System;
using Gtk;

namespace UndertaleModToolGtk
{
	class Program : Window
	{
		private int Width = 1200;
		private int MinWidth = 300;
		private int Height = 650;
		private int MinHeight = 100;

		private HPaned Seperator;
		private Box MainLeft;
		private Box MainRight;

		private CssProvider CSS;

		private Program() : base("Unofficial Undertale Mod Tool") {
			SetDefaultSize(Width, Height);
			SetSizeRequest(MinWidth, MinHeight);

			CSS.LoadFromData("* { font-size: 18pt; }");
			StyleContext.AddProviderForScreen(Gdk.Screen.Default, CSS, 800);

			Seperator = new HPaned();

			MainLeft = new Box(Orientation.Vertical, 0);
			MainLeft.SetSizeRequest(MinWidth / 2, Height);
			MainRight = new Box(Orientation.Vertical, 0);
			MainRight.SetSizeRequest(MinWidth / 2, Height);

			Label label = new Label("Hello, World!");
			MainRight.Add(label);

			Expander TestExpander = new Expander("stuff");
			TestExpander.Halign = Align.Start;
			VBox content = new VBox();
content.PackStart(new Label("Hidden content"), false, true, 0);
TestExpander.Add(content);

			MainLeft.PackStart(TestExpander, false, false, 0);

			Seperator.Pack1(MainLeft, true, false);
			Seperator.Pack2(MainRight, true, false);

			Seperator.Position = Width / 4;

			DeleteEvent += Window_DeleteEvent;

			Add(Seperator);
			ShowAll();
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) {
			Application.Quit();
		}

		public static void Main(string[] args)
		{
			Application.Init();
			var Win = new Program();
			Application.Run();
		}
	}
}
