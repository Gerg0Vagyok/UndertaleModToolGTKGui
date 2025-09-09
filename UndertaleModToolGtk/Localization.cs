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

	public void Add(Widget widget) {
		if (widget.GetType().GetProperty("Text") != null) {
			var prop = widget.GetType().GetProperty("Text");
			if (prop != null && prop.CanRead) {
				LocalizedWidgets.Add((widget, prop.GetValue(widget).ToString()));
			}
		} else if (widget.GetType().GetProperty("Label") != null) {
			var prop = widget.GetType().GetProperty("Label");
			if (prop != null && prop.CanRead) {
				LocalizedWidgets.Add((widget, prop.GetValue(widget).ToString()));
			}
		}
	}

	public void Update() {
		if (LocalizationData.ContainsKey(Language)) {
			var LangData = LocalizationData[Language];
			foreach ((Widget widget, String text) WidgetEl in LocalizedWidgets) {
				if (LangData.ContainsKey(WidgetEl.text)) {
					if (WidgetEl.widget.GetType().GetProperty("Text") != null) {
						var prop = WidgetEl.widget.GetType().GetProperty("text");
						if (prop != null && prop.CanWrite) {
							prop.SetValue(WidgetEl.widget, LangData[WidgetEl.text]);
						}
					} else if (WidgetEl.widget.GetType().GetProperty("Label") != null) {
						var prop = WidgetEl.widget.GetType().GetProperty("Label");
						if (prop != null && prop.CanWrite) {
							prop.SetValue(WidgetEl.widget, LangData[WidgetEl.text]);
						}
					} else {
						Console.WriteLine($"For type '{WidgetEl.widget.GetType()}' output text field not found!");
					}
				}
			}
		}
	}
}
