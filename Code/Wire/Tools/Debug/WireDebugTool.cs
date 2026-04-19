
using Sandbox;
using Sandbox.UI;
using Sandbox.Wire;
using System.Runtime.CompilerServices;

[Title( "Wire - Debug" )]
[Icon( "🔋" )]
[ClassName( "wiredebug" )]
[Group( "Wire" )]
public class WireDebugTool : ToolMode
{
	public override string Description => "Displays inputs and outputs of a wire component";
	public override string PrimaryAction => "Add component";
	public override string SecondaryAction => "Remove component";
	public override string ReloadAction => "Clear list";


	private Color _previewTint = Color.Random;

	protected override void OnEnabled()
	{
		base.OnEnabled();
		_previewTint = Color.Random;
	}

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		if ( !select.IsValid() ) return;

		var wireEntity = CheckForWireComponent( select );

		if ( Input.Pressed( "attack1" ) )
		{
			if ( wireEntity is not null )
			{
				var wireManager = Scene.GetSystem<WireManager>();
				if ( wireManager is null )
					return;

				wireManager.AddWireEntityToDebug( wireEntity );
				ShootEffects( select );
			}
		}
		else if ( Input.Pressed( "attack2" ) )
		{
			if ( wireEntity is not null )
			{

				var wireManager = Scene.GetSystem<WireManager>();
				if ( wireManager is null )
					return;

				wireManager.RemoveWireEntityFromDebug( wireEntity );
				ShootEffects( select );
			}
		}else if( Input.Pressed( "reload" ) )
		{
			var wireManager = Scene.GetSystem<WireManager>();
			if ( wireManager is null )
				return;

			wireManager.ClearWireEntitesFromDebug();
		}
	}

	private BaseWireEntity? CheckForWireComponent(SelectionPoint traceSelect)
	{
		if ( traceSelect.GameObject is null )
			return null;

		BaseWireEntity? baseWireEntity = traceSelect.GameObject.Components.FirstOrDefault( c => c is BaseWireEntity ) as BaseWireEntity;

		if ( baseWireEntity is null )
			return null;

		return baseWireEntity;
	}
}
