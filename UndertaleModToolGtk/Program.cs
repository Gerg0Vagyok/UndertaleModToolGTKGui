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

				private String CategoryName;
				private ListBox CategoryListbox = new ListBox();
				private ListBoxRow SelectedRow = null;
				private Dictionary<String, Label> ListOfAllLabels = new Dictionary<String, Label>();
				private Action<String, String, Boolean, Boolean> Select;
				private Action<Category> UnselectOthers;

				public String GetName() { // Get the name of the category. Unlocalized.
					return CategoryName;
				}

				public void UnselectAll() { // Unselect all rows, used outside and inside as well.
					CategoryListbox.UnselectAll();
				}

				public Label GetLabel(String Name) { // Get a label, used outside, i think.
					return ListOfAllLabels[Name];
				}

				public void SelectLabel(String Name) { // Unselect everything besides 1 label.
					UnselectOthers(this);
					UnselectAll();
					CategoryListbox.SelectRow(ListOfAllLabels[Name].Parent as ListBoxRow);
				}

				public void Search(String SearchString) {  // Search function.
					foreach(String Key in ListOfAllLabels.Keys) {
						if (!Key.Contains(SearchString)) { // Hide all elements that dont contain the SearchString
							ListOfAllLabels[Key].Parent.NoShowAll = true;
							ListOfAllLabels[Key].Parent.Visible = false;
						} else { 						   // Show everything else
							ListOfAllLabels[Key].Parent.NoShowAll = false;
							ListOfAllLabels[Key].Parent.Visible = true;
						}
					}
				}

				public void LoadString(String Name) { // Load data from a singular string.
					Label NewLabel = new Label(Name);
					NewLabel.UseUnderline = false;
					NewLabel.Halign = Align.Start;
					CategoryListbox.Add(NewLabel);
					ListOfAllLabels.Add(Name, NewLabel);
					NewLabel.Parent.Hexpand = true;
					NewLabel.Parent.MarginStart = 15;
				}

				public void LoadArray(String[] Names) { // Load data from an array of strings. uses LoadString under the hood.
					foreach(String Name in Names) {
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

				public Category(String Name, Action<String, String, Boolean, Boolean> SelectFunc, Action<Category> UnselectOthersFunc) {
					UnselectOthers = UnselectOthersFunc;
					Select = SelectFunc; // Set function pointer things.

					CategoryExpander = new Expander(Name); // Initalize the expander and set some properties
					CategoryExpander.Add(CategoryListbox);
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

			private int BackListIndex = 0;
			private List<(Category category, String name)> BackList = new List<(Category category, String name)>(); // I swear i can name variables properly
			private (Category category, String name) CurrentlySelected;
			private Dictionary<String, Category> Categories = new Dictionary<String, Category>();

			public Category GetCategory(String Name) { // Get a category by name.
				return Categories[Name];
			}

			public Category New(String Name) { // Add a new category.
				Category NewCategory = new Category(Name, Select, UnselectOthers);
				Categories[Name] = NewCategory;

				return NewCategory;
			}

			public void Search(String SearchString) { // Search, the one outside stuff uses.
				foreach (Category Cat in Categories.Values) {
					Cat.Search(SearchString);
				}
			}

			public void Back() { // Go back in the selection.
				if (BackList.Count() > 0 && BackListIndex-1 >= 0) {
					Select(BackList.Last().name, BackList[BackListIndex-1].category.GetName(), true, false);
					BackListIndex--;
				}
			}

			public void Forward() { // Go forward in the selection.
				if (BackList.Count() > 0 && BackListIndex+1 < BackList.Count()) {
					Console.WriteLine("asdasd");
					Select(BackList.Last().name, BackList[BackListIndex+1].category.GetName(), true, false);
					BackListIndex++;
				}
			}

			private void Select(String SelectedName, String CategoryName, Boolean IsBack, Boolean IsForward) { // This is the select function, idk what to tell u its complicated.
				if (Categories.ContainsKey(CategoryName)) {
					if (!IsBack && !IsForward) {
						if (CurrentlySelected.name != null && ((BackList.Count() > 0 && CurrentlySelected != BackList.Last()) || BackList.Count() == 0)) {
							BackList.Add(CurrentlySelected);
							BackListIndex++;
						}
						CurrentlySelected = (Categories[CategoryName], SelectedName);
					} else if (IsBack) {
						if (BackList.Count() < 2) {
							BackList.Clear();
							CurrentlySelected = (Categories[CategoryName], SelectedName);
						} else {
							BackList.RemoveAt(BackList.Count() - 1);
							CurrentlySelected = BackList.Last();
						}
					} else if (IsForward) {
						if (BackList.Count() < 2) {
							BackList.Clear();
							CurrentlySelected = (Categories[CategoryName], SelectedName);
						} else {
							BackList.RemoveAt(BackList.Count() - 1);
							CurrentlySelected = BackList.Last();
						}
					}
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

			CSS.LoadFromData(String.Join( // My editor hates this.
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

			MainLeft.SetSizeRequest(MinWidth / 2, MinHeight);
			MainRight.SetSizeRequest(MinWidth / 2, MinHeight);

			ResizeChecked += (o, args) => {
				Width = Window.Width;
				Height = Window.Height;
			};

			Button LanguageTest = new Button("LangTest"); // Simple language test button.
			LanguageTest.Clicked += btn_clicked;

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

			MainRight.Add(LanguageTest);

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

		private CssProvider CreateCssProviderFromData(String Css) { // Load css from a string, and return the CSSProvider.
			var CSSProvider = new CssProvider();
			CSSProvider.LoadFromData(Css);
			return CSSProvider;
		}

		private void SpriteItemCliced(string ItemName) { // Is this even used?
			Console.WriteLine(ItemName);
		}

		private void Window_DeleteEvent(object sender, DeleteEventArgs a) { // The exit event.
			Application.Quit();
		}

		private void btn_clicked(object sender, EventArgs e) { // Simple language test button click function.
			if (Localizer.GetLanguage() == "English") {
				Localizer.SetLanguage("TestLang");
			} else {
				Localizer.SetLanguage("English");
			}
		}


		public static void Main(string[] args) { // The main function.
			Application.Init();
			var Win = new Program();
			Application.Run();
		}
	}
}
