using Sandbox;

[WireGate("And Gate")]
public sealed class WireEntity : BaseWireEntity
{
	[WireInput("A")]
	public bool A {  get; set; }

	[WireOutput("Out")]
	public bool Out {  get; set; }
	protected override void OnUpdate()
	{

	}
}
