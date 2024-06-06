using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NIFT_CMS
{
    /// <summary>
    /// Class MimeTypeMap.
    /// </summary>
    public class MimeTypeMap
    {
        private const string Dot = ".";
        private const string QuestionMark = "?";
        private const string DefaultMimeType = "application/octet-stream";
        private static readonly Lazy<IDictionary<string, string>> _mappings = new Lazy<IDictionary<string, string>>(BuildMappings);

        private static IDictionary<string, string> BuildMappings()
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {

                #region Big freaking list of mime types

                // maps both ways,
                // extension -> mime type
                //   and
                // mime type -> extension
                //
                // any mime types on left side not pre-loaded on right side, are added automatically
                // some mime types can map to multiple extensions, so to get a deterministic mapping,
                // add those to the dictionary specifically
                //
                // combination of values from Windows 7 Registry and 
                // from C:\Windows\System32\inetsrv\config\applicationHost.config
                // some added, including .7z and .dat
                //
                // Some added based on http://www.iana.org/assignments/media-types/media-types.xhtml
                // which lists mime types, but not extensions
                //
                {".JPEG", "image/pjpeg"},
                {".JPG", "image/jpeg"},
                { ".XPNG", "image/png"},
                { ".PNG", "image/x-png"},
                {".PDF", "application/pdf"},
                {".DOCX", "application/msword"},

                #endregion

            };

            var cache = mappings.ToList(); // need ToList() to avoid modifying while still enumerating

            foreach (var mapping in cache)
            {
                if (!mappings.ContainsKey(mapping.Value))
                {
                    mappings.Add(mapping.Value, mapping.Key);
                }
            }

            return mappings;
        }

        /// <summary>
        /// Tries to get the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <param name="mimeType">The variable to store the MIME type.</param>
        /// <returns>The MIME type.</returns>
        /// <exception cref="ArgumentNullException" />
        public static bool TryGetMimeType(string str, out string mimeType)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            var indexQuestionMark = str.IndexOf(QuestionMark, StringComparison.Ordinal);
            if (indexQuestionMark != -1)
            {
                str = str.Remove(indexQuestionMark);
            }


            if (!str.StartsWith(Dot))
            {
                var index = str.LastIndexOf(Dot);
                if (index != -1 && str.Length > index + 1)
                {
                    str = str.Substring(index + 1);
                }

                str = Dot + str;
            }

            return _mappings.Value.TryGetValue(str, out mimeType);
        }

        /// <summary>
        /// Gets the type of the MIME from the provided string.
        /// </summary>
        /// <param name="str">The filename or extension.</param>
        /// <returns>The MIME type.</returns>
        /// <exception cref="ArgumentNullException" />
        public static string GetMimeType(string str)
        {
            return MimeTypeMap.TryGetMimeType(str, out var result) ? result : DefaultMimeType;
        }

        /// <summary>
        /// Gets the extension from the provided MINE type.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <param name="throwErrorIfNotFound">if set to <c>true</c>, throws error if extension's not found.</param>
        /// <returns>The extension.</returns>
        /// <exception cref="ArgumentNullException" />
        /// <exception cref="ArgumentException" />
        public static string GetExtension(string mimeType, bool throwErrorIfNotFound = true)
        {
            if (mimeType == null)
            {
                throw new ArgumentNullException(nameof(mimeType));
            }

            if (mimeType.StartsWith(Dot))
            {
                return "false";
            }

            if (_mappings.Value.TryGetValue(mimeType, out string extension))
            {
                return extension;
            }

            if (throwErrorIfNotFound)
            {
                return "false";
            }

            return string.Empty;
        }
    }
}