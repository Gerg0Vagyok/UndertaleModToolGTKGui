// This whole thing is held together by ducktape

using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;

namespace UndertaleModToolGtk {
	class Program : Window {
		public enum ItemType { // I hope i dont have to explain these.
			MenuItem,
			Separator
		}
		
		private class CategoriesManager { // The CategoriesManager class. This manages and does stuff for the categories. Makes it easier to use.
			public class Category { // The Category class. This stores the individual categories.
				private readonly Expander CategoryExpander;
				private readonly string CategoryName;
				private readonly ListBox CategoryListbox = new ListBox();
				private readonly Dictionary<string, Label> ListOfAllLabels = new Dictionary<string, Label>();
				private readonly Action<string, string, bool, bool> Select;
				private readonly Action<Category> UnselectOthers;
				private ListBoxRow SelectedRow = null;

				public string GetName() { // Get the name of the category. Unlocalized.
					return CategoryName;
				}

				public void UnselectAll() { // Unselect all rows, used outside and inside as well.
					CategoryListbox.UnselectAll();
				}

				public Label GetLabel(string Name) { // Get a label, used outside, i think.
					return ListOfAllLabels[Name];
				}

				public void SelectLabel(string Name) { // Unselect everything besides 1 label.
					UnselectOthers(this);
					UnselectAll();
					CategoryListbox.SelectRow(ListOfAllLabels[Name].Parent as ListBoxRow);
				}

				public void Search(string SearchString) {  // Search function.
					foreach(string Key in ListOfAllLabels.Keys) {
						if (!Key.Contains(SearchString)) { // Hide all elements that dont contain the SearchString
							ListOfAllLabels[Key].Parent.NoShowAll = true;
							ListOfAllLabels[Key].Parent.Visible = false;
						} else { 						   // Show everything else
							ListOfAllLabels[Key].Parent.NoShowAll = false;
							ListOfAllLabels[Key].Parent.Visible = true;
						}
					}
				}

				public void LoadString(string Name) { // Load data from a singular string.
					Label NewLabel = new Label(Name);
					NewLabel.UseUnderline = false;
					NewLabel.Halign = Align.Start;
					CategoryListbox.Add(NewLabel);
					ListOfAllLabels.Add(Name, NewLabel);
					NewLabel.Parent.Hexpand = true;
					NewLabel.Parent.MarginStart = 15;
				}

				public void LoadArray(string[] Names) { // Load data from an array of strings. uses LoadString under the hood.
					foreach(string Name in Names) {
						LoadString(Name);
					}
				}

				public Expander GetExpander() { // For localization and stuff.
					return CategoryExpander;
				}

				public void Clear() { // Clear all stuff from the expanders. Will be used for loading a new file for example.
					foreach(Widget LabelEl in CategoryListbox.Children) {
						CategoryListbox.Remove(LabelEl);
						LabelEl.Destroy();
					}
					ListOfAllLabels.Clear();
				}

				public Category(string Name, Action<string, string, bool, bool> SelectFunc, Action<Category> UnselectOthersFunc) {
					UnselectOthers = UnselectOthersFunc;
					Select = SelectFunc; // Set function pointer things.

					CategoryExpander = new Expander(Name); // Initalize the expander and set some properties
					CategoryExpander.Child = CategoryListbox;
					CategoryExpander.Halign = Align.Fill;

					CategoryListbox.MarginStart = 15;
					CategoryListbox.StyleContext.AddClass("listbox"); // Set some properties for the listbox
					CategoryListbox.Hexpand = true;
					CategoryListbox.Halign = Align.Fill;
					CategoryListbox.RowSelected += (o, args) => { // Code to select items to show on the right panel.
						if (args.Row != null && (SelectedRow == null || SelectedRow != args.Row)) {
							SelectedRow = args.Row;
							Select((args.Row.Children[0] as Label).Text, Name, false, false); // This somehow works. tho it breaks if its not a label.
						}															   // Not the only thing that would.
					};
					CategoryName = Name;
				}
			}

			private int BackListIndex = -1;
			private readonly List<(string Name, string CategoryName)> BackList = new List<(string Name, string CategoryName)>(); // I swear i can name variables properly
			private readonly Dictionary<string, Category> Categories = new Dictionary<string, Category>();

			public Category GetCategory(string Name) { // Get a category by name.
				return Categories[Name];
			}

			public Category New(string Name) { // Add a new category.
				Category NewCategory = new Category(Name, Select, UnselectOthers);
				Categories[Name] = NewCategory;

				return NewCategory;
			}

			public void Search(string SearchString) { // Search, the one outside stuff uses.
				foreach (Category Cat in Categories.Values) {
					Cat.Search(SearchString);
				}
			}

			public void Back() { // Go back in the selection.
				if (BackListIndex > 0) {
					Select(BackList[BackListIndex-1].Name, BackList[BackListIndex-1].CategoryName, true, false);
				}
			}

			public void Forward() { // Go forward in the selection.
				if (BackListIndex+1 < BackList.Count()) {
					Select(BackList[BackListIndex+1].Name, BackList[BackListIndex+1].CategoryName, false, true);
				}
			}

			private void Select(string SelectedName, string CategoryName, bool IsBack, bool IsForward) { // Rewite the whole thing.
				if (Categories.ContainsKey(CategoryName)) {
					if (!IsBack && !IsForward) {
						int BLL = BackList.Count();  // BackListLength, to make the if a little shorter, IT WORKS
						if ((BLL != 0 && BackListIndex > -1 && BackList[BackListIndex] != (SelectedName, CategoryName) && BackListIndex < BLL) || BLL == 0 || BackListIndex < 0) {
							if (BLL > 0 && BackListIndex < BLL) {
								BackList.RemoveRange(BackListIndex+1, BLL - BackListIndex - 1);
							}
							BackList.Add((SelectedName, CategoryName));
							BackListIndex++;
						} else {
							return;
						}
					} else if (IsBack) {
						BackListIndex--;
					} else if (IsForward) {
						BackListIndex++;
					}
					//Console.WriteLine("BackList: [" + String.Join(", ", BackList) + "] - BackListIndex: " + BackListIndex + " - IsBack: " + IsBack + " - IsForward: " + IsForward);
					// If needed in the future, its just commented out.
					Categories[CategoryName].SelectLabel(SelectedName);
				}
			}

			private void UnselectOthers(Category category) { // Unselect all categories besides one.
				foreach(Category cat in Categories.Values) {
					if (cat != category) {
						cat.UnselectAll();
					}
				}
			}
		}

		private class TitleBarManager {
			public class TitleBarItem {
				private readonly List<MenuItem> Items = new List<MenuItem>();
				private readonly Menu MenuMain = new Menu();
				private readonly MenuItem MenuSub;

				private bool ItemsContains(string Name) {
					foreach(MenuItem Item in Items) {
						if (Item.Name == Name) {
							return true;
						}
					}
					return false;
				}

				public MenuItem Add(ItemType Type, string Name = "", Action<object, EventArgs> Func = null) {
					switch(Type) {
						case ItemType.MenuItem:
							if (Func != null && Name != "" && !ItemsContains(Name)) {
								MenuItem NewItem = new MenuItem($"{Name}");
								NewItem.Activated += new EventHandler(Func);
								Items.Add(NewItem);
								MenuMain.Append(NewItem);
								return NewItem;
							}
							break;
						case ItemType.Separator:
							SeparatorMenuItem NewSeparator = new SeparatorMenuItem();
							Items.Add(NewSeparator);
							return NewSeparator;
					}
					return null;
				}

				public List<MenuItem> GetItems() {
					return Items;
				}

				public MenuItem GetMenu() {
					return MenuSub;
				}

				public TitleBarItem(string Name) {
					MenuSub = new MenuItem(Name);
					MenuSub.Submenu = MenuMain;
				}
			}

			private readonly Dictionary<string, TitleBarItem> Menus = new Dictionary<string, TitleBarItem>();
			private readonly MenuBar TitleBar = new MenuBar();

			public MenuItem AddMenuSeparator(string MenuName) {
				return Menus[MenuName].Add(ItemType.Separator);
			}

			public MenuItem AddMenuItem(string MenuName, string ItemName, Action<object, EventArgs> Func) {
				return Menus[MenuName].Add(ItemType.MenuItem, ItemName, Func);
			}

			public TitleBarItem AddMenu(string Name) {
				TitleBarItem NewTitleBarItem = new TitleBarItem(Name);
				TitleBar.Append(NewTitleBarItem.GetMenu());
				Menus[Name] = NewTitleBarItem;
				return NewTitleBarItem;
			}

			public MenuBar GetMenuBar() {
				return TitleBar;
			}
		}

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
		private readonly MenuBar TitleBar;

		private Program() : base("Unofficial UndertaleModTool") {
			SoundsCategory = Categories.New("CAT_SOUNDS");
			SpritesCategory = Categories.New("CAT_SPRITES");
			TitleBar = MenuBarManager.GetMenuBar();
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
