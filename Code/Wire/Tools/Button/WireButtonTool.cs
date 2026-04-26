
using Sandbox;
using Sandbox.UI;
using System.Runtime.CompilerServices;

[Title( "Wire - Button" )]
[Icon( "🔋" )]
[ClassName( "wirebutton" )]
[Group( "Wire" )]
public class WireButtonTool : ToolMode
{
	public override string Description => "Tool to place Wire Buttons";
	public override string PrimaryAction => "Place Button";

	[Property, ResourceSelect( Extension = "wibt", AllowPackages = true ), Title( "Button" )]
	public string ButtonDefinition { get; set; } = "Wire/Buttons/Button/Button.wibt";

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

		var buttonDef = ResourceLibrary.Get<ButtonDefinition>(ButtonDefinition );
		if ( buttonDef == null ) return;

		if ( Input.Pressed( "attack1" ) )
		{
			Spawn( select, buttonDef.Prefab, placementTx, true, _previewTint );
			ShootEffects( select );
			_previewTint = Color.Random;
		}
		else if ( Input.Pressed( "attack2" ) )
		{

		}

		DebugOverlay.GameObject( buttonDef.Prefab.GetScene(), transform: placementTx, castShadows: true);
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
		undo.Name = "Button";
		undo.Add( go );

		Player.PlayerData?.AddStat( "tool.wire.button.place" );
	}
}
