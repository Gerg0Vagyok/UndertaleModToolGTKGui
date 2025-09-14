// This whole thing is held together by ducktape

using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;

namespace UndertaleModToolGtk
{
	class Program : Window
	{
		private class CategoriesManager { // The CategoriesManager class. This manages and does stuff for the categories. Makes it easier to use.
			public class Category { // The Category class. This stores the individual categories.
				private Expander CategoryExpander;
				
				private string CategoryName;
				private ListBox CategoryListbox = new ListBox();
				private ListBoxRow SelectedRow = null;
				private Dictionary<string, Label> ListOfAllLabels = new Dictionary<string, Label>();
				private Action<string, string, bool, bool> Select;
				private Action<Category> UnselectOthers;

				public String GetName() { // Get the name of the category. Unlocalized.
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
			private List<(string Name, string CategoryName)> BackList = new List<(string Name, string CategoryName)>(); // I swear i can name variables properly

			private Dictionary<string, Category> Categories = new Dictionary<string, Category>();

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

			private void UnselectOthers(Category category) { // Unselect all categories besides 1.
				foreach(Category cat in Categories.Values) {
					if (cat != category) {
						cat.UnselectAll();
					}
				}
			}
		}

		private int Width = 1200;
		private int MinWidth = 300;
		private int Height = 650;
		private int MinHeight = 100;

		private Paned Seperator = new Paned(Orientation.Horizontal);
		private Box MainLeft = new Box(Orientation.Vertical, 0);
		private Box MainRight = new Box(Orientation.Vertical, 0);
		private Box MainBox = new Box(Orientation.Vertical, 0);

		private Localization Localizer = new Localization();

		private CategoriesManager Categories = new CategoriesManager();
		private CategoriesManager.Category SoundsCategory;
		private CategoriesManager.Category SpritesCategory;

		private CssProvider CSS = new CssProvider();

		private MenuBar TitleBar = new MenuBar();

		private Program() : base("Unofficial UndertaleModTool") {
			SoundsCategory = Categories.New("CAT_SOUNDS");
			SpritesCategory = Categories.New("CAT_SPRITES");
			
			SetDefaultSize(Width, Height);
			SetSizeRequest(MinWidth, MinHeight);

			CSS.LoadFromData(String.Join( // My editor hated this.
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

			MenuItem LangTest = new MenuItem("LangTest");
			LangTest.Activated += (o, args) => {
				if (Localizer.GetLanguage() == "English") {
					Localizer.SetLanguage("TestLang");
				} else {
					Localizer.SetLanguage("English");
				}
			};
			fileMenu.Append(LangTest);
			
			Menu fileMenu2 = new Menu();
			MenuItem file2 = new MenuItem("File2");
			file2.Submenu = fileMenu2;
			
			MenuItem open2 = new MenuItem("Open2");
			fileMenu2.Append(open2);

			TitleBar.Append(file);
			TitleBar.Append(file2);

			MainBox.PackStart(TitleBar, false, false, 0);

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

			Seperator.Pack1(MainLeft, true, false);
			Seperator.Pack2(MainRight, true, false);

			Seperator.Position = Width / 4;

			DeleteEvent += Window_DeleteEvent;

			MainLeft.Margin = 5;
			MainRight.Margin = 5;

			MainBox.PackEnd(Seperator, true, true, 0);

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

		public static void Main(string[] args) { // The main function.
			Application.Init();
			Program Win = new Program();
			Application.Run();
		}
	}
}
