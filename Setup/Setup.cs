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
        const string installPath19 = @"%AppDataFolder%\Autodesk Navisworks Simulate 2019\Plugins";
        const string installPath20 = @"%AppDataFolder%\Autodesk Navisworks Simulate 2020\Plugins";
        const string installPath21 = @"%AppDataFolder%\Autodesk Navisworks Simulate 2021\Plugins";
        const string installPath22 = @"%AppDataFolder%\Autodesk Navisworks Simulate 2022\Plugins";
        const string installPathM19 = @"%AppDataFolder%\Autodesk Navisworks Manage 2019\Plugins";
        const string installPathM20 = @"%AppDataFolder%\Autodesk Navisworks Manage 2020\Plugins\HolooneNavis";
        const string installPathM21 = @"%AppDataFolder%\Autodesk Navisworks Manage 2021\Plugins";
        const string installPathM22 = @"%AppDataFolder%\Autodesk Navisworks Manage 2022\Plugins";
        const string sourceDir = "install";

        var project = new Project(projectName)
        {
            Dirs = new[] {
                new InstallDir(installPathM20, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPath20, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPath21, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPath22, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPathM19, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPathM20, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPathM21, FillEntities(null, sourceDir).ToArray()),
                //new InstallDir(installPathM22, FillEntities(null, sourceDir).ToArray()),
            },
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
            Codepage = "1251"
        };

        // the path to wix311 binaries
        Compiler.WixLocation = @"C:\Program Files (x86)\WiX Toolset v3.11\bin";
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