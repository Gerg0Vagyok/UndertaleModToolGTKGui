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

		private Paned Seperator = new Paned(Orientation.Horizontal);
		private Box MainLeft = new Box(Orientation.Vertical, 0);
		private Box MainRight = new Box(Orientation.Vertical, 0);
		private Box MainBox = new Box(Orientation.Vertical, 0);

		private Expander SpriteCategory = new Expander("Sprites");

		public string Language = "Default";

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

			Button TestBuddon;
			TestBuddon = new Button("asd");

			TestBuddon.Clicked += btn_clicked;
			MainRight.Add(TestBuddon);

			Expander TestExpander = new Expander("Sprites");
			TestExpander.Halign = Align.Start;
			Box content = new Box(Orientation.Vertical, 0);
			content.MarginStart = 15;
			content.PackStart(new Label("Hidden content"), false, true, 0);
			TestExpander.Add(content);

			Expander TestExpander2 = new Expander("Sprites2");
			TestExpander2.Halign = Align.Start;
			Box content2 = new Box(Orientation.Vertical, 0);
			content2.MarginStart = 15;
			content2.PackStart(new Label("Hidden content2"), false, true, 0);
			TestExpander2.Add(content2);

			Frame LeftFrame = new Frame(); 
			Box LeftFrameBox = new Box(Orientation.Vertical, 0);
			LeftFrameBox.PackStart(TestExpander, false, false, 0);
			LeftFrameBox.PackStart(TestExpander2, false, false, 0);
			LeftFrameBox.Margin = 5;
			LeftFrame.Add(LeftFrameBox);

			MainLeft.PackStart(LeftFrame, true, true, 0);

			Seperator.Pack1(MainLeft, true, false);
			Seperator.Pack2(MainRight, true, false);

			Seperator.Position = Width / 4;

			DeleteEvent += Window_DeleteEvent;

			MainLeft.Margin = 5;
			MainRight.Margin = 5;

			MainBox.PackEnd(Seperator, false, false, 0);

			Add(MainBox);

			ShowAll();
		}

		private void SpriteItemCliced(string ItemName) {
			Console.WriteLine(ItemName);
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) {
			Application.Quit();
		}

		private void btn_clicked(object sender, EventArgs e) {
			Console.WriteLine("c# sucks");
		}


		public static void Main(string[] args) {
			Application.Init();
			var Win = new Program();
			Application.Run();
			Localization thing = new Localization();
			thing.Add(new Label("asdasd"), "asd3");
			thing.Add(new Frame("dsadsa"), "asd2");
			thing.Add(new Button("asdasd"), "asd");
			thing.Add(new Entry("asdasd"), "asd");
			thing.Add(new MenuItem("asdasd"), "asd");
			thing.Add(new Expander("asdasd"), "asd");
			thing.Update();
		}
	}
}
