using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using Gtk;

namespace UndertaleModToolGtk {
	class Localization {
		private string Language;
		private List<(Widget widget, string text)> LocalizedWidgets = new List<(Widget widget, string text)>();
		private Dictionary<string, Dictionary<string, string>> LocalizationData = new Dictionary<string, Dictionary<string, string>>();
		private Dictionary<string, string> Overwrites = new Dictionary<string, string>();

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
			} else if (widget.GetType().GetProperty("Name") != null) {
				var prop = widget.GetType().GetProperty("Name");
				if (prop != null && prop.CanRead) {
					LocalizedWidgets.Add((widget, prop.GetValue(widget).ToString()));
				}
			}
			Update();
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
				foreach ((Widget widget, string text) WidgetEl in LocalizedWidgets) {
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
