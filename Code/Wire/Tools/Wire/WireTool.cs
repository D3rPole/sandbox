
using Sandbox.UI;

[Title( "Wire" )]
[Icon( "🔋" )]
[ClassName( "wiretool" )]
[Group( "Wire" )]
public class WireTool : ToolMode
{
	public override string Description => "Tool to wire entities";
	public override string PrimaryAction => "Place wire";

	public override void OnControl()
	{
		base.OnControl();
	}
}
