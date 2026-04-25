
using Sandbox.UI;
using Sandbox.Wire.UI.WireTool;

[Title( "Wire" )]
[Icon( "🔋" )]
[ClassName( "wiretool" )]
[Group( "Wire" )]
public class WireTool : ToolMode
{
	public override string Description => "Tool to wire entities";
	public override string PrimaryAction => "Wire component";
	public override string SecondaryAction => "Unwire component";
	public override string ReloadAction => inputEntity is null && outputEntity is null ? null : "Abort wiring";
	public override bool AbsorbMouseInput => propertySelection?.Entity is not null && (inputEntity is not null && inputPropertyDescription is null || outputEntity is not null && outputPropertyDescription is null);
	public override bool DisallowWeaponSwitching => propertySelection?.Entity is not null && (inputEntity is not null && inputPropertyDescription is null || outputEntity is not null && outputPropertyDescription is null);
	private GameObject gui;
	private PropertySelection propertySelection => (gui?.Components.FirstOrDefault( c => c is PropertySelection ) as PropertySelection);

	private BaseWireEntity inputEntity;
	private PropertyDescription inputPropertyDescription;

	private BaseWireEntity outputEntity;
	private PropertyDescription outputPropertyDescription;
	private int step;

	public override void OnControl()
	{
		base.OnControl();

		var select = TraceSelect();
		if ( !select.IsValid() ) return;

		var wireEntity = CheckForWireComponent( select );

		if ( wireEntity is null )
		{
			if ( gui is not null )
			{
				propertySelection.Entity = null;
			}
		}
		else
		{
			if ( gui is null )
			{
				gui = new GameObject( true, "Wire" );
				gui.Flags = GameObjectFlags.NotSaved | GameObjectFlags.NotNetworked;
				var screenPanel = gui.Components.Create<ScreenPanel>();
				var propertySelection = gui.Components.Create<PropertySelection>();
				Scene.Root.Children.Add( gui );
			}
			propertySelection.Entity = wireEntity;
			if ( inputEntity is null || inputPropertyDescription is null )
			{
				propertySelection?.ShowOutputs = false;
			}
			else
			{
				propertySelection.ShowOutputs = true;
			}
		}

		if ( Input.Pressed( "attack1" ) )
		{
			if ( wireEntity is null )
				return;

			switch ( step )
			{
				case 0:
					inputEntity = wireEntity;
					propertySelection.SelectedProperty = 0;
					this.ShootEffects( select );
					step = 1;
					return;
				case 1:
					{
						var property = propertySelection.GetSelection();
						if ( property is null )
							return;
						inputPropertyDescription = property?.Item2;
						step = 2;
						this.ShootEffects( select );
						return;
					}
				case 2:
					outputEntity = wireEntity;
					propertySelection.SelectedProperty = 0;
					this.ShootEffects( select );
					step = 3;
					return;
				case 3:
					{
						var property = propertySelection.GetSelection();
						if ( property is null )
							return;
						outputPropertyDescription = property?.Item2;
						outputEntity.wireConnections.Add(new WireConnection()
						{
							TargetComponent = inputEntity,
							TargetField = inputPropertyDescription,
							OriginField = outputPropertyDescription
						} );
						// Reset before finishing
						inputEntity = null;
						inputPropertyDescription = null;
						outputEntity = null;
						outputPropertyDescription = null;
						this.ShootEffects( select );
						step = 0;
						return;
					}
			}

		}
		else if ( Input.Pressed( "attack2" ) )
		{

		}
		else if ( Input.Pressed( "reload" ) )
		{
			// Reset entire process
			if ( inputEntity is null && outputEntity is null )
				return;
			inputEntity = null;
			inputPropertyDescription = null;
			outputEntity = null;
			outputPropertyDescription = null;
			step = 0;
			this.Toolgun.SpinCoil();
		}
		else if ( Input.MouseWheel.y != 0 )
		{
			if ( DisallowWeaponSwitching )
			{
				propertySelection?.MoveSlot( -(int)Input.MouseWheel.y );
			}
		}
	}

	private BaseWireEntity? CheckForWireComponent( SelectionPoint traceSelect )
	{
		if ( traceSelect.GameObject is null )
			return null;

		BaseWireEntity? baseWireEntity = traceSelect.GameObject.Components.FirstOrDefault( c => c is BaseWireEntity ) as BaseWireEntity;

		if ( baseWireEntity is null )
			return null;

		return baseWireEntity;
	}
}
