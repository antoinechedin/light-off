using System.Xml;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[XmlRoot("DialogScript")]
public class DialogScript {

	[XmlArray("DialogLineArray")]
	[XmlArrayItem("DialogLine")]
	public DialogLine[] dialogLineArray;

	public void Save (string path) {
		var serializer = new XmlSerializer (typeof(DialogScript));
		using (var stream = new FileStream(Path.Combine(Application.dataPath, path), FileMode.Create)) {
			serializer.Serialize (stream, this);
		}
	}

	public static DialogScript Load (string path) {
		var serializer = new XmlSerializer (typeof(DialogScript));
		using (var stream = new FileStream (Path.Combine(Application.dataPath, path), FileMode.Open)) {
			return serializer.Deserialize (stream) as DialogScript;
		}
	}

	public static DialogScript LoadFromString (string text) {
		var serializer = new XmlSerializer (typeof(DialogScript));
		return serializer.Deserialize (new StreamReader(text)) as DialogScript;
	}
}
