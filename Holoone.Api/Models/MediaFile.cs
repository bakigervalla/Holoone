using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Holoone.Api.Models
{
    public class MediaFile : BaseModel
    {
        // ID of media files internally stored in Sphere backend (needs to remain an int)
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public int Id { get; set; }

        // ID of the media file hosted outside of sphere i.e. OneDrive
        //[JsonProperty("external_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string ExternalId { get; set; }

        /// <summary>
        /// External file's file system <see cref="FileSystemType"/>
        /// </summary>
        //[JsonProperty("origin")]
        //public string Origin { get; set; }

        /// <summary>
        /// Synonym of Origin field (used in some requests) 
        /// </summary>
        //[JsonProperty("source")]
        //public string Source { get; set; }

        // ID of the company drive
        //[JsonProperty("drive_id", NullValueHandling = NullValueHandling.Ignore)]
        //public string DriveId { get; set; }

        [JsonProperty("parent_folder", NullValueHandling = NullValueHandling.Ignore)]
        public int ParentFolderId { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        //// this file URL will get manipulated in the media panel if spawning from OneDrive where we don't get the download link right away
        //[JsonProperty("file")]
        //public string FileURL { get; set; }

        //[JsonProperty("thumbnail", NullValueHandling = NullValueHandling.Ignore)]
        //public string RelativeThumbnailURL { get; set; }

        //public string ThumbnailURL { get { return RelativeThumbnailURL; } }

        /// <summary>
        /// <see cref="MediaType"/>
        /// </summary>
        [JsonProperty("media_file_type")]
        public string MediaFileType { get; set; }

        //[JsonProperty("media_subtype")]
        //public string MediaSubtype { get; set; }

        //[JsonProperty("model_subtype")]
        //public string ModelSubtype { get; set; }

        //[JsonProperty("overlay_enabled")]
        //public bool OverlayEnabled { get; set; }

        [JsonProperty("file_extension", NullValueHandling = NullValueHandling.Ignore)]
        public string FileExtension { get; set; }

        //[JsonProperty("updated", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime Updated { get; set; }

        //[JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        //public DateTime Created { get; set; }

        //public System.Guid Guid { get; set; } = default(System.Guid);

        //// processing states: failed, completed, processing
        //[JsonProperty("processing_state")]
        //public string ProcessingState { get; set; }

        //public bool Is360Video => Equals(MediaSubtype, VideoSubtypes.Video360);
        //public bool IsBIMModel => (Equals(MediaSubtype, ModelSubtypes.BIMModel) || Equals(ModelSubtype, ModelSubtypes.BIMModel));

        private bool _isSelected;
        [JsonIgnore]
        public bool IsSelected { get => _isSelected; set { _isSelected = value; RaisePropertyChanged(nameof(IsSelected)); } }

        private IList<MediaFile> _mediaFolders;
        [JsonIgnore]
        public IList<MediaFile> SubFolders { get => _mediaFolders; set { _mediaFolders = value; RaisePropertyChanged(nameof(SubFolders)); } }

        //public override string ToString()
        //{
        //    return $"{nameof(Id)}: {Id}, {nameof(Updated)}: {Updated}, {nameof(Created)}: {Created}";
        //}

        //public bool Equals(MediaFile other)
        //{
        //    if (ReferenceEquals(null, other)) return false;
        //    if (ReferenceEquals(this, other)) return true;

        //    if (Id < 0)
        //    {
        //        return Equals(FileURL, other.FileURL);
        //    }

        //    return Equals(Id, other.Id) || Equals(ExternalId, other.ExternalId);
        //}

        //public override bool Equals(object obj)
        //{
        //    if (ReferenceEquals(null, obj)) return false;
        //    if (ReferenceEquals(this, obj)) return true;
        //    if (obj.GetType() != this.GetType()) return false;

        //    return Equals((MediaFile)obj);
        //}

        //public override int GetHashCode()
        //{
        //    unchecked
        //    {
        //        return (Id * 397) ^ (ExternalId != null ? ExternalId.GetHashCode() : 0);
        //    }
        //}
    }
}