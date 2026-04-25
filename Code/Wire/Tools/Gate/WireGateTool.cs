
using Sandbox;
using Sandbox.UI;
using System.Runtime.CompilerServices;

[Title( "Wire - Gate" )]
[Icon( "🔋" )]
[ClassName( "wiregate" )]
[Group( "Wire" )]
public class WireGateTool : ToolMode
{
	public override string Description => "Tool to place Wire Gates";
	public override string PrimaryAction => "Place Gate";

	[Property, ResourceSelect( Extension = "wigt", AllowPackages = true ), Title( "Gate" )]
	public string GateDefinition { get; set; } = "Wire/Gates/Or/or.wigt";

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

		var pos = select.WorldTransform();
		var placementTx = new Transform( pos.Position );

		var thrusterDef = ResourceLibrary.Get<GateDefinition>(GateDefinition );
		if ( thrusterDef == null ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			Spawn( select, thrusterDef.Prefab, placementTx, true, _previewTint );
			ShootEffects( select );
			_previewTint = Color.Random;
		}
		else if ( Input.Pressed( "attack2" ) )
		{

		}

		DebugOverlay.GameObject( thrusterDef.Prefab.GetScene(), transform: placementTx, castShadows: true);
	}

	[Rpc.Host]
	public void Spawn( SelectionPoint point, PrefabFile thrusterPrefab, Transform tx, bool withRope, Color spawnTint )
	{
		var go = thrusterPrefab.GetScene().Clone( global::Transform.Zero, startEnabled: false );
		go.Tags.Add( "removable" );
		go.WorldTransform = tx;

		ApplyPhysicsProperties( go );

		go.NetworkSpawn( true, null );

		var undo = Player.Undo.Create();
		undo.Name = "Gate";
		undo.Add( go );

		Player.PlayerData?.AddStat( "tool.wire.gate.place" );
	}
}
