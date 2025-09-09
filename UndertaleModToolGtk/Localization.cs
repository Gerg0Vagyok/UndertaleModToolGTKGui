using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Gtk;

class Localization {
	public String Language;
	public List<(Widget widget, String text)> LocalizedWidgets = new List<(Widget widget, String text)>();
	private Dictionary<String, Dictionary<String, String>> LocalizationData;

	public Localization(String Lang = "English", String LocalizationFileName = "lang.json") {
		Language = Lang;
		LocalizationData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(LocalizationFileName));
	}

	public void Add(Widget widget, String text) {
		LocalizedWidgets.Add((widget, text));
	}

	public void Update() {
		if (LocalizationData.ContainsKey(Language)) {
			var LangData = LocalizationData[Language];
			foreach ((Widget widget, String text) WidgetEl in LocalizedWidgets) {
				if (LangData.ContainsKey(WidgetEl.text)) {
					if (WidgetEl.widget.GetType().GetProperty("Text") != null) {
						WidgetEl.widget.SetProperty("Text", new GLib.Value(LocalizationData[Language][WidgetEl.text]));
						Console.WriteLine($"Text - {WidgetEl.widget.GetType()}");
					} else if (WidgetEl.widget.GetType().GetProperty("Label") != null) {
						Console.WriteLine($"Label - {WidgetEl.widget.GetType()}");
					} else {
						Console.WriteLine($"For type '{WidgetEl.widget.GetType()}' output text field not found!");
					}
				}
			}
		}
	}
}
