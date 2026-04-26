using Sandbox;
using Sandbox.Wire;

namespace Sandbox.Wire;

public class Button : BaseWireEntity, Component.IPressable
{
	public override string Name => "Button";

    [Property, WireOutput( "Out" )]
	public bool Out { get; set; }

    public bool _out;

	public bool Press( IPressable.Event e )
	{
		_out = !_out;
        return true;
	}

	protected override void OnFixedUpdate()
	{
		base.OnFixedUpdate();
        if(Out != _out)
            Out = _out;
	}

	protected override void OnInputChanged( string fieldName )
	{
		throw new NotImplementedException();
	}
}