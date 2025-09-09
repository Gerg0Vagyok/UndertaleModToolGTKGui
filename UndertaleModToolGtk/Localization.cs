using System;
using System.Collections.Generic;
using Gtk;

class Localization {
	public String Language;
	public List<(Widget widget, String text)> LocalizedWidgets = new List<(Widget widget, String text)>();

	public Localization(String lang = "English") {
		Language = lang;
	}

	public void Add(Widget widget, String text) {
		LocalizedWidgets.Add((widget, text));
	}

	public void Update() {
		foreach ((Widget widget, String text) WidgetEl in LocalizedWidgets) {
			if (WidgetEl.widget.GetType().GetProperty("Text") != null) {
				Console.WriteLine($"Text - {WidgetEl.widget.GetType()}");
			} else if (WidgetEl.widget.GetType().GetProperty("Label") != null) {
				Console.WriteLine($"Label - {WidgetEl.widget.GetType()}");
			} else {
				Console.WriteLine($"For type '{WidgetEl.widget.GetType()}' output text field not found!");
			}
			//if (WidgetEl.Text != null) {
			//	Console.WriteLine("Its label");
			//}
		}
	}
}
