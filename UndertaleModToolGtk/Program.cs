//using System;
using Gtk;

namespace UndertaleModToolGtk
{
	class Program : Window
	{
		private int Width = 1200;
		private int MinWidth = 300;
		private int Height = 650;
		private int MinHeight = 100;

		private Paned Seperator = new Paned(Orientation.Horizontal);
		private Box MainLeft = new Box(Orientation.Vertical, 0);
		private Box MainRight = new Box(Orientation.Vertical, 0);
		private Box MainBox = new Box(Orientation.Vertical, 0);

		private CssProvider CSS = new CssProvider();

		private MenuBar TitleBar = new MenuBar();

		private Program() : base("Unofficial UndertaleModTool") {
			SetDefaultSize(Width, Height);
			SetSizeRequest(MinWidth, MinHeight);

			CSS.LoadFromData("* { font-size: 10pt; }");
			StyleContext.AddProviderForScreen(Gdk.Screen.Default, CSS, 800);

			Menu fileMenu = new Menu();
			MenuItem file = new MenuItem("File");
			file.Submenu = fileMenu;

			MenuItem open = new MenuItem("Open");
			fileMenu.Append(open);

			MenuItem settings = new MenuItem("Settings");
			fileMenu.Append(settings);

			TitleBar.Append(file);

			MainBox.PackStart(TitleBar, false, false, 0);

			MainLeft.SetSizeRequest(MinWidth / 2, Height);
			MainRight.SetSizeRequest(MinWidth / 2, Height);

			Label label = new Label("Hello, World!");
			MainRight.Add(label);

			Expander TestExpander = new Expander("Sprites");
			TestExpander.Halign = Align.Start;
			Box content = new Box(Orientation.Vertical, 0);
			content.PackStart(new Label("Hidden content"), false, true, 0);
			TestExpander.Add(content);

			Expander TestExpander2 = new Expander("Sprites");
			TestExpander2.Halign = Align.Start;
			Box content2 = new Box(Orientation.Vertical, 0);
			content2.PackStart(new Label("Hidden content"), false, true, 0);
			TestExpander2.Add(content);

			Frame FrameExpander = new Frame(); 
			FrameExpander.Add(TestExpander);
			FrameExpander.Add(TestExpander2);

			MainLeft.PackStart(FrameExpander, true, true, 0);

			Seperator.Pack1(MainLeft, true, false);
			Seperator.Pack2(MainRight, true, false);

			Seperator.Position = Width / 4;

			DeleteEvent += Window_DeleteEvent;

			MainBox.PackEnd(Seperator, false, false, 0);

			Add(MainBox);

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
