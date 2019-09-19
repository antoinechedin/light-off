using System.Xml;
using System.Xml.Serialization;

public class DialogLine {

	public static class CharPosition {
		public static readonly int LEFT = 0;
		public static readonly int RIGHT = 1;
	}

	[XmlAttribute("position")]
	public int position;
	[XmlText]
	public string text;

	public DialogLine () {
		this.position = -1;
		this.text = "";
	}

	public DialogLine (int charPosition, string text) {
		this.position = charPosition;
		this.text = text;
	}


}
