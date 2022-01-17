using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp;

class Script
{
    static public void Main(string[] args)
    {
        const string projectName = "Holoone Navisworks integration";
        string navisDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // $"{Environment.ExpandEnvironmentVariables("%ProgramW6432%")}\\Autodesk\\";
        string[] versions = new string[] { "2019", "2020", "2021", "2022" };
        IList<Dir> dirs = new List<Dir>();

        foreach (var version in versions)
        {
            string pathManage = Path.Combine(navisDir, $"Autodesk Navisworks Manage {version}");
            string pathSimulate = Path.Combine(navisDir, $"Autodesk Navisworks Simulate {version}");

            if (Directory.Exists(pathManage))
                dirs.Add(new Dir(Path.Combine(pathManage, "Plugins", "HolooneNavis"), FillEntities(null, "install").ToArray()));
            if (Directory.Exists(pathSimulate))
                dirs.Add(new Dir(Path.Combine(pathSimulate, "Plugins", "HolooneNavis"), FillEntities(null, "install").ToArray()));
        }

        foreach(var dir in dirs)
        {
            if (!Directory.Exists(dir.Name))
                Directory.CreateDirectory(dir.Name);
        }
        var project = new Project(projectName)
        {
            Dirs = dirs.ToArray(),
            Description = "Holoone integration for Autodesk Navisworks",
            Package =
                    {
                AttributesDefinition =
                    @"AdminImage=yes; Comments=Holoone integration for Autodesk Navisworks; Description=Holoone integration for Autodesk Navisworks"
                    },
            GUID = new Guid("{B5AF2152-9F97-4B14-AA75-E2E25A78BF1B}"),
            UpgradeCode = new Guid("{69B86E21-99B1-4E97-9A54-2F908AD3DC3A}"),
            MajorUpgradeStrategy = new MajorUpgradeStrategy
            {
                UpgradeVersions = VersionRange.ThisAndOlder,
                PreventDowngradingVersions = VersionRange.NewerThanThis,
                NewerProductInstalledErrorMessage = "Newer version already installed",
                RemoveExistingProductAfter = Step.InstallInitialize
            },
            Version = new Version(0, 1, 1),
            UI = WUI.WixUI_ProgressOnly,
            InstallScope = InstallScope.perMachine,
            ControlPanelInfo = { Manufacturer = "Holo-one", ProductIcon = "holo_one_logo.ico" },
            Encoding = Encoding.UTF8,
            LicenceFile = string.Empty,
            Platform = Platform.x64,
            Codepage = "1251"
        };

        Compiler.BuildMsi(project);
    }

    private static IEnumerable<WixEntity> FillEntities(Dir parent, string path)
    {
        var files = Directory
            .EnumerateFiles(path, "*.*")
            .Select(x => new WixSharp.File(x));

        var childDirectories = Directory.EnumerateDirectories(path);

        var dirs = new List<Dir>();

        foreach (var dirPath in childDirectories)
        {
            var dir = new Dir(Path.GetFileName(dirPath));

            FillEntities(dir, dirPath);

            dirs.Add(dir);
        }

        if (parent != null)
        {
            parent.Files = files.ToArray();

            parent.Dirs = dirs.ToArray();
        }

        var result = new List<WixEntity>();

        result.AddRange(files);
        result.AddRange(dirs);

        return result;
    }
}