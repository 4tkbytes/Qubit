using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qubit.Engine.Utils
{
    public static class File
    {
        /// <summary>
        /// Reads an embedded resource file and returns its contents as a string.
        /// </summary>
        /// <param name="resourceName">The name of the embedded resource.</param>
        /// <returns>The contents of the resource file as a string.</returns>
        public static string GetEmbeddedResourceString(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new FileNotFoundException($"Resource '{resourceName}' not found.");
            }

            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
