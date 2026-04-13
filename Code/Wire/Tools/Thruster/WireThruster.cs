using Sandbox.UI;

[Title( "Wire - Thruster" )]
[Icon( "🚀" )]
[ClassName( "wiretool" )]
[Group( "Wire" )]
public class WireThruster : ToolMode
{
	public override string Description => "Tool to place wire thrusters";
	public override string PrimaryAction => "Place thruster";

	public override void OnControl()
	{
		base.OnControl();
	}
}
