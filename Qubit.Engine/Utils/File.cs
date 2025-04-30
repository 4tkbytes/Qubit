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
            // Get the executing assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Ensure resourceName is properly formatted with the assembly namespace
            // if not already fully qualified
            if (!resourceName.Contains('.'))
            {
                string assemblyNamespace = assembly.GetName().Name;
                resourceName = $"{assemblyNamespace}.{resourceName}";
            }

            // Try to get the manifest resource stream
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);

            // Handle the case when the resource isn't found
            if (stream == null)
            {
                // Get available resources to help with debugging
                string[] availableResources = assembly.GetManifestResourceNames();

                // Create helpful error message with available resources
                string errorMessage = $"Resource '{resourceName}' not found. Available resources:\n";
                errorMessage += string.Join("\n", availableResources);

                throw new FileNotFoundException(errorMessage);
            }

            // Read and return the resource content
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
