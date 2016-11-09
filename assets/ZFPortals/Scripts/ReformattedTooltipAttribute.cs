using UnityEngine;
using System.Text.RegularExpressions;

namespace ZenFulcrum.Portal { 

public class ReformattedTooltipAttribute : TooltipAttribute {

	public ReformattedTooltipAttribute(string tip) : base(Reformat(tip)) {}

	protected static string Reformat(string str) {
		var ret = str.Trim();

		//take text that's formatted in code and make it formatted in the editor
		ret = Regex.Replace(ret, @"(?m)^[ \t]+", "");
		ret = Regex.Replace(ret, @"\n\n", "\n  ");
		ret = Regex.Replace(ret, @"\n(?!\t)", " ");

		return "\t" + ret;
	}

}

}
