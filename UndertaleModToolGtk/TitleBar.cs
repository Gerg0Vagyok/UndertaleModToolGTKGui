using System.Collections.Generic;
using System;
using Gtk;

namespace UndertaleModToolGtk {
	public class TitleBarManager {
		public class TitleBarItem {
			private readonly List<MenuItem> Items = new List<MenuItem>();
			private readonly Menu MenuMain = new Menu();
			private readonly MenuItem MenuSub;

			private bool ItemsContains(string Name) { // Check if the list already has an item with a specific name
				foreach(MenuItem Item in Items) {
					if (Item.Name == Name) {
						return true;
					}
				}
				return false;
			}

			public MenuItem Add(ItemType Type, string Name = "", Action<object, EventArgs> Func = null) { // New menu item
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

			public List<MenuItem> GetItems() { // Get all items
				return Items;
			}

			public MenuItem GetMenu() { // Get the submenu
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
}
