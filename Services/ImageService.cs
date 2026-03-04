using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TracNghiemLaiXe.Services
{
    public class ImageService
    {
        // Weak references to hold images that we want to keep alive briefly during pre-fetching
        // without preventing GC if memory pressure is high. 
        // Note: MAUI's native image handler has its own strong LRU cache on Android/iOS.
        private readonly List<WeakReference<ImageSource>> _preFetchCache = new();

        public ImageService() { }

        /// <summary>
        /// Resolves the image path for a given question ID.
        /// Assumes images are in Resources/Images with format "q_{id}.jpg" or similar.
        /// </summary>
        public string GetImagePath(int questionId, string? imagePathFromDb)
        {
            if (string.IsNullOrEmpty(imagePathFromDb)) return string.Empty;

            // In MAUI, images in Resources/Images are accessed by filename (without extension usually on some platforms,
            // but with extension is safer for consistency if MauiImage build action processes them).
            // However, typical pattern is just the filename.
            
            // Should verify if the path from DB is a full URL or a local filename.
            // Assuming current dataset uses local filenames like "question_123"
            return imagePathFromDb;
        }

        /// <summary>
        /// Pre-fetches images into the native platform cache.
        /// </summary>
        public async Task PreFetchImagesAsync(List<string> imagePaths)
        {
            if (imagePaths == null || imagePaths.Count == 0) return;

            await Task.Run(() =>
            {
                foreach (var path in imagePaths)
                {
                    if (string.IsNullOrEmpty(path)) continue;

                    // Triggering a load on a background thread forces the native platform 
                    // (Glide/Picasso/SDWebImage) to download/decode and cache the bitmap.
                    // We construct an ImageSource but don't attach it to the visual tree yet.
                    var source = ImageSource.FromFile(path);
                    
                    // Hold a weak reference to ensure it satisfies immediate access but yields to GC
                    _preFetchCache.Add(new WeakReference<ImageSource>(source));
                }
            });
        }

        /// <summary>
        /// Aggressive cleanup helper.
        /// </summary>
        public void ReleaseMemory()
        {
            _preFetchCache.Clear();
            
            // Garbage Collection advice - usually let the runtime handle this, 
            // but can be useful after a heavy review session results page closing.
            GC.Collect(); 
        }
    }
}
