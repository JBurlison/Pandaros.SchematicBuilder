using Pandaros.API;
using Pipliz.JSON;
using System;
using System.Collections.Generic;
using System.IO;

namespace Pandaros.SchematicBuilder.NBT
{
    public class MappingBlock
    {
        private ushort _index = ushort.MaxValue;

        public int Type { get; set; }
        public int Meta { get; set; }
        public string Name { get; set; }
        public string TextType { get; set; }
        public string CSType { get; set; }
        public ushort CSIndex
        {
            get
            {
                if (_index == ushort.MaxValue)
                {
                    var newType = ColonyBuiltIn.ItemTypes.AIR.Id;
                    
                    if (!string.IsNullOrWhiteSpace(CSType))
                    {
                        if (ItemTypes.IndexLookup.TryGetIndex(CSType, out ushort index))
                            newType = index;
                        else
                        {
                            SchematicBuilderLogger.Log(ChatColor.yellow, "Unable to find CSType {0} from the itemType table for block {1} from mapping the file. This item will be mapped to air.", CSType, Name);
                            _index = ColonyBuiltIn.ItemTypes.AIR.Id;
                        }
                    }
                    else
                    { 
                        SchematicBuilderLogger.Log(ChatColor.yellow, "Item {0} from mapping file has a blank cstype. This item will be mapped to air.", Name);
                        _index = ColonyBuiltIn.ItemTypes.AIR.Id;
                    }

                    _index = newType;
                }

                return _index;
            }
        }
    }
}
