//
// Jaller - An advanced IPFS Gateway
// Copyright (C) 2025 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using Jaller.Core.Configuration;
using Jaller.Core.Monitoring;
using Jaller.Tests.Mocks;

namespace Jaller.Tests.Core.Monitoring;

[TestClass]
[DoNotParallelize] // <- Doing file IO, do not parallelize so tests don't step on each other.
public sealed class CanaryFileMonitorTests
{
    // ---------------- Fields ----------------

    private DirectoryInfo? workingDirectory;
    private FileInfo? canaryFile1;
    private FileInfo? canaryFile2;
    private FileInfo? canaryFile3;

    // ---------------- Setup / Teardown ----------------

    [TestInitialize]
    public void TestSetup()
    {
        string? directory = Path.GetDirectoryName( GetType().Assembly.Location );
        Assert.IsNotNull( directory );

        this.workingDirectory = new DirectoryInfo( Path.Combine( directory, GetType().Name ) );

        if( this.workingDirectory.Exists )
        {
            this.workingDirectory.Delete( true );
        }
        this.workingDirectory.Create();

        this.canaryFile1 = new FileInfo( Path.Combine( this.workingDirectory.FullName, "canary1.txt" ) );
        this.canaryFile2 = new FileInfo( Path.Combine( this.workingDirectory.FullName, "canary2.txt" ) );
        this.canaryFile3 = new FileInfo( Path.Combine( this.workingDirectory.FullName, "canary3.txt" ) );
    }

    [TestCleanup]
    public void TestTeardown()
    {
        if( this.workingDirectory?.Exists ?? false )
        {
            if( this.canaryFile1?.Exists ?? false )
            {
                this.canaryFile1.Refresh();
                this.canaryFile1.Delete();
            }

            if( this.canaryFile2?.Exists ?? false )
            {
                this.canaryFile2.Refresh();
                this.canaryFile2.Delete();
            }

            if( this.canaryFile3?.Exists ?? false )
            {
                this.canaryFile3.Refresh();
                this.canaryFile3.Delete();
            }

            this.workingDirectory.Delete( true );
        }
    }

    // ---------------- Properties ----------------

    private FileInfo CanaryFile1
    {
        get
        {
            Assert.IsNotNull( this.canaryFile1 );
            return this.canaryFile1;
        }
    }

    private FileInfo CanaryFile2
    {
        get
        {
            Assert.IsNotNull ( this.canaryFile2 );
            return this.canaryFile2;
        }
    }

    private FileInfo CanaryFile3
    {
        get
        {
            Assert.IsNotNull( this.canaryFile3 );
            return this.canaryFile3;
        }
    }

    // ---------------- Methods ----------------

    [TestMethod]
    public void RefreshWithAllFilesThereTest()
    {
        // Setup
        var config = new JallerMonitoringConfig
        {
            CanaryFiles = [this.CanaryFile1, this.CanaryFile2, this.CanaryFile3]
        };

        CreateFile( CanaryFile1 );
        CreateFile( CanaryFile2 );
        CreateFile( CanaryFile3 );

        var uut = new CanaryFileMonitor( config, new StubLogger() );

        // Act
        uut.Refresh();

        // Check.

        // All files are there, no files should be flagged as missing.
        Assert.AreEqual( 0, uut.GetMissingFiles().Count );
    }

    [TestMethod]
    public void RefreshWithNoFilesThereTest()
    {
        // Setup
        var config = new JallerMonitoringConfig
        {
            CanaryFiles = [this.CanaryFile1, this.CanaryFile2, this.CanaryFile3]
        };

        var uut = new CanaryFileMonitor( config, new StubLogger() );

        // Act
        uut.Refresh();
        IList<FileInfo> missingFiles = uut.GetMissingFiles();

        // Check.

        // All files missing, they should all appear.
        Assert.AreEqual( 3, missingFiles.Count );
        Assert.IsTrue( missingFiles.Any( f => ReferenceEquals( this.CanaryFile1, f ) ) );
        Assert.IsTrue( missingFiles.Any( f => ReferenceEquals( this.CanaryFile2, f ) ) );
        Assert.IsTrue( missingFiles.Any( f => ReferenceEquals( this.CanaryFile3, f ) ) );
    }

    [TestMethod]
    public void RefreshWithAllFilesThenOneGoesAwayTest()
    {
        // Setup
        var config = new JallerMonitoringConfig
        {
            CanaryFiles = [this.CanaryFile1, this.CanaryFile2, this.CanaryFile3]
        };

        CreateFile( CanaryFile1 );
        CreateFile( CanaryFile2 );
        CreateFile( CanaryFile3 );

        var uut = new CanaryFileMonitor( config, new StubLogger() );

        // Act
        uut.Refresh();
        Assert.AreEqual( 0, uut.GetMissingFiles().Count ); // <- Sanity check

        this.CanaryFile2.Delete();
        uut.Refresh();
        IList<FileInfo> missingFiles = uut.GetMissingFiles();

        // Check.

        // One file should be missing.
        Assert.AreEqual( 1, uut.GetMissingFiles().Count );
        Assert.AreSame( missingFiles[0], this.CanaryFile2 );
    }

    [TestMethod]
    public void RefreshWithAllFilesThenOneGoesAwayAndComesBackTest()
    {
        // Setup
        var config = new JallerMonitoringConfig
        {
            CanaryFiles = [this.CanaryFile1, this.CanaryFile2, this.CanaryFile3]
        };

        CreateFile( CanaryFile1 );
        CreateFile( CanaryFile2 );
        CreateFile( CanaryFile3 );

        var uut = new CanaryFileMonitor( config, new StubLogger() );

        // Act
        uut.Refresh();
        Assert.AreEqual( 0, uut.GetMissingFiles().Count ); // <- Sanity check

        this.CanaryFile2.Delete();
        uut.Refresh();
        IList<FileInfo> missingFiles = uut.GetMissingFiles();
        Assert.AreEqual( 1, uut.GetMissingFiles().Count ); // <- Sanity check
        Assert.AreSame( missingFiles[0], this.CanaryFile2 );

        CreateFile( this.CanaryFile2 );
        uut.Refresh();
        missingFiles = uut.GetMissingFiles();

        // Check

        // All files should be accounted for now.
        Assert.AreEqual( 0, missingFiles.Count );
    } 

    // ---------------- Test Helpers ----------------

    private static void CreateFile( FileInfo fileInfo )
    {
        using FileStream fs = fileInfo.Create();
        fs.WriteByte( 0 );
        fs.Close();
    }
}
