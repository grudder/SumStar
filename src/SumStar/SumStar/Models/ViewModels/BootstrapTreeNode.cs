using System.Collections.Generic;

using Newtonsoft.Json;

namespace SumStar.Models.ViewModels
{
	/// <summary>
	/// Bootstrap树节点
	/// </summary>
	public class BootstrapTreeNode
	{
		[JsonProperty("text")]
		public string Text
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
		{
			get;
			set;
		}

		[JsonProperty("selectedIcon")]
		public string SelectedIcon
		{
			get;
			set;
		}

		[JsonProperty("color")]
		public string Color
		{
			get;
			set;
		}

		[JsonProperty("backColor")]
		public string BackColor
		{
			get;
			set;
		}

		[JsonProperty("href")]
		public string Href
		{
			get;
			set;
		}

		[JsonProperty("selectable")]
		public string Selectable
		{
			get;
			set;
		}

		[JsonProperty("state")]
		public State State
		{
			get;
			set;
		}

		[JsonProperty("tags")]
		public IList<string> Tags
		{
			get;
			set;
		}

		[JsonProperty("nodes")]
		public IList<BootstrapTreeNode> Nodes
		{
			get;
			set;
		}
	}

	public class State
	{
		[JsonProperty("checked")]
		public bool Checked
		{
			get;
			set;
		}

		[JsonProperty("disabled")]
		public bool Disabled
		{
			get;
			set;
		}

		[JsonProperty("expanded")]
		public bool Expanded
		{
			get;
			set;
		}

		[JsonProperty("selected")]
		public bool Selected
		{
			get;
			set;
		}
	}
}