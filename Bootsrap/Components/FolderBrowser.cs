using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Bootstrap.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Bootstrap.Components
{
    /// <summary>
    /// Code-behind for the <b>FolderBrowser</b> view component.
    /// </summary>
    public class FolderBrowser: ViewComponent
    {
        private FileTreeConfig _config;

        public FolderBrowser(IOptions<FileTreeConfig> config)
        {
            _config = config.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync(string dir, bool showFiles = false)
        {
            var browsingRoot = Path.Combine(_config.BaseDir, dir);
            var nodes = new List<TreeNode>();
            nodes.AddRange(RecurseDirectory(browsingRoot));
            return new ContentViewComponentResult(JsonConvert.SerializeObject(nodes));
        }

        private List<TreeNode> RecurseDirectory(string directory)
        {
            var ret = new List<TreeNode>();
            var dirInfo = new DirectoryInfo(directory);

            try
            {
                var directories = dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
                foreach (var dir in directories)
                {
                    if (dir.FullName.ToLower() == dirInfo.FullName)
                    {
                        continue;
                    }
                    var thisNode = TreeNode.FromDirInfo(dir);
                    thisNode.Nodes.AddRange(RecurseDirectory(dir.FullName));
                    ret.Add(thisNode);
                }
            }
            catch (UnauthorizedAccessException ux)
            {
                // NB Log.
            }

            return ret;
        }
    }
}
