using System.Collections.Generic;
using System.Linq;
using System;
using Gtk;

namespace UndertaleModToolGtk {
	class CategoriesManager { // The CategoriesManager class. This manages and does stuff for the categories. Makes it easier to use.
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
}

