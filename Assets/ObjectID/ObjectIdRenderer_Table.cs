// (C) UTJ
using UnityEngine;
using System.Collections.Generic;
using Utj.ObjectIdRendererHelper;

namespace Utj.ObjectIdRendererHelper {
	public class Table {
		public Color defaultColor = Color.black;

		static MaterialPropertyBlock defaultMaterialPropertyBlock;
		Dictionary<string,MaterialPropertyBlock> wildcardToMaterialPropertyBlocks = new Dictionary<string,MaterialPropertyBlock>();

		public bool Load(string csvFilename) {
		    using ( CsvReader reader = new CsvReader(csvFilename))
		    {
				bool firstLine = true;
				var labels = new Dictionary<string, int>();
		        foreach( string[] values in reader.RowEnumerator )
		        {
					if(firstLine) {
						firstLine = false;
						for(int iCol = 0; iCol < values.Length; ++iCol) {
							var s = values[iCol];
							if(s.StartsWith("*")) {
								labels[s] = iCol;
							}
						}
					} else {
						var line = values;
						var wildcard = line[labels["*GameObjectName"]];
						float r = float.TryParse(line[labels["*ColorR"]], out r) ? r : 0.0f;
						float g = float.TryParse(line[labels["*ColorG"]], out g) ? g : 0.0f;
						float b = float.TryParse(line[labels["*ColorB"]], out b) ? b : 0.0f;
						wildcardToMaterialPropertyBlocks[wildcard] = CreateMaterialPropertyBlock(new Color(r, g, b, 1f));
					}
		        }
		    }

			return true;
		}

		static MaterialPropertyBlock CreateMaterialPropertyBlock(Color color) {
			var mpb = new MaterialPropertyBlock();
			mpb.SetColor("_IdColor", color);
			return mpb;
		}

		public MaterialPropertyBlock Get(string gameObjectName) {
			foreach(KeyValuePair<string, MaterialPropertyBlock> kv in wildcardToMaterialPropertyBlocks) {
				var wildcard = kv.Key;
				if(gameObjectName.Like(wildcard)) {
					return kv.Value;
				}
			}
			if(defaultMaterialPropertyBlock == null) {
				defaultMaterialPropertyBlock = CreateMaterialPropertyBlock(defaultColor);
			}
			return defaultMaterialPropertyBlock;
		}
	}
}
