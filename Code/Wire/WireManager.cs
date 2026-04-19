using Sandbox.Wire.UI.Debug;
using System;
using System.Collections.Generic;
using System.Text;
using static Sandbox.Material;

namespace Sandbox.Wire
{
	class WireManager : GameObjectSystem<WireManager>
	{
		List<BaseWireEntity> wireEntitiesInDebug = new List<BaseWireEntity>();

		GameObject gui;

		public WireManager( Scene scene ) : base( scene )
		{
			Listen( Stage.StartUpdate, 0, Updated, "Updated" );
		}

		private void Updated()
		{
		}

		public void AddWireEntityToDebug( BaseWireEntity baseWireEntity )
		{
			if(gui is null )
			{
				gui = new GameObject(true, "Wire");
				gui.Flags = GameObjectFlags.NotSaved | GameObjectFlags.NotNetworked;
				var screenPanel = gui.Components.Create<ScreenPanel>();
				var debugPanel = gui.Components.Create<DebugPanel>();
				debugPanel.Entities = wireEntitiesInDebug;
				// set screenPanel.targetCamera
				Scene.Root.Children.Add(gui);
			}
			
			if ( baseWireEntity is null )
				return;
			if ( wireEntitiesInDebug.Contains( baseWireEntity ) )
				return;
			wireEntitiesInDebug.Add( baseWireEntity );
		}
		public void RemoveWireEntityFromDebug( BaseWireEntity baseWireEntity )
		{
			if ( baseWireEntity is null )
				return;
			if ( !wireEntitiesInDebug.Contains( baseWireEntity ) )
				return;
			wireEntitiesInDebug.Remove( baseWireEntity );
		}

		public void ClearWireEntitesFromDebug()
		{
			wireEntitiesInDebug.Clear(); 
		}
	}
}
