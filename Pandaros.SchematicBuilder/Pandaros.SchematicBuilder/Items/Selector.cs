using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pandaros.API.Models;

namespace Pandaros.SchematicBuilder.Items
{
    public class Selector : CSType
    {
        public override string name { get; set; } = GameLoader.NAMESPACE + ".Selector";
        public override string icon { get; set; } = GameLoader.ICON_PATH + "Selector.png";
        public override string mesh { get; set; } = GameLoader.MESH_PATH + "Selector.ply";
        public override bool? isPlaceable { get; set; } = false;
        public override int? destructionTime { get; set; } = 1;
        public override string sideall { get; set; } = "SELF";
        public override List<OnRemove> onRemove { get; set; } = new List<OnRemove>();
    }

    public class SelectorMapping : CSTextureMapping
    {
        public override string name => GameLoader.NAMESPACE + ".Selector";
        public override string albedo => GameLoader.BLOCKS_ALBEDO_PATH + "Selector.png";
    }
}
