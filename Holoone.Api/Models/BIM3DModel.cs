using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class BIM3DModel : BaseModel
	{
		[JsonProperty("cad_model_id")]
		public int CadModelId { get; set; }

		[JsonProperty("meta_data")]
		public string MetaData { get; set; }

		[JsonProperty("updated")]
		public DateTime Updated { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }
		
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("is_primary")]
		public bool IsPrimary { get; set; }
		
		[JsonProperty("processing_failed")]
		public bool ProcessingFailed { get; set; }
		
		[JsonProperty("blocking_collider")]
		public bool BlockingCollider { get; set; }
		
		[JsonProperty("prefab_name")]
		public string PrefabName { get; set; }
		
		[JsonProperty("asset_bundle_data_ios")]
		public string AssetBundleDataIos { get; set; }
		
		[JsonProperty("asset_bundle_data_android")]
		public string AssetBundleDataAndroid { get; set; }
		
		[JsonProperty("asset_bundle_data_uwp")]
		public string AssetBundleDataUwp { get; set; }
		
		[JsonProperty("asset_bundle_data_windows")]
		public string AssetBundleDataWindows { get; set; }
		
		[JsonProperty("asset_bundle_data_lumin")]
		public string AssetBundleDataLumin { get; set; }

		[JsonProperty("asset_bundle_data_webgl")]
		public string AssetBundleDataWebgl { get; set; }

	}
}
