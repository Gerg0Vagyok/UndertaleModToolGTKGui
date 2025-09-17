// This whole thing is held together by ducktape, but it works i guess.

using System;
using Gtk;

namespace UndertaleModToolGtk {
	public enum ItemType { // I hope i dont have to explain these.
		MenuItem,
		Separator
	}

	class Program : Window {
		private int Width = 1200;
		private const int MinWidth = 300;
		private int Height = 650;
		private const int MinHeight = 100;

		private readonly Paned Separator = new Paned(Orientation.Horizontal);
		private readonly Box MainLeft = new Box(Orientation.Vertical, 0);
		private readonly Box MainRight = new Box(Orientation.Vertical, 0);
		private readonly Box MainBox = new Box(Orientation.Vertical, 0);

		private readonly Localization Localizer = new Localization();

		private readonly CategoriesManager Categories = new CategoriesManager();
		private readonly CategoriesManager.Category SoundsCategory;
		private readonly CategoriesManager.Category SpritesCategory;

		private readonly CssProvider CSS = new CssProvider();

		private readonly TitleBarManager MenuBarManager = new TitleBarManager();
		private readonly TitleBarManager.TitleBarItem MenuBarFile;
		private readonly TitleBarManager.TitleBarItem MenuBarScripts;
		private readonly TitleBarManager.TitleBarItem MenuBarHelp;

		private Program() : base("Unofficial UndertaleModTool") {
			SoundsCategory = Categories.New("CAT_SOUNDS");
			SpritesCategory = Categories.New("CAT_SPRITES");
			MenuBarFile = MenuBarManager.AddMenu("TB_FILE");
			MenuBarScripts = MenuBarManager.AddMenu("TB_SCRIPTS");
			MenuBarHelp = MenuBarManager.AddMenu("TB_HELP");

			SetDefaultSize(Width, Height);
			SetSizeRequest(MinWidth, MinHeight);

			CSS.LoadFromData(string.Join( // My editor hated this.
				Environment.NewLine,
				"* {",
				"	font-size: 10pt;", 
				"} ", 
				".listbox, row:not(:selected):not(:hover):not(:focus):not(:active) {", 
				"	background-color: transparent;", 
				"} "
			));
			StyleContext.AddProviderForScreen(Gdk.Screen.Default, CSS, 800);

			MenuBarManager.AddMenuItem("TB_FILE", "TB_FILE_LANGTEST", (o, args) => {
				if (Localizer.GetLanguage() == "English") {
					Localizer.SetLanguage("TestLang");
				} else {
					Localizer.SetLanguage("English");
				}
			});
			
			Menu fileMenu2 = new Menu();
			MenuItem file2 = new MenuItem("File2");
			file2.Submenu = fileMenu2;
			
			MenuItem open2 = new MenuItem("Open2");
			fileMenu2.Append(open2);

			var test = new MenuBar();
			test.Append(file2);

			MainBox.PackStart(MenuBarManager.GetMenuBar(), false, false, 0);

			MainLeft.SetSizeRequest(MinWidth / 2, MinHeight);
			MainRight.SetSizeRequest(MinWidth / 2, MinHeight);

			ResizeChecked += (o, args) => {
				Width = Window.Width;
				Height = Window.Height;
			};

			Box BackSearchBox = new Box(Orientation.Vertical, 0);

			Entry SearchEntry = new Entry();
			SearchEntry.Changed += (s, e) => Categories.Search(SearchEntry.Text);
			Box BackForwardButtonsBox = new Box(Orientation.Horizontal, 0);

			Button BackButton = new Button("BTN_BACK");
			BackButton.Clicked += (o, args) => {Categories.Back();};
			Button ForwardButton = new Button("BTN_FORWARD");
			ForwardButton.Clicked += (o, args) => {Categories.Forward();};

			BackForwardButtonsBox.PackStart(BackButton, true, true, 0);
			BackForwardButtonsBox.PackStart(ForwardButton, true, true, 0);

			BackSearchBox.PackStart(BackForwardButtonsBox, false, false, 0);
			BackSearchBox.PackStart(SearchEntry, true, true, 0);

			MainLeft.PackStart(BackSearchBox, false, false, 0);

			Box RightTabsBox = new Box(Orientation.Vertical, 0);
			Stack RightTabsStack = new Stack();
			RightTabsStack.TransitionType = StackTransitionType.None;

			var label1 = new Label("Tab 1 content");
			var label2 = new Label("Tab 2 content");

			RightTabsStack.AddTitled(label1, "tab1", "Tab 1");
			RightTabsStack.AddTitled(label2, "tab2", "Tab 2");

			StackSwitcher RightTabsSwitcher = new StackSwitcher();
			RightTabsSwitcher.Stack = RightTabsStack;

			RightTabsBox.Add(RightTabsSwitcher);
			RightTabsBox.Add(RightTabsStack);

			MainRight.Add(RightTabsBox);

			SpritesCategory.LoadArray(["test1", "test2", "test3", "spr_3", "test12", "test22"]); // Load test data.
			SoundsCategory.LoadArray(["test1", "test2", "test3", "spr_3", "test12", "test22"]); // Load test data.

			Frame LeftFrame = new Frame(); 
			Box LeftFrameBox = new Box(Orientation.Vertical, 0);
			LeftFrameBox.PackStart(SoundsCategory.GetExpander(), false, false, 0);
			LeftFrameBox.PackStart(SpritesCategory.GetExpander(), false, false, 0);
			LeftFrameBox.Margin = 5;
			LeftFrame.Add(LeftFrameBox);

			MainLeft.PackStart(LeftFrame, true, true, 0);

			Separator.Pack1(MainLeft, true, false);
			Separator.Pack2(MainRight, true, false);

			Separator.Position = Width / 4;

			DeleteEvent += Window_DeleteEvent;

			MainLeft.Margin = 5;
			MainRight.Margin = 5;

			MainBox.PackEnd(Separator, true, true, 0);

			Add(MainBox);

			// Localization adding widgets START
			Localizer.Add(SpritesCategory.GetExpander());
			Localizer.Add(SoundsCategory.GetExpander());
			Localizer.Add(BackButton);
			Localizer.Add(ForwardButton);
			Localizer.Add(MenuBarFile.GetMenu());
			Localizer.Add(MenuBarScripts.GetMenu());
			Localizer.Add(MenuBarHelp.GetMenu());
			Localizer.AddArr(MenuBarFile.GetItems());
			// Localization adding widgets END

			ShowAll();
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) { // The exit event.
			Application.Quit();
		}


		/* UselessStuffToMakeTheLSPShutUp*/
		private readonly char[] UATMTLSPSU = ['1','2'];
		private void UFTMTLSPSU() {UATMTLSPSU[0] = UATMTLSPSU[1];}

		public static void Main() { // The main function.
			Application.Init();
			Program Win = new Program();
			Win.UFTMTLSPSU();
			Application.Run();
		}
	}
}
