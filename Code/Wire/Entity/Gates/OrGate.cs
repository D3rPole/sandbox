using System;
using System.Collections.Generic;
using System.Text;

[WireGate( "Or Gate" )]
public class OrGate : BaseWireEntity
{
	public override string Name => "Or Gate";

	[WireInput( "A" )]
	public bool A { get; set; }

	[WireInput( "B" )]
	public bool B { get; set; }

	[WireOutput( "Out" )]
	public bool Out { get; set; }
	protected override void OnUpdate()
	{

	}

	protected override void OnInputChanged( string inputName )
	{
	
	}
}
