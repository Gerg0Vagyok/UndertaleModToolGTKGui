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

		private Localization Localizer;

		private Expander SpriteCategory = new Expander("Sprites");

		public string Language = "Default";

		private CssProvider CSS = new CssProvider();

		private MenuBar TitleBar = new MenuBar();

		private Program() : base("Unofficial UndertaleModTool") {
			Localizer = new Localization();
			SetDefaultSize(Width, Height);
			SetSizeRequest(MinWidth, MinHeight);
			CSS.LoadFromData(String.Join(
				Environment.NewLine,
				"* {",
				"	font-size: 10pt;", 
				"} ", 
				".listbox, row:not(:selected):not(:hover):not(:focus):not(:active) {", 
				"	background-color: transparent;", 
				"} "
			));
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

			Expander SpritesCategoryExpander = new Expander("CAT_SPRITES");
			SpritesCategoryExpander.Halign = Align.Start;
			ListBox SpritesCategoryContent = new ListBox();
			SpritesCategoryContent.MarginStart = 15;
			SpritesCategoryContent.StyleContext.AddClass("listbox");
			SpritesCategoryContent.Add(new Label("asdasd"));
			SpritesCategoryContent.Add(new Label("asdasd"));
			SpritesCategoryContent.Add(new Label("asdasd"));
			SpritesCategoryExpander.Add(SpritesCategoryContent);

			SpritesCategoryContent.RowSelected += (o, args) => {
				var row = args.Row;
				Console.WriteLine($"Clicked on: {row}");
				// Add logic to make that it check doubleclicks and is shared across multiple categories so use a outisde, and make it into a proper function
			};

			Expander SoundsCategoryExpander = new Expander("CAT_SOUNDS");
			SoundsCategoryExpander.Halign = Align.Start;
			ListBox SoundsCategoryContent = new ListBox();
			SoundsCategoryContent.MarginStart = 15;
			SoundsCategoryContent.StyleContext.AddClass("listbox");
			SoundsCategoryContent.Add(new Label("asdasd"));
			SoundsCategoryContent.Add(new Label("asdasd"));
			SoundsCategoryContent.Add(new Label("asdasd"));
			SoundsCategoryExpander.Add(SoundsCategoryContent);

			Frame LeftFrame = new Frame(); 
			Box LeftFrameBox = new Box(Orientation.Vertical, 0);
			LeftFrameBox.PackStart(SoundsCategoryExpander, false, false, 0);
			LeftFrameBox.PackStart(SpritesCategoryExpander, false, false, 0);
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

			Localizer.Add(SpritesCategoryExpander);
			Localizer.Add(SoundsCategoryExpander);

			ShowAll();
		}

		private CssProvider CreateCssProviderFromData(String Css) {
			var CSSProvider = new CssProvider();
			CSSProvider.LoadFromData(Css);
			return CSSProvider;
		}

		private void SpriteItemCliced(string ItemName) {
			Console.WriteLine(ItemName);
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) {
			Application.Quit();
		}

		private void btn_clicked(object sender, EventArgs e) {
			if (Localizer.GetLanguage() == "English") {
				Localizer.SetLanguage("TestLang");
			} else {
				Localizer.SetLanguage("English");
			}
		}


		public static void Main(string[] args) {
			Application.Init();
			var Win = new Program();
			Application.Run();
		}
	}
}
