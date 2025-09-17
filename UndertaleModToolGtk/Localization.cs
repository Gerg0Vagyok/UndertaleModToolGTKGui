using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Gtk;

namespace UndertaleModToolGtk {
	class Localization {
		private class WidgetTextClass {
			public Widget widget;
			public string text;
			public WidgetTextClass(Widget widget, string text) {
				this.widget = widget;
				this.text = text;
			}
		}

		private string Language;
		private readonly List<WidgetTextClass> LocalizedWidgets = new List<WidgetTextClass>();
		private readonly Dictionary<string, Dictionary<string, string>> LocalizationData = new Dictionary<string, Dictionary<string, string>>();
		private readonly Dictionary<string, string> Overwrites = new Dictionary<string, string>();

		public Localization(string Lang = "English", string LocalizationFileName = "lang.json") {
			Language = Lang;
			LocalizationData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(File.ReadAllText(LocalizationFileName));
		}

		public string GetLanguage() {
			return Language;
		}

		public void SetLanguage(string Lang) {
			Language = Lang;
			Update();
		}

		public void Add(Widget widget) { // Add a widget
			if (widget.GetType().GetProperty("Text") != null) {
				var prop = widget.GetType().GetProperty("Text");
				if (prop != null && prop.CanRead) {
					LocalizedWidgets.Add(new WidgetTextClass(widget, prop.GetValue(widget).ToString()));
				}
			} else if (widget.GetType().GetProperty("Label") != null) {
				var prop = widget.GetType().GetProperty("Label");
				if (prop != null && prop.CanRead) {
					LocalizedWidgets.Add(new WidgetTextClass(widget, prop.GetValue(widget).ToString()));
				}
			} else if (widget.GetType().GetProperty("Name") != null) {
				var prop = widget.GetType().GetProperty("Name");
				if (prop != null && prop.CanRead) {
					LocalizedWidgets.Add(new WidgetTextClass(widget, prop.GetValue(widget).ToString()));
				}
			}
			Update();
		}

		public void AddArr<T>(List<T> Widgets) where T : Widget{ // Add an array of widgets
			foreach(Widget widget in Widgets) {
				Add(widget);
			}
		}

		public void ClearOverwrites() {
			Overwrites.Clear();
		}

		public void AddOverwrite(string LocalizationID, string Text) {
			Overwrites[LocalizationID] = Text;
		}

		public void DeleteOverwrite(string LocalizationID) {
			Overwrites.Remove(LocalizationID);
		}

		public string GetOverwrite(string LocalizationID) {
			return Overwrites[LocalizationID];
		}

		public void Update() {
			if (LocalizationData.ContainsKey(Language)) {
			var LangData = LocalizationData[Language];
				foreach (WidgetTextClass WidgetEl in LocalizedWidgets) {
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
						} else if (WidgetEl.widget.GetType().GetProperty("Name") != null) {
							var prop = WidgetEl.widget.GetType().GetProperty("Name");
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
}
